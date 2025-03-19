using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class MapVisualization : MonoBehaviour
{
    [SerializeField]
    private GameObject pointPrefab;
    
    [SerializeField]
    private float pointSize = 0.02f;
    
    [SerializeField]
    private Color mapPointColor = Color.green;
    
    private List<GameObject> mapPointVisualizers = new List<GameObject>();
    
    // 맵 데이터 저장 클래스
    [System.Serializable]
    public class MapData
    {
        public List<Vector3> points = new List<Vector3>();
        public List<Vector3> planeNormals = new List<Vector3>();
        public List<Vector3> planePositions = new List<Vector3>();
        public List<Vector2> planeSizes = new List<Vector2>();
    }
    
    // 3D 맵 데이터 저장
    public void SaveMap(ARPointCloudManager pointCloudManager, ARPlaneManager planeManager, string filename = "ar_map.json")
    {
        MapData mapData = new MapData();
        
        // 모든 포인트 클라우드 데이터 수집
        foreach (var pointCloud in pointCloudManager.trackables)
        {
            if (pointCloud.positions.HasValue)
            {
                mapData.points.AddRange(pointCloud.positions.Value);
            }
        }
        
        // 평면 데이터 수집
        foreach (var plane in planeManager.trackables)
        {
            mapData.planeNormals.Add(plane.normal);
            mapData.planePositions.Add(plane.center);
            mapData.planeSizes.Add(new Vector2(plane.size.x, plane.size.y));
        }
        
        // 데이터 JSON으로 변환
        string json = JsonUtility.ToJson(mapData);
        
        // 파일에 저장
        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, json);
        
        Debug.Log($"3D 맵이 저장되었습니다: {path}");
    }
    
    // 저장된 3D 맵 로드, 시각화
    public void LoadAndVisualizeMap(string filename = "ar_map.json")
    {
        // 기존 시각화 포인트 제거
        ClearMapVisualization();
        
        string path = Path.Combine(Application.persistentDataPath, filename);
        if (!File.Exists(path))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {path}");
            return;
        }
        
        // 파일에서 JSON 데이터 읽기
        string json = File.ReadAllText(path);
        MapData mapData = JsonUtility.FromJson<MapData>(json);
        
        Debug.Log($"3D 맵을 로드했습니다. 포인트 개수: {mapData.points.Count}, 평면 개수: {mapData.planeNormals.Count}");
        
        // 포인트 시각화
        foreach (var point in mapData.points)
        {
            GameObject pointVisualizer = Instantiate(pointPrefab, point, Quaternion.identity);
            pointVisualizer.transform.SetParent(transform);
            pointVisualizer.transform.localScale = Vector3.one * pointSize;
            
            // 저장된 맵 포인트 색상 설정
            if (pointVisualizer.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.color = mapPointColor;
            }
            
            mapPointVisualizers.Add(pointVisualizer);
        }
        
        // 평면 시각화
        for (int i = 0; i < mapData.planeNormals.Count; i++)
        {
            Vector3 position = mapData.planePositions[i];
            Vector3 normal = mapData.planeNormals[i];
            Vector2 size = mapData.planeSizes[i];
            
            GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            planeObj.transform.position = position;
            
            planeObj.transform.up = normal;
            
            planeObj.transform.localScale = new Vector3(size.x, 0.01f, size.y);
            
            Renderer planeRenderer = planeObj.GetComponent<Renderer>();
            Material planeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            planeMat.color = new Color(0, 0.8f, 1, 0.3f);
            planeRenderer.material = planeMat;
            
            planeObj.transform.SetParent(transform);
            mapPointVisualizers.Add(planeObj);
        }
    }
    
    // 시각화 제거
    public void ClearMapVisualization()
    {
        foreach (var obj in mapPointVisualizers)
        {
            Destroy(obj);
        }
        mapPointVisualizers.Clear();
    }
}