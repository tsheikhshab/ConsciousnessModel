using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class InfoPanelSystem : MonoBehaviour
{
    [System.Serializable]
    public class InfoPanel
    {
        public string title;
        [TextArea(3, 10)]
        public string description;
        [TextArea(2, 5)]
        public string quote;
        public Sprite image;
    }
    
    [Header("Info Panels")]
    public InfoPanel ambientPanel;
    public InfoPanel biologicalPanel;
    public InfoPanel cognitivePanel;
    public InfoPanel philosophicalPanel;
    
    [Header("UI References")]
    public Canvas infoCanvas;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI quoteText;
    public Image panelImage;
    public Button closeButton;
    
    [Header("VR Interaction")]
    public XRRayInteractor rayInteractor;
    public Transform followCamera;
    public float followDistance = 2f;
    public float followSpeed = 5f;
    public float followRotationSpeed = 5f;
    
    private InfoPanel currentPanel;
    private bool isShowing = false;
    private ConsciousnessVisualization consciousnessManager;
    
    private void Start()
    {
        // Set default panel content
        SetupDefaultPanels();
        
        // Hide panel initially
        TogglePanel(false);
        
        // Setup close button
        if (closeButton)
            closeButton.onClick.AddListener(() => TogglePanel(false));
        
        // Find the consciousness manager
        consciousnessManager = FindObjectOfType<ConsciousnessVisualization>();
    }
    
    private void Update()
    {
        if (isShowing)
        {
            FollowCamera();
        }
    }
    
    private void SetupDefaultPanels()
    {
        if (ambientPanel == null)
        {
            ambientPanel = new InfoPanel
            {
                title = "Ambient Consciousness",
                description = "The lowest level of consciousness - present everywhere as a fundamental aspect of information. " +
                             "This represents the baseline of consciousness that exists throughout the universe.",
                quote = "Consciousness exists outside the organism but it remains at a low, ambient level."
            };
        }
        
        if (biologicalPanel == null)
        {
            biologicalPanel = new InfoPanel
            {
                title = "Biological Consciousness",
                description = "The first threshold of consciousness occurs in living systems through autocatalytic closure. " +
                             "Biological systems amplify ambient consciousness by creating self-reinforcing information processes.",
                quote = "The origin of life through autocatalytic closure introduced a first level of self-proliferating information processes."
            };
        }
        
        if (cognitivePanel == null)
        {
            cognitivePanel = new InfoPanel
            {
                title = "Cognitive Consciousness",
                description = "A second level of amplification through the development of integrated worldviews. " +
                             "Human consciousness emerges through conceptual closure - a worldview that weaves together disparate concepts and experiences.",
                quote = "The origin of an integrated understanding of the world—i.e., a worldview—through conceptual closure."
            };
        }
        
        if (philosophicalPanel == null)
        {
            philosophicalPanel = new InfoPanel
            {
                title = "Philosophical Implications",
                description = "This theory bridges panpsychism and emergentism by showing how consciousness exists at a basic level " +
                             "everywhere but becomes amplified through specific structures and processes. This provides a solution to " +
                             "the 'combination problem' in consciousness studies.",
                quote = "Like Fourier analysis shows how complex waves can be both unitary and composed of simpler waves, a unified " +
                       "conscious experience can both be unitary and composed of simpler conscious elements."
            };
        }
    }
    
    public void ShowAmbientPanel()
    {
        ShowPanel(ambientPanel);
    }
    
    public void ShowBiologicalPanel()
    {
        ShowPanel(biologicalPanel);
    }
    
    public void ShowCognitivePanel()
    {
        ShowPanel(cognitivePanel);
    }
    
    public void ShowPhilosophicalPanel()
    {
        ShowPanel(philosophicalPanel);
    }
    
    public void ShowPanel(InfoPanel panel)
    {
        if (panel == null) return;
        
        currentPanel = panel;
        
        // Update UI elements
        titleText.text = panel.title;
        descriptionText.text = panel.description;
        quoteText.text = "\"" + panel.quote + "\"";
        
        if (panel.image != null)
        {
            panelImage.sprite = panel.image;
            panelImage.gameObject.SetActive(true);
        }
        else
        {
            panelImage.gameObject.SetActive(false);
        }
        
        // Show the panel
        TogglePanel(true);
    }
    
    public void TogglePanel(bool show)
    {
        isShowing = show;
        
        if (infoCanvas)
            infoCanvas.gameObject.SetActive(show);
        
        // If showing, position in front of the camera
        if (show && followCamera)
        {
            transform.position = followCamera.position + followCamera.forward * followDistance;
            transform.rotation = Quaternion.LookRotation(transform.position - followCamera.position);
        }
    }
    
    private void FollowCamera()
    {
        if (followCamera == null) return;
        
        // Calculate target position in front of camera
        Vector3 targetPosition = followCamera.position + followCamera.forward * followDistance;
        
        // Smoothly move panel toward target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        
        // Calculate target rotation (facing camera)
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - followCamera.position);
        
        // Smoothly rotate panel
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followRotationSpeed);
    }
    
    // Called by interactable objects in the visualization
    public void ShowLayerInfo(string layerName)
    {
        switch (layerName.ToLower())
        {
            case "ambient":
                ShowAmbientPanel();
                if (consciousnessManager != null)
                    consciousnessManager.ShowAmbientLayer();
                break;
                
            case "biological":
                ShowBiologicalPanel();
                if (consciousnessManager != null)
                    consciousnessManager.ShowBiologicalLayer();
                break;
                
            case "cognitive":
                ShowCognitivePanel();
                if (consciousnessManager != null)
                    consciousnessManager.ShowCognitiveLayer();
                break;
                
            case "philosophical":
                ShowPhilosophicalPanel();
                break;
                
            default:
                Debug.LogWarning("Unknown layer name: " + layerName);
                break;
        }
    }
    
    // For VR interaction with buttons
    public void OnInfoButtonPressed(string layerName)
    {
        ShowLayerInfo(layerName);
    }
} 