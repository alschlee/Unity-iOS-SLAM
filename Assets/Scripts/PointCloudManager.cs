using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PointCloudManager : MonoBehaviour
{
    [SerializeField]
    private ARPointCloudManager pointCloudManager;
    
    [SerializeField]
    private GameObject pointPrefab;
    
    private Dictionary<ulong, GameObject> pointVisualizers = new Dictionary<ulong, GameObject>();
    
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
        // 새로운 포인트 클라우드 처리
        foreach (var pointCloud in args.added)
        {
            Debug.Log($"새 포인트 클라우드 감지: {pointCloud.trackableId}");
        }
        
        // 포인트 클라우드 업데이트 처리
        foreach (var pointCloud in args.updated)
        {
            UpdatePointCloud(pointCloud);
        }
        
        // 제거된 포인트 클라우드 처리
        foreach (var pointCloud in args.removed)
        {
            Debug.Log($"포인트 클라우드 제거: {pointCloud.trackableId}");
        }
    }
    
    private void UpdatePointCloud(ARPointCloud pointCloud)
    {
        // 각 포인트의 위치 정보 가져오기
        if (pointCloud.positions.HasValue)
        {
            var positions = pointCloud.positions.Value;
            Debug.Log($"포인트 개수: {positions.Length}");
            
            // 나중에 포인트 클라우드 데이터 처리
        }
    }
}