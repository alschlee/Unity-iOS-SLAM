using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SLAMManager : MonoBehaviour
{
    [SerializeField]
    private ARPointCloudManager pointCloudManager;
    
    [SerializeField]
    private ARPlaneManager planeManager;
    
    [SerializeField]
    private MapVisualization mapVisualization;
    
    private string defaultMapFilename = "ar_map.json";
    
    private void Start()
    {
        // Validate required components
        if (pointCloudManager == null)
        {
            Debug.LogError("ARPointCloudManager not assigned to SLAMManager!");
        }
        
        if (planeManager == null)
        {
            Debug.LogError("ARPlaneManager not assigned to SLAMManager!");
        }
        
        if (mapVisualization == null)
        {
            Debug.LogError("MapVisualization not assigned to SLAMManager!");
        }
    }
    
    public void SaveMap()
    {
        if (pointCloudManager == null || planeManager == null || mapVisualization == null)
        {
            Debug.LogError("Cannot save map: Missing required components!");
            return;
        }
        
        mapVisualization.SaveMap(pointCloudManager, planeManager, defaultMapFilename);
        Debug.Log("Map saved successfully!");
    }
    
    public void LoadMap()
    {
        if (mapVisualization == null)
        {
            Debug.LogError("Cannot load map: MapVisualization component is missing!");
            return;
        }
        
        mapVisualization.LoadAndVisualizeMap(defaultMapFilename);
    }
    
    public void ClearVisualization()
    {
        if (mapVisualization != null)
        {
            mapVisualization.ClearMapVisualization();
        }
    }
}