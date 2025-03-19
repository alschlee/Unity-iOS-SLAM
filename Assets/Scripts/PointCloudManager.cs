using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PointCloudManager : MonoBehaviour
{
    [SerializeField]
    private ARPointCloudManager pointCloudManager;
    
    [SerializeField]
    private GameObject pointPrefab;
    
    [SerializeField, Range(0.01f, 1.0f)]
    private float pointSize = 0.02f;
    
    [SerializeField]
    private int maxPoints = 2000; // 성능 문제 방지를 위한 최대 포인트 수 제한
    
    [SerializeField]
    private bool showAllPoints = true;
    
    private List<GameObject> pointVisualizers = new List<GameObject>();
    private Dictionary<TrackableId, List<GameObject>> pointsPerCloud = new Dictionary<TrackableId, List<GameObject>>();
    
    private void OnEnable()
    {
        pointCloudManager.pointCloudsChanged += OnPointCloudsChanged;
    }
    
    private void OnDisable()
    {
        pointCloudManager.pointCloudsChanged -= OnPointCloudsChanged;
    }
    
    private void OnPointCloudsChanged(ARPointCloudChangedEventArgs args)
    {
        // 새 포인트 클라우드 처리
        foreach (var pointCloud in args.added)
        {
            VisualizePointCloud(pointCloud);
        }
        
        // 기존 포인트 클라우드 업데이트
        foreach (var pointCloud in args.updated)
        {
            UpdatePointCloud(pointCloud);
        }
        
        // 제거된 포인트 클라우드 처리
        foreach (var pointCloud in args.removed)
        {
            if (pointsPerCloud.TryGetValue(pointCloud.trackableId, out List<GameObject> points))
            {
                foreach (var point in points)
                {
                    Destroy(point);
                }
                pointsPerCloud.Remove(pointCloud.trackableId);
            }
        }
    }
    
    private void VisualizePointCloud(ARPointCloud pointCloud)
    {
        if (!pointsPerCloud.ContainsKey(pointCloud.trackableId))
        {
            pointsPerCloud[pointCloud.trackableId] = new List<GameObject>();
        }
        
        if (pointCloud.positions.HasValue)
        {
            Vector3[] positions = pointCloud.positions.Value.ToArray();
            Debug.Log($"새 포인트 클라우드 감지: {pointCloud.trackableId}, 포인트 개수: {positions.Length}");
            
            // 신뢰도 값이 있다면 사용
            float[] confidences = null;
            if (pointCloud.confidenceValues.HasValue)
            {
                confidences = pointCloud.confidenceValues.Value.ToArray();
            }
            
            int pointsToCreate = Mathf.Min(positions.Length, maxPoints - pointVisualizers.Count);
            for (int i = 0; i < pointsToCreate; i++)
            {
                float confidence = confidences != null && i < confidences.Length ? confidences[i] : 1.0f;
                
                // 충분한 신뢰도를 가진 포인트만 생성
                if (confidence > 0.5f)
                {
                    GameObject pointVisualizer = Instantiate(pointPrefab, positions[i], Quaternion.identity);
                    pointVisualizer.transform.SetParent(transform);
                    pointVisualizer.transform.localScale = Vector3.one * pointSize;
                    
                    // 신뢰도에 따라 포인트 색상 지정
                    if (pointVisualizer.TryGetComponent<Renderer>(out Renderer renderer))
                    {
                        Color color = new Color(1, 1, 1, confidence);
                        renderer.material.color = color;
                    }
                    
                    pointVisualizers.Add(pointVisualizer);
                    pointsPerCloud[pointCloud.trackableId].Add(pointVisualizer);
                }
            }
        }
    }
    
    private void UpdatePointCloud(ARPointCloud pointCloud)
    {
        if (pointCloud.positions.HasValue)
        {
            Vector3[] positions = pointCloud.positions.Value.ToArray();
            Debug.Log($"포인트 개수: {positions.Length}");
            
            // 모든 포인트를 표시하지 않는 경우, 이 클라우드의 이전 포인트 제거
            if (!showAllPoints && pointsPerCloud.ContainsKey(pointCloud.trackableId))
            {
                foreach (var point in pointsPerCloud[pointCloud.trackableId])
                {
                    pointVisualizers.Remove(point);
                    Destroy(point);
                }
                pointsPerCloud[pointCloud.trackableId].Clear();
            }
            
            // 신뢰도 값 가져오기
            float[] confidences = null;
            if (pointCloud.confidenceValues.HasValue)
            {
                confidences = pointCloud.confidenceValues.Value.ToArray();
            }
            
            // 추가할 수 있는 포인트 수 계산
            int pointsToCreate = Mathf.Min(positions.Length, maxPoints - pointVisualizers.Count);
            for (int i = 0; i < pointsToCreate; i++)
            {
                float confidence = confidences != null && i < confidences.Length ? confidences[i] : 1.0f;
                
                // 충분한 신뢰도를 가진 포인트만 생성
                if (confidence > 0.5f)
                {
                    GameObject pointVisualizer = Instantiate(pointPrefab, positions[i], Quaternion.identity);
                    pointVisualizer.transform.SetParent(transform);
                    pointVisualizer.transform.localScale = Vector3.one * pointSize;
                    
                    // 신뢰도에 따라 포인트 색상 지정
                    if (pointVisualizer.TryGetComponent<Renderer>(out Renderer renderer))
                    {
                        Color color = new Color(1, 1, 1, confidence);
                        renderer.material.color = color;
                    }
                    
                    pointVisualizers.Add(pointVisualizer);
                    pointsPerCloud[pointCloud.trackableId].Add(pointVisualizer);
                }
            }
            
            if (pointVisualizers.Count >= maxPoints)
            {
                Debug.LogWarning($"최대 포인트 개수({maxPoints})에 도달했습니다. 모든 포인트가 표시되지 않을 수 있습니다.");
            }
        }
    }
    
    public void ClearAllPoints()
    {
        foreach (var point in pointVisualizers)
        {
            Destroy(point);
        }
        pointVisualizers.Clear();
        pointsPerCloud.Clear();
        Debug.Log("모든 포인트 삭제됨");
    }
    
    public int GetPointCount()
    {
        return pointVisualizers.Count;
    }
}