/*
    _____  _____  _____  _____  ______
        |  _____ |      |      |  ___|
        |  _____ |      |      |     |
    
     U       N       I       T      Y
                                         
    
    TerraUnity Co. - Earth Simulation Tools - 2016
    
    http://terraunity.com
    info@terraunity.com
    
    This script is written for Unity 3D Engine.
    Unity 3D Version: Unity 5.x
    
    
    INFO: This script creates a temporary render texture from a virtual camera and detects first pixel color from the camera screen view.

    Written by: Amir Badamchi
    
*/


using UnityEngine;
using System.Collections;

public class ParticleColors : MonoBehaviour
{
    private Camera cam;
    private GameObject cameraObject;
    private RenderTexture renderTexture;
    Texture2D screenTexture;
    private int resolution = 1;
    private UnityEngine.Color pixelColor;
    private LayerMask terrainMask;
    public string terrainLayerName = "Terrain";

    public void SetRenderCamera ()
    {
        cameraObject = new GameObject("Particle Camera");
        cameraObject.transform.parent = transform;
        cameraObject.transform.localPosition = Vector3.zero;
        cameraObject.transform.localEulerAngles = new Vector3(90f, 0f,0f);
        cameraObject.transform.localScale = Vector3.one;
        cameraObject.transform.hideFlags = HideFlags.HideInHierarchy;
        cameraObject.AddComponent<Camera>();

        cam = cameraObject.GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 0.1f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 10f;
        cam.depth = -100;
        cam.useOcclusionCulling = false;
        
        if(cam.renderingPath == RenderingPath.VertexLit)
            cam.renderingPath = RenderingPath.DeferredLighting;
        
        cam.aspect = 1;

        // All main terrains in the scene are in the 8th layer named "Terrain"
        terrainMask = 1 << LayerMask.NameToLayer(terrainLayerName);
        cam.cullingMask = terrainMask;
    }

    public UnityEngine.Color GetColor ()
    {
        renderTexture = RenderTexture.GetTemporary(resolution, resolution, 16, RenderTextureFormat.ARGB32);

        cam.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        cam.Render();
        screenTexture = new Texture2D(resolution, resolution);
        screenTexture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);

        pixelColor = screenTexture.GetPixel(1, 1);

        return pixelColor;
    }

    public void ClearRenderTexture ()
    {
        RenderTexture.active = null;
        cam.targetTexture = null;
        Destroy(screenTexture);
        RenderTexture.ReleaseTemporary(renderTexture);
        renderTexture = null;
    }
}

