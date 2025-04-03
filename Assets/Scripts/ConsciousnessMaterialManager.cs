using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creates and manages materials for the consciousness visualization
/// </summary>
public class ConsciousnessMaterialManager : MonoBehaviour
{
    // Singleton instance
    public static ConsciousnessMaterialManager Instance { get; private set; }
    
    [Header("Ambient Layer Materials")]
    public Material ambientWaveMaterial;
    public Material ambientParticleMaterial;
    
    [Header("Biological Layer Materials")]
    public Material biologicalBoundaryMaterial;
    public Material biologicalRingMaterial;
    public Material biologicalNodeMaterial;
    public Material biologicalLoopMaterial;
    public Material biologicalConnectionMaterial;
    
    [Header("Cognitive Layer Materials")]
    public Material cognitiveBoundaryMaterial;
    public Material cognitiveCoreMaterial;
    public Material cognitiveWaveMaterial;
    public Material cognitiveNodeMaterial;
    public Material cognitiveConnectionMaterial;
    
    [Header("UI Materials")]
    public Material uiPanelMaterial;
    
    [Header("Platform Materials")]
    public Material teleportSurfaceMaterial;
    public Material ambientPlatformMaterial;
    public Material biologicalPlatformMaterial;
    public Material cognitivePlatformMaterial;
    
    [Header("Global Settings")]
    public float globalEmissionIntensity = 1.5f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Initialize all materials at runtime if not already set
    private void Start()
    {
        if (ambientWaveMaterial == null)
        {
            CreateAllMaterials();
        }
    }
    
    public void CreateAllMaterials()
    {
        // Create ambient layer materials
        ambientWaveMaterial = CreateLuminousMaterial("AmbientWaveMaterial", new Color(0.2f, 0.3f, 0.6f, 0.4f), 0.3f);
        ambientParticleMaterial = CreateLuminousMaterial("AmbientParticleMaterial", new Color(0.4f, 0.5f, 0.9f, 0.5f), 0.5f);
        
        // Create biological layer materials
        biologicalBoundaryMaterial = CreateLuminousMaterial("BiologicalBoundaryMaterial", new Color(0.2f, 0.4f, 0.8f, 0.4f), 0.4f);
        biologicalRingMaterial = CreateLuminousMaterial("BiologicalRingMaterial", new Color(0.3f, 0.5f, 0.9f, 0.5f), 0.5f);
        biologicalNodeMaterial = CreateLuminousMaterial("BiologicalNodeMaterial", new Color(0.5f, 0.7f, 1.0f, 0.7f), 0.7f);
        biologicalLoopMaterial = CreateLuminousMaterial("BiologicalLoopMaterial", new Color(0.4f, 0.6f, 0.9f, 0.5f), 0.5f);
        biologicalConnectionMaterial = CreateLuminousMaterial("BiologicalConnectionMaterial", new Color(0.4f, 0.6f, 0.9f, 0.5f), 0.4f);
        
        // Create cognitive layer materials
        cognitiveBoundaryMaterial = CreateLuminousMaterial("CognitiveBoundaryMaterial", new Color(0.7f, 0.8f, 1.0f, 0.6f), 0.6f);
        cognitiveCoreMaterial = CreateLuminousMaterial("CognitiveCoreMaterial", new Color(1.0f, 1.0f, 1.0f, 0.7f), 1.0f);
        cognitiveWaveMaterial = CreateLuminousMaterial("CognitiveWaveMaterial", new Color(0.8f, 0.9f, 1.0f, 0.6f), 0.7f);
        cognitiveNodeMaterial = CreateLuminousMaterial("CognitiveNodeMaterial", new Color(0.9f, 0.95f, 1.0f, 0.8f), 0.8f);
        cognitiveConnectionMaterial = CreateLuminousMaterial("CognitiveConnectionMaterial", new Color(0.8f, 0.9f, 1.0f, 0.6f), 0.6f);
        
        // Create UI materials
        uiPanelMaterial = CreateUIMaterial("UIPanelMaterial", new Color(0.1f, 0.1f, 0.2f, 0.8f));
        
        // Create platform materials
        teleportSurfaceMaterial = CreateLuminousMaterial("TeleportSurfaceMaterial", new Color(0.1f, 0.1f, 0.3f, 0.4f), 0.2f);
        ambientPlatformMaterial = CreateLuminousMaterial("AmbientPlatformMaterial", new Color(0.2f, 0.3f, 0.5f, 0.6f), 0.3f);
        biologicalPlatformMaterial = CreateLuminousMaterial("BiologicalPlatformMaterial", new Color(0.3f, 0.5f, 0.7f, 0.6f), 0.4f);
        cognitivePlatformMaterial = CreateLuminousMaterial("CognitivePlatformMaterial", new Color(0.4f, 0.6f, 0.9f, 0.6f), 0.5f);
    }
    
    private Material CreateLuminousMaterial(string name, Color color, float emissionStrength)
    {
        // Use the custom glowing shader if available, otherwise fallback to standard
        Shader glowingShader = Shader.Find("Custom/GlowingShader");
        Material material = glowingShader != null 
            ? new Material(glowingShader) 
            : new Material(Shader.Find("Standard"));
        
        material.name = name;
        
        // Set the main color (albedo)
        material.color = color;
        
        if (glowingShader != null)
        {
            // Configure GlowingShader specific properties
            material.SetColor("_EmissionColor", color * emissionStrength * globalEmissionIntensity);
            material.SetFloat("_EmissionIntensity", emissionStrength * globalEmissionIntensity);
            material.SetFloat("_FresnelPower", 2.0f);
            material.SetFloat("_PulseSpeed", 1.0f);
            material.SetFloat("_PulseAmount", 0.2f);
            material.SetFloat("_AlphaFalloff", 1.5f);
        }
        else
        {
            // Standard shader configuration
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            
            // Enable emission for the glow effect
            material.EnableKeyword("_EMISSION");
            
            // Set emission color - usually a more intense version of the main color
            Color emissionColor = new Color(
                Mathf.Clamp01(color.r * 1.5f),
                Mathf.Clamp01(color.g * 1.5f),
                Mathf.Clamp01(color.b * 1.5f),
                color.a
            );
            
            material.SetColor("_EmissionColor", emissionColor * emissionStrength * globalEmissionIntensity);
            
            // Adjust smoothness and metallic properties for a more glowing appearance
            material.SetFloat("_Glossiness", 0.7f);
            material.SetFloat("_Metallic", 0.2f);
        }
        
        return material;
    }
    
    private Material CreateUIMaterial(string name, Color color)
    {
        // Create material specifically for UI elements
        Material material = new Material(Shader.Find("UI/Default"));
        material.name = name;
        material.color = color;
        
        return material;
    }
    
#if UNITY_EDITOR
    // Editor utility to create all materials as assets
    [MenuItem("Consciousness VR/Create Material Assets")]
    public static void CreateMaterialAssets()
    {
        ConsciousnessMaterialManager tempManager = new GameObject("TempMaterialManager").AddComponent<ConsciousnessMaterialManager>();
        tempManager.CreateAllMaterials();
        
        SaveMaterialAsset(tempManager.ambientWaveMaterial, "AmbientWaveMaterial");
        SaveMaterialAsset(tempManager.ambientParticleMaterial, "AmbientParticleMaterial");
        
        SaveMaterialAsset(tempManager.biologicalBoundaryMaterial, "BiologicalBoundaryMaterial");
        SaveMaterialAsset(tempManager.biologicalRingMaterial, "BiologicalRingMaterial");
        SaveMaterialAsset(tempManager.biologicalNodeMaterial, "BiologicalNodeMaterial");
        SaveMaterialAsset(tempManager.biologicalLoopMaterial, "BiologicalLoopMaterial");
        SaveMaterialAsset(tempManager.biologicalConnectionMaterial, "BiologicalConnectionMaterial");
        
        SaveMaterialAsset(tempManager.cognitiveBoundaryMaterial, "CognitiveBoundaryMaterial");
        SaveMaterialAsset(tempManager.cognitiveCoreMaterial, "CognitiveCoreMaterial");
        SaveMaterialAsset(tempManager.cognitiveWaveMaterial, "CognitiveWaveMaterial");
        SaveMaterialAsset(tempManager.cognitiveNodeMaterial, "CognitiveNodeMaterial");
        SaveMaterialAsset(tempManager.cognitiveConnectionMaterial, "CognitiveConnectionMaterial");
        
        SaveMaterialAsset(tempManager.uiPanelMaterial, "UIPanelMaterial");
        
        SaveMaterialAsset(tempManager.teleportSurfaceMaterial, "TeleportSurfaceMaterial");
        SaveMaterialAsset(tempManager.ambientPlatformMaterial, "AmbientPlatformMaterial");
        SaveMaterialAsset(tempManager.biologicalPlatformMaterial, "BiologicalPlatformMaterial");
        SaveMaterialAsset(tempManager.cognitivePlatformMaterial, "CognitivePlatformMaterial");
        
        // Destroy the temporary game object
        DestroyImmediate(tempManager.gameObject);
        
        Debug.Log("All materials created successfully!");
    }
    
    private static void SaveMaterialAsset(Material material, string materialName)
    {
        // Create directory if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        
        // Save the material as an asset
        string path = "Assets/Materials/" + materialName + ".mat";
        AssetDatabase.CreateAsset(material, path);
    }
#endif

    // Methods to apply materials to specific components
    public void ApplyMaterialsToAmbientLayer(AmbientConsciousnessLayer layer)
    {
        if (layer == null) return;
        
        layer.waveMaterial = ambientWaveMaterial;
        layer.particleMaterial = ambientParticleMaterial;
    }
    
    public void ApplyMaterialsToBiologicalLayer(BiologicalConsciousnessLayer layer)
    {
        if (layer == null) return;
        
        layer.boundaryMaterial = biologicalBoundaryMaterial;
        layer.ringMaterial = biologicalRingMaterial;
        layer.nodeMaterial = biologicalNodeMaterial;
        layer.loopMaterial = biologicalLoopMaterial;
        layer.connectionMaterial = biologicalConnectionMaterial;
    }
    
    public void ApplyMaterialsToCognitiveLayer(CognitiveConsciousnessLayer layer)
    {
        if (layer == null) return;
        
        layer.boundaryMaterial = cognitiveBoundaryMaterial;
        layer.coreMaterial = cognitiveCoreMaterial;
        layer.wavePatternMaterial = cognitiveWaveMaterial;
        layer.conceptNodeMaterial = cognitiveNodeMaterial;
        layer.selfNodeMaterial = cognitiveNodeMaterial;
        layer.connectionMaterial = cognitiveConnectionMaterial;
    }
} 