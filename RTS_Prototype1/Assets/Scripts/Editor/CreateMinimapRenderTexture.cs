using UnityEngine;
using UnityEditor;

public class CreateMinimapRenderTexture
{
    [MenuItem("Assets/Create/RTS/Minimap Render Texture")]
    public static void CreateRenderTextureAsset()
    {
        RenderTexture rt = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
        rt.name = "MinimapRenderTexture";
        rt.filterMode = FilterMode.Bilinear;
        rt.Create();

        string path = "Assets/Minimap/MinimapRenderTexture.renderTexture";
        AssetDatabase.CreateAsset(rt, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Created Render Texture at: {path}");
    }
}