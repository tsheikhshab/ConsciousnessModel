using UnityEngine;
using UnityEditor;

public class CreateMaterialsEditor : Editor
{
    [MenuItem("Consciousness VR/Create Material Assets")]
    public static void CreateMaterialAssets()
    {
        // Check if the Materials folder exists, create it if not
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        // Create a temporary material manager to generate all materials
        GameObject tempObj = new GameObject("TempMaterialManager");
        ConsciousnessMaterialManager materialManager = tempObj.AddComponent<ConsciousnessMaterialManager>();
        materialManager.CreateAllMaterials();

        // Save all the materials as assets
        SaveMaterialAsset(materialManager.ambientWaveMaterial, "AmbientWaveMaterial");
        SaveMaterialAsset(materialManager.ambientParticleMaterial, "AmbientParticleMaterial");
        
        SaveMaterialAsset(materialManager.biologicalBoundaryMaterial, "BiologicalBoundaryMaterial");
        SaveMaterialAsset(materialManager.biologicalRingMaterial, "BiologicalRingMaterial");
        SaveMaterialAsset(materialManager.biologicalNodeMaterial, "BiologicalNodeMaterial");
        SaveMaterialAsset(materialManager.biologicalLoopMaterial, "BiologicalLoopMaterial");
        SaveMaterialAsset(materialManager.biologicalConnectionMaterial, "BiologicalConnectionMaterial");
        
        SaveMaterialAsset(materialManager.cognitiveBoundaryMaterial, "CognitiveBoundaryMaterial");
        SaveMaterialAsset(materialManager.cognitiveCoreMaterial, "CognitiveCoreMaterial");
        SaveMaterialAsset(materialManager.cognitiveWaveMaterial, "CognitiveWaveMaterial");
        SaveMaterialAsset(materialManager.cognitiveNodeMaterial, "CognitiveNodeMaterial");
        SaveMaterialAsset(materialManager.cognitiveConnectionMaterial, "CognitiveConnectionMaterial");
        
        SaveMaterialAsset(materialManager.uiPanelMaterial, "UIPanelMaterial");
        
        SaveMaterialAsset(materialManager.teleportSurfaceMaterial, "TeleportSurfaceMaterial");
        SaveMaterialAsset(materialManager.ambientPlatformMaterial, "AmbientPlatformMaterial");
        SaveMaterialAsset(materialManager.biologicalPlatformMaterial, "BiologicalPlatformMaterial");
        SaveMaterialAsset(materialManager.cognitivePlatformMaterial, "CognitivePlatformMaterial");
        
        // Destroy the temporary game object
        DestroyImmediate(tempObj);
        
        Debug.Log("All materials created successfully!");
    }
    
    private static void SaveMaterialAsset(Material material, string materialName)
    {
        if (material == null) return;
        
        string path = "Assets/Materials/" + materialName + ".mat";
        
        // Check if material already exists
        Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existingMaterial != null)
        {
            // Update existing material
            EditorUtility.CopySerialized(material, existingMaterial);
            AssetDatabase.SaveAssets();
            Debug.Log("Updated existing material: " + materialName);
        }
        else
        {
            // Create new material asset
            AssetDatabase.CreateAsset(material, path);
            Debug.Log("Created new material: " + materialName);
        }
    }
} 