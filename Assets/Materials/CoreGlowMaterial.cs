using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creates a special high-intensity glowing material for the cognitive core
/// </summary>
public class CoreGlowMaterial : MonoBehaviour
{
    public Material cognitiveCoreMaterial;
    public float pulseSpeed = 1.0f;
    public float pulseAmount = 0.3f;
    public float emissionIntensity = 3.0f;
    
    private float initialIntensity;
    
    private void Start()
    {
        if (cognitiveCoreMaterial == null)
        {
            CreateCoreMaterial();
        }
        
        initialIntensity = emissionIntensity;
    }
    
    private void Update()
    {
        if (cognitiveCoreMaterial != null)
        {
            // Create a pulsating effect for the core
            float pulseFactor = 1.0f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            cognitiveCoreMaterial.SetFloat("_EmissionIntensity", initialIntensity * pulseFactor);
        }
    }
    
    public void CreateCoreMaterial()
    {
        // Find the glowing shader
        Shader glowingShader = Shader.Find("Custom/GlowingShader");
        if (glowingShader == null)
        {
            Debug.LogWarning("GlowingShader not found, using standard shader instead.");
            
            // Create a standard material if the shader is not found
            cognitiveCoreMaterial = new Material(Shader.Find("Standard"));
            cognitiveCoreMaterial.name = "CognitiveCore";
            cognitiveCoreMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
            
            // Enable emission for glow effect
            cognitiveCoreMaterial.EnableKeyword("_EMISSION");
            cognitiveCoreMaterial.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f, 1.0f) * emissionIntensity);
            
            // Set transparency
            cognitiveCoreMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            cognitiveCoreMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            cognitiveCoreMaterial.SetInt("_ZWrite", 0);
            cognitiveCoreMaterial.DisableKeyword("_ALPHATEST_ON");
            cognitiveCoreMaterial.EnableKeyword("_ALPHABLEND_ON");
            cognitiveCoreMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            cognitiveCoreMaterial.renderQueue = 3000;
            
            return;
        }
        
        // Create material with the glowing shader
        cognitiveCoreMaterial = new Material(glowingShader);
        cognitiveCoreMaterial.name = "CognitiveCore";
        
        // Set the base color (slightly transparent white)
        Color coreColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        cognitiveCoreMaterial.color = coreColor;
        
        // Configure the shader properties for intense glow
        cognitiveCoreMaterial.SetColor("_EmissionColor", coreColor * 2.0f);
        cognitiveCoreMaterial.SetFloat("_EmissionIntensity", emissionIntensity);
        cognitiveCoreMaterial.SetFloat("_FresnelPower", 1.5f);
        cognitiveCoreMaterial.SetFloat("_PulseSpeed", pulseSpeed);
        cognitiveCoreMaterial.SetFloat("_PulseAmount", pulseAmount);
        cognitiveCoreMaterial.SetFloat("_AlphaFalloff", 1.2f);
    }
    
#if UNITY_EDITOR
    [MenuItem("Consciousness VR/Create Core Glow Material")]
    public static void CreateCoreGlowMaterial()
    {
        // Find the glowing shader
        Shader glowingShader = Shader.Find("Custom/GlowingShader");
        if (glowingShader == null)
        {
            Debug.LogError("Custom/GlowingShader not found. Cannot create core glow material.");
            return;
        }
        
        // Create material with the glowing shader
        Material coreMaterial = new Material(glowingShader);
        coreMaterial.name = "CognitiveCoreMaterial";
        
        // Set the base color (slightly transparent white)
        Color coreColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        coreMaterial.color = coreColor;
        
        // Configure the shader properties for intense glow
        coreMaterial.SetColor("_EmissionColor", coreColor * 2.0f);
        coreMaterial.SetFloat("_EmissionIntensity", 3.0f);
        coreMaterial.SetFloat("_FresnelPower", 1.5f);
        coreMaterial.SetFloat("_PulseSpeed", 1.0f);
        coreMaterial.SetFloat("_PulseAmount", 0.3f);
        coreMaterial.SetFloat("_AlphaFalloff", 1.2f);
        
        // Save the material as an asset
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        
        string path = "Assets/Materials/CognitiveCoreMaterial.mat";
        
        // Check if material already exists
        Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existingMaterial != null)
        {
            EditorUtility.CopySerialized(coreMaterial, existingMaterial);
            AssetDatabase.SaveAssets();
            Debug.Log("Updated existing material: CognitiveCoreMaterial");
        }
        else
        {
            AssetDatabase.CreateAsset(coreMaterial, path);
            Debug.Log("Created new material: CognitiveCoreMaterial");
        }
        
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Material>(path);
    }
#endif
} 