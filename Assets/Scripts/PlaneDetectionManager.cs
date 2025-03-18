using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneDetectionManager : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager planeManager;
    
    [SerializeField]
    private GameObject planePrefab;
    
    private List<ARPlane> detectedPlanes = new List<ARPlane>();
    
    private void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }
    
    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }
    
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // 새로 감지된 평면 처리
        foreach (ARPlane plane in args.added)
        {
            Debug.Log($"새 평면 감지: {plane.trackableId}");
            detectedPlanes.Add(plane);
        }
        
        // 업데이트된 평면 처리
        foreach (ARPlane plane in args.updated)
        {
            Debug.Log($"평면 업데이트: {plane.trackableId}");
        }
        
        // 제거된 평면 처리
        foreach (ARPlane plane in args.removed)
        {
            Debug.Log($"평면 제거: {plane.trackableId}");
            detectedPlanes.Remove(plane);
        }
    }
    
    // 감지된 평면 수 반환
    public int GetDetectedPlanesCount()
    {
        return detectedPlanes.Count;
    }
}