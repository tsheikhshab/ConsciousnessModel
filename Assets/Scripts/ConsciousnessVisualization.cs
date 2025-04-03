using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConsciousnessVisualization : MonoBehaviour
{
    [Header("Layer Control")]
    public GameObject ambientLayer;
    public GameObject biologicalLayer;
    public GameObject cognitiveLayer;
    
    [Header("Transition Effects")]
    public float transitionSpeed = 2.0f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Audio")]
    public AudioSource ambientAudioSource;
    public AudioSource biologicalAudioSource;
    public AudioSource cognitiveAudioSource;
    
    [Header("VR Interaction")]
    public XRGrabInteractable grabInteractable;
    public Transform centerPoint;
    
    [Header("Materials")]
    public ConsciousnessMaterialManager materialManager;
    
    public enum VisualizationLayer
    {
        Ambient,
        Biological,
        Cognitive,
        All
    }
    
    private VisualizationLayer currentLayer = VisualizationLayer.All;
    private Coroutine transitionCoroutine;
    
    private void Start()
    {
        // Setup material manager if not assigned
        if (materialManager == null)
        {
            materialManager = FindObjectOfType<ConsciousnessMaterialManager>();
            
            // If still null, try to use the singleton instance
            if (materialManager == null && ConsciousnessMaterialManager.Instance != null)
            {
                materialManager = ConsciousnessMaterialManager.Instance;
            }
            
            // If still null, create a new one
            if (materialManager == null)
            {
                GameObject materialManagerObj = new GameObject("MaterialManager");
                materialManager = materialManagerObj.AddComponent<ConsciousnessMaterialManager>();
                materialManager.CreateAllMaterials();
            }
        }
        
        // Apply materials to layers
        ApplyMaterialsToLayers();
        
        // Initialize all layers
        SetLayerActive(ambientLayer, true);
        SetLayerActive(biologicalLayer, true);
        SetLayerActive(cognitiveLayer, true);
        
        // Initialize audio
        if (ambientAudioSource) ambientAudioSource.Play();
        if (biologicalAudioSource) biologicalAudioSource.volume = 0.2f;
        if (cognitiveAudioSource) cognitiveAudioSource.volume = 0.2f;
        
        // Setup VR interaction
        if (grabInteractable)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }
    
    // Apply materials from the material manager to all layers
    private void ApplyMaterialsToLayers()
    {
        if (materialManager == null) return;
        
        // Apply materials to Ambient layer
        if (ambientLayer != null)
        {
            AmbientConsciousnessLayer ambientComponent = ambientLayer.GetComponent<AmbientConsciousnessLayer>();
            if (ambientComponent != null)
            {
                materialManager.ApplyMaterialsToAmbientLayer(ambientComponent);
            }
        }
        
        // Apply materials to Biological layer
        if (biologicalLayer != null)
        {
            BiologicalConsciousnessLayer biologicalComponent = biologicalLayer.GetComponent<BiologicalConsciousnessLayer>();
            if (biologicalComponent != null)
            {
                materialManager.ApplyMaterialsToBiologicalLayer(biologicalComponent);
            }
        }
        
        // Apply materials to Cognitive layer
        if (cognitiveLayer != null)
        {
            CognitiveConsciousnessLayer cognitiveComponent = cognitiveLayer.GetComponent<CognitiveConsciousnessLayer>();
            if (cognitiveComponent != null)
            {
                materialManager.ApplyMaterialsToCognitiveLayer(cognitiveComponent);
            }
        }
    }
    
    public void TransitionToLayer(int layerIndex)
    {
        VisualizationLayer targetLayer = (VisualizationLayer)layerIndex;
        TransitionToLayer(targetLayer);
    }
    
    public void TransitionToLayer(VisualizationLayer targetLayer)
    {
        if (currentLayer == targetLayer) return;
        
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        transitionCoroutine = StartCoroutine(TransitionLayersCoroutine(targetLayer));
        currentLayer = targetLayer;
    }
    
    private IEnumerator TransitionLayersCoroutine(VisualizationLayer targetLayer)
    {
        float time = 0f;
        
        // Initial states for fading
        bool shouldShowAmbient = targetLayer == VisualizationLayer.Ambient || targetLayer == VisualizationLayer.All;
        bool shouldShowBiological = targetLayer == VisualizationLayer.Biological || targetLayer == VisualizationLayer.All;
        bool shouldShowCognitive = targetLayer == VisualizationLayer.Cognitive || targetLayer == VisualizationLayer.All;
        
        // Current states
        bool isAmbientActive = ambientLayer.activeSelf;
        bool isBiologicalActive = biologicalLayer.activeSelf;
        bool isCognitiveActive = cognitiveLayer.activeSelf;
        
        // Activate layers that need to be visible during transition
        if (shouldShowAmbient && !isAmbientActive) SetLayerActive(ambientLayer, true, 0f);
        if (shouldShowBiological && !isBiologicalActive) SetLayerActive(biologicalLayer, true, 0f);
        if (shouldShowCognitive && !isCognitiveActive) SetLayerActive(cognitiveLayer, true, 0f);
        
        // Initial audio volumes
        float ambientVolume = ambientAudioSource ? ambientAudioSource.volume : 0f;
        float biologicalVolume = biologicalAudioSource ? biologicalAudioSource.volume : 0f;
        float cognitiveVolume = cognitiveAudioSource ? cognitiveAudioSource.volume : 0f;
        
        // Target audio volumes
        float targetAmbientVolume = shouldShowAmbient ? 1f : 0.2f;
        float targetBiologicalVolume = shouldShowBiological ? 1f : 0.2f;
        float targetCognitiveVolume = shouldShowCognitive ? 1f : 0.2f;
        
        while (time < 1f)
        {
            time += Time.deltaTime * transitionSpeed;
            float t = transitionCurve.Evaluate(time);
            
            // Fade layers
            FadeLayer(ambientLayer, isAmbientActive, shouldShowAmbient, t);
            FadeLayer(biologicalLayer, isBiologicalActive, shouldShowBiological, t);
            FadeLayer(cognitiveLayer, isCognitiveActive, shouldShowCognitive, t);
            
            // Fade audio
            if (ambientAudioSource)
                ambientAudioSource.volume = Mathf.Lerp(ambientVolume, targetAmbientVolume, t);
            if (biologicalAudioSource)
                biologicalAudioSource.volume = Mathf.Lerp(biologicalVolume, targetBiologicalVolume, t);
            if (cognitiveAudioSource)
                cognitiveAudioSource.volume = Mathf.Lerp(cognitiveVolume, targetCognitiveVolume, t);
            
            yield return null;
        }
        
        // Final states
        if (!shouldShowAmbient) SetLayerActive(ambientLayer, false);
        if (!shouldShowBiological) SetLayerActive(biologicalLayer, false);
        if (!shouldShowCognitive) SetLayerActive(cognitiveLayer, false);
        
        transitionCoroutine = null;
    }
    
    private void FadeLayer(GameObject layer, bool isActive, bool shouldBeActive, float t)
    {
        if (layer == null) return;
        
        if (isActive && !shouldBeActive)
        {
            // Fading out
            SetLayerAlpha(layer, 1f - t);
        }
        else if (!isActive && shouldBeActive)
        {
            // Fading in
            SetLayerAlpha(layer, t);
        }
    }
    
    private void SetLayerActive(GameObject layer, bool active, float alpha = 1f)
    {
        if (layer == null) return;
        
        layer.SetActive(active);
        if (active)
        {
            SetLayerAlpha(layer, alpha);
        }
    }
    
    private void SetLayerAlpha(GameObject layer, float alpha)
    {
        Renderer[] renderers = layer.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                Color color = material.color;
                color.a = alpha;
                material.color = color;
                
                // Ensure transparency is enabled
                if (alpha < 1f)
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                }
                else
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                }
            }
        }
        
        // Handle particle systems if present
        ParticleSystem[] particleSystems = layer.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            Color startColor = main.startColor.color;
            startColor.a = alpha;
            main.startColor = startColor;
        }
    }
    
    private void OnReleased(SelectExitEventArgs args)
    {
        // Handle when user releases the object after grabbing
        // Calculate where they are in relation to the visualization
        if (centerPoint != null && grabInteractable != null)
        {
            float distance = Vector3.Distance(grabInteractable.transform.position, centerPoint.position);
            
            // Determine which layer to transition to based on distance
            if (distance < 1f)
            {
                TransitionToLayer(VisualizationLayer.Cognitive);
            }
            else if (distance < 3f)
            {
                TransitionToLayer(VisualizationLayer.Biological);
            }
            else
            {
                TransitionToLayer(VisualizationLayer.Ambient);
            }
        }
    }
    
    // Called by UI buttons or voice commands
    public void ShowAllLayers()
    {
        TransitionToLayer(VisualizationLayer.All);
    }
    
    public void ShowAmbientLayer()
    {
        TransitionToLayer(VisualizationLayer.Ambient);
    }
    
    public void ShowBiologicalLayer()
    {
        TransitionToLayer(VisualizationLayer.Biological);
    }
    
    public void ShowCognitiveLayer()
    {
        TransitionToLayer(VisualizationLayer.Cognitive);
    }
} 