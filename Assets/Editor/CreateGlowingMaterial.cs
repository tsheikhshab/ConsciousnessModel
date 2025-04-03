using UnityEngine;
using UnityEditor;

public class CreateGlowingMaterial : Editor
{
    [MenuItem("Consciousness VR/Create Glowing Material Template")]
    public static void CreateGlowingMaterialTemplate()
    {
        // Check if the Materials folder exists, create it if not
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        
        // Find the glowing shader
        Shader glowingShader = Shader.Find("Custom/GlowingShader");
        if (glowingShader == null)
        {
            Debug.LogError("Custom/GlowingShader not found. Make sure the shader is properly imported.");
            return;
        }
        
        // Create a material using the glowing shader
        Material glowingMaterial = new Material(glowingShader);
        glowingMaterial.name = "GlowingMaterialTemplate";
        
        // Set properties
        Color mainColor = new Color(0.5f, 0.7f, 1.0f, 0.8f);
        glowingMaterial.color = mainColor;
        
        // Configure GlowingShader specific properties
        glowingMaterial.SetColor("_EmissionColor", mainColor * 1.5f);
        glowingMaterial.SetFloat("_EmissionIntensity", 2.0f);
        glowingMaterial.SetFloat("_FresnelPower", 2.0f);
        glowingMaterial.SetFloat("_PulseSpeed", 0.8f);
        glowingMaterial.SetFloat("_PulseAmount", 0.3f);
        glowingMaterial.SetFloat("_AlphaFalloff", 1.5f);
        
        // Save as asset
        string path = "Assets/Materials/GlowingMaterialTemplate.mat";
        
        // Check if material already exists
        Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existingMaterial != null)
        {
            EditorUtility.CopySerialized(glowingMaterial, existingMaterial);
            AssetDatabase.SaveAssets();
            Debug.Log("Updated existing material: GlowingMaterialTemplate");
        }
        else
        {
            AssetDatabase.CreateAsset(glowingMaterial, path);
            Debug.Log("Created new material: GlowingMaterialTemplate");
        }
        
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Material>(path);
    }
} 