using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class InteractiveElement : MonoBehaviour
{
    [System.Serializable]
    public enum ElementLayer
    {
        Ambient,
        Biological,
        Cognitive,
        Philosophical
    }
    
    [Header("Element Settings")]
    public ElementLayer layer;
    public string elementName;
    [TextArea(2, 5)]
    public string elementDescription;
    
    [Header("Visual Feedback")]
    public Material defaultMaterial;
    public Material hoveredMaterial;
    public Material selectedMaterial;
    public float pulseSpeed = 1.5f;
    public float pulseAmount = 0.1f;
    public bool useGlow = true;
    
    [Header("Audio Feedback")]
    public AudioClip hoverSound;
    public AudioClip selectSound;
    
    [Header("Interactable Settings")]
    public XRSimpleInteractable interactable;
    public bool highlightOnHover = true;
    public bool rotateOnHover = true;
    public float rotationSpeed = 20f;
    
    [Header("Events")]
    public UnityEvent<string> OnElementSelected;
    
    private Renderer elementRenderer;
    private AudioSource audioSource;
    private bool isHovered = false;
    private bool isSelected = false;
    private Vector3 originalScale;
    private Material originalMaterial;
    private ConsciousnessVisualization visualizationManager;
    private InfoPanelSystem infoPanelSystem;
    
    private void Start()
    {
        // Get required components
        elementRenderer = GetComponent<Renderer>();
        if (elementRenderer == null)
            elementRenderer = GetComponentInChildren<Renderer>();
        
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (hoverSound != null || selectSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();
        
        originalScale = transform.localScale;
        
        if (elementRenderer != null)
            originalMaterial = elementRenderer.material;
        
        // Setup interactable events
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.selectExited.AddListener(OnSelectExit);
        }
        
        // Find managers
        visualizationManager = FindObjectOfType<ConsciousnessVisualization>();
        infoPanelSystem = FindObjectOfType<InfoPanelSystem>();
    }
    
    private void Update()
    {
        if (isHovered && rotateOnHover)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        
        if (isHovered || isSelected)
        {
            PulseScale();
        }
    }
    
    private void PulseScale()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * pulse;
    }
    
    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovered = true;
        
        if (highlightOnHover && elementRenderer != null && hoveredMaterial != null)
        {
            elementRenderer.material = hoveredMaterial;
        }
        
        if (audioSource != null && hoverSound != null)
        {
            audioSource.clip = hoverSound;
            audioSource.Play();
        }
        
        // Show tooltip or highlight
        ShowTooltip();
    }
    
    private void OnHoverExit(HoverExitEventArgs args)
    {
        isHovered = false;
        
        if (!isSelected && elementRenderer != null)
        {
            elementRenderer.material = originalMaterial;
        }
        
        transform.localScale = originalScale;
        
        // Hide tooltip
        HideTooltip();
    }
    
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        isSelected = true;
        
        if (elementRenderer != null && selectedMaterial != null)
        {
            elementRenderer.material = selectedMaterial;
        }
        
        if (audioSource != null && selectSound != null)
        {
            audioSource.clip = selectSound;
            audioSource.Play();
        }
        
        // Trigger layer transition in visualization
        if (visualizationManager != null)
        {
            switch (layer)
            {
                case ElementLayer.Ambient:
                    visualizationManager.ShowAmbientLayer();
                    break;
                case ElementLayer.Biological:
                    visualizationManager.ShowBiologicalLayer();
                    break;
                case ElementLayer.Cognitive:
                    visualizationManager.ShowCognitiveLayer();
                    break;
            }
        }
        
        // Show info panel
        if (infoPanelSystem != null)
        {
            infoPanelSystem.ShowLayerInfo(layer.ToString());
        }
        
        // Invoke custom event
        OnElementSelected?.Invoke(layer.ToString());
    }
    
    private void OnSelectExit(SelectExitEventArgs args)
    {
        isSelected = false;
        
        if (!isHovered && elementRenderer != null)
        {
            elementRenderer.material = originalMaterial;
        }
    }
    
    private void ShowTooltip()
    {
        // Here you would show a tooltip with the element's name and description
        // This would depend on your UI system, but could create a world-space canvas
        // or use an existing tooltip system
        Debug.Log($"Tooltip: {elementName} - {elementDescription}");
    }
    
    private void HideTooltip()
    {
        // Hide the tooltip
    }
    
    // This method can be called by animation events or other scripts
    public void ActivateElement()
    {
        OnSelectEnter(new SelectEnterEventArgs());
    }
    
    // For non-VR testing
    private void OnMouseEnter()
    {
        if (interactable == null)
        {
            OnHoverEnter(new HoverEnterEventArgs());
        }
    }
    
    private void OnMouseExit()
    {
        if (interactable == null)
        {
            OnHoverExit(new HoverExitEventArgs());
        }
    }
    
    private void OnMouseDown()
    {
        if (interactable == null)
        {
            OnSelectEnter(new SelectEnterEventArgs());
        }
    }
    
    private void OnMouseUp()
    {
        if (interactable == null)
        {
            OnSelectExit(new SelectExitEventArgs());
        }
    }
} 