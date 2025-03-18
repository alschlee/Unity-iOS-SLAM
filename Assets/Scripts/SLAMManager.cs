using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class SLAMManager : MonoBehaviour
{
    [SerializeField]
    private ARSession arSession;
    
    [SerializeField]
    private ARCameraManager cameraManager;
    
    [SerializeField]
    private PlaneDetectionManager planeDetectionManager;
    
    [SerializeField]
    private PointCloudManager pointCloudManager;
    
    [SerializeField]
    private Text statusText;
    
    private bool isMapping = false;
    
    public void StartMapping()
    {
        isMapping = true;
        Debug.Log("매핑 시작");
        
        if (statusText != null)
            statusText.text = "매핑 진행 중~~~ 주변을 천천히 스캔하세요 ^_^";
    }
    
    public void StopMapping()
    {
        isMapping = false;
        Debug.Log("매핑 중지");
        
        if (statusText != null)
            statusText.text = "매핑 완료. 감지된 평면: " + planeDetectionManager.GetDetectedPlanesCount();
    }
    
    public void SaveMap()
    {
        Debug.Log("3D 맵 저장");
        
        // 나중에 매핑 데이터 저장 로직 추가
        
        if (statusText != null)
            statusText.text = "3D 맵이 저장되었습니다.";
    }
    
    // 카메라 위치 추적 (로컬라이제이션)
    public Vector3 GetCurrentCameraPosition()
    {
        if (cameraManager != null && cameraManager.transform != null)
        {
            return cameraManager.transform.position;
        }
        
        return Vector3.zero;
    }
}