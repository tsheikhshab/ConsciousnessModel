using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
using Unity.XR.CoreUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Consciousness VR/Setup Complete Scene")]
    public static void SetupCompleteScene()
    {
        // Create root objects
        GameObject root = new GameObject("ConsciousnessVisualization");
        
        // Create material manager first
        GameObject materialManagerObj = new GameObject("MaterialManager");
        materialManagerObj.transform.SetParent(root.transform);
        ConsciousnessMaterialManager materialManager = materialManagerObj.AddComponent<ConsciousnessMaterialManager>();
        materialManager.CreateAllMaterials();
        
        // Create each layer
        GameObject ambientLayer = CreateAmbientLayer(root.transform, materialManager);
        GameObject biologicalLayer = CreateBiologicalLayer(root.transform, materialManager);
        GameObject cognitiveLayer = CreateCognitiveLayer(root.transform, materialManager);
        
        // Create UI and info panels
        GameObject uiSystem = CreateUISystem(root.transform, materialManager);
        
        // Create main visualization controller
        GameObject controllerObj = new GameObject("VisualizationController");
        controllerObj.transform.SetParent(root.transform);
        ConsciousnessVisualization controller = controllerObj.AddComponent<ConsciousnessVisualization>();
        
        // Configure controller
        controller.ambientLayer = ambientLayer;
        controller.biologicalLayer = biologicalLayer;
        controller.cognitiveLayer = cognitiveLayer;
        controller.centerPoint = cognitiveLayer.transform;
        
        // Create guided tour system
        GameObject tourSystem = CreateGuidedTourSystem(root.transform, materialManager);
        
        // Setup VR Player
        GameObject player = CreateVRPlayerRig(root.transform);
        
        // Create interactive elements
        CreateInteractiveElements(root.transform, controller, materialManager);
        
        // Create teleportation surfaces for movement
        CreateTeleportationSurfaces(root.transform, materialManager);
        
        // Create VR Controls Info Panel
        CreateVRControlsInfoPanel(root.transform, materialManager);
        
        // Add post-processing effects for enhanced glow
        SetupPostProcessing(root.transform);
        
        Debug.Log("Consciousness VR scene setup complete!");
    }
    
    private static GameObject CreateAmbientLayer(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        GameObject layer = new GameObject("AmbientConsciousnessLayer");
        layer.transform.SetParent(parent);
        
        AmbientConsciousnessLayer component = layer.AddComponent<AmbientConsciousnessLayer>();
        
        // Configure default parameters
        component.waveCount = 20;
        component.particleCount = 1000;
        component.waveAmplitude = 0.5f;
        component.waveFrequency = 1.0f;
        component.waveSpeed = 0.5f;
        component.particleSize = 0.05f;
        component.spawnRadius = 50f;
        component.waveColor = new Color(0.2f, 0.3f, 0.5f, 0.3f);
        component.particleColor = new Color(0.4f, 0.5f, 0.8f, 0.4f);
        
        // Apply materials from material manager
        if (materialManager != null)
        {
            materialManager.ApplyMaterialsToAmbientLayer(component);
        }
        
        return layer;
    }
    
    private static GameObject CreateBiologicalLayer(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        GameObject layer = new GameObject("BiologicalConsciousnessLayer");
        layer.transform.SetParent(parent);
        
        BiologicalConsciousnessLayer component = layer.AddComponent<BiologicalConsciousnessLayer>();
        
        // Configure default parameters
        component.boundaryRadius = 10f;
        component.boundaryColor = new Color(0.2f, 0.4f, 0.8f, 0.3f);
        component.ringCount = 3;
        component.ringColor = new Color(0.3f, 0.5f, 0.9f, 0.4f);
        component.nodeCount = 24;
        component.nodeSize = 0.2f;
        component.nodeColor = new Color(0.7f, 0.8f, 1.0f, 0.8f);
        component.loopCount = 3;
        component.loopRadius = 2.5f;
        component.loopColor = new Color(0.4f, 0.6f, 0.9f, 0.5f);
        component.connectionCount = 8;
        component.connectionColor = new Color(0.4f, 0.6f, 0.9f, 0.5f);
        component.rotationSpeed = 5f;
        component.pulsateSpeed = 1f;
        component.pulsateAmount = 0.1f;
        
        // Apply materials from material manager
        if (materialManager != null)
        {
            materialManager.ApplyMaterialsToBiologicalLayer(component);
        }
        
        return layer;
    }
    
    private static GameObject CreateCognitiveLayer(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        GameObject layer = new GameObject("CognitiveConsciousnessLayer");
        layer.transform.SetParent(parent);
        
        CognitiveConsciousnessLayer component = layer.AddComponent<CognitiveConsciousnessLayer>();
        
        // Configure default parameters
        component.boundaryRadius = 5f;
        component.boundaryColor = new Color(0.7f, 0.8f, 1.0f, 0.5f);
        component.coreRadius = 3f;
        component.coreColor = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        component.coreEmissionIntensity = 1.5f;
        component.wavePatternCount = 3;
        component.pointsPerPattern = 20;
        component.waveAmplitude = 0.5f;
        component.waveFrequency = 1.5f;
        component.waveWidth = 0.05f;
        component.wavePatternColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        component.conceptNodeCount = 8;
        component.conceptNodeSize = 0.25f;
        component.conceptNodeColor = new Color(1.0f, 1.0f, 1.0f, 0.9f);
        component.selfNodeSize = 0.5f;
        component.selfNodeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        component.connectionColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
        component.rotationSpeed = 10f;
        component.pulsateSpeed = 2f;
        component.pulsateAmount = 0.1f;
        
        // Apply materials from material manager
        if (materialManager != null)
        {
            materialManager.ApplyMaterialsToCognitiveLayer(component);
        }
        
        return layer;
    }
    
    private static GameObject CreateUISystem(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        // Create UI Canvas
        GameObject uiSystem = new GameObject("UISystem");
        uiSystem.transform.SetParent(parent);
        
        // Create info panel system
        GameObject infoPanelObj = new GameObject("InfoPanelSystem");
        infoPanelObj.transform.SetParent(uiSystem.transform);
        InfoPanelSystem infoPanelSystem = infoPanelObj.AddComponent<InfoPanelSystem>();
        
        // Create UI canvas for info panels
        GameObject canvasObj = new GameObject("InfoCanvas");
        canvasObj.transform.SetParent(infoPanelObj.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(500, 300);
        canvasRect.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        
        // Create panel background
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        
        if (materialManager != null && materialManager.uiPanelMaterial != null)
        {
            panelImage.material = materialManager.uiPanelMaterial;
            panelImage.color = Color.white; // Use material color
        }
        else
        {
            panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);
        }
        
        // Create title text
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Info Panel";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 40);
        titleRect.anchoredPosition = new Vector2(0, -20);
        
        // Create description text
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Description goes here...";
        descText.fontSize = 18;
        descText.alignment = TextAlignmentOptions.TopLeft;
        descText.color = Color.white;
        
        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.sizeDelta = new Vector2(-40, -100);
        descRect.anchoredPosition = new Vector2(0, -30);
        
        // Create quote text
        GameObject quoteObj = new GameObject("QuoteText");
        quoteObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI quoteText = quoteObj.AddComponent<TextMeshProUGUI>();
        quoteText.text = "\"Quote goes here...\"";
        quoteText.fontSize = 16;
        quoteText.fontStyle = FontStyles.Italic;
        quoteText.alignment = TextAlignmentOptions.Center;
        quoteText.color = new Color(0.8f, 0.9f, 1f);
        
        RectTransform quoteRect = quoteObj.GetComponent<RectTransform>();
        quoteRect.anchorMin = new Vector2(0, 0);
        quoteRect.anchorMax = new Vector2(1, 0);
        quoteRect.sizeDelta = new Vector2(0, 60);
        quoteRect.anchoredPosition = new Vector2(0, 30);
        
        // Create close button
        GameObject closeObj = new GameObject("CloseButton");
        closeObj.transform.SetParent(panelObj.transform);
        Image closeImage = closeObj.AddComponent<Image>();
        closeImage.color = new Color(0.8f, 0.2f, 0.2f);
        Button closeButton = closeObj.AddComponent<Button>();
        closeButton.targetGraphic = closeImage;
        
        RectTransform closeRect = closeObj.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(30, 30);
        closeRect.anchoredPosition = new Vector2(-15, -15);
        
        // Create button text
        GameObject closeTextObj = new GameObject("CloseText");
        closeTextObj.transform.SetParent(closeObj.transform);
        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "X";
        closeText.fontSize = 20;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = Color.white;
        
        RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        closeTextRect.anchoredPosition = Vector2.zero;
        
        // Connect components to InfoPanelSystem
        infoPanelSystem.infoCanvas = canvas;
        infoPanelSystem.titleText = titleText;
        infoPanelSystem.descriptionText = descText;
        infoPanelSystem.quoteText = quoteText;
        infoPanelSystem.closeButton = closeButton;
        infoPanelSystem.followDistance = 2f;
        
        return uiSystem;
    }
    
    private static GameObject CreateGuidedTourSystem(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        GameObject tourSystem = new GameObject("GuidedTourSystem");
        tourSystem.transform.SetParent(parent);
        
        GuidedTourManager tourManager = tourSystem.AddComponent<GuidedTourManager>();
        
        // Create tour canvas
        GameObject canvasObj = new GameObject("TourCanvas");
        canvasObj.transform.SetParent(tourSystem.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(600, 400);
        canvasRect.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        
        // Create tour panel
        GameObject panelObj = new GameObject("TourPanel");
        panelObj.transform.SetParent(canvasObj.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.localPosition = Vector3.zero;
        
        // Create title text
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Guided Tour";
        titleText.fontSize = 28;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 50);
        titleRect.anchoredPosition = new Vector2(0, -25);
        
        // Create narration text
        GameObject narrationObj = new GameObject("NarrationText");
        narrationObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI narrationText = narrationObj.AddComponent<TextMeshProUGUI>();
        narrationText.text = "Welcome to the guided tour of consciousness amplification...";
        narrationText.fontSize = 20;
        narrationText.alignment = TextAlignmentOptions.TopLeft;
        narrationText.color = Color.white;
        
        RectTransform narrationRect = narrationObj.GetComponent<RectTransform>();
        narrationRect.anchorMin = new Vector2(0, 0.2f);
        narrationRect.anchorMax = new Vector2(1, 0.9f);
        narrationRect.sizeDelta = new Vector2(-40, 0);
        narrationRect.anchoredPosition = new Vector2(0, 0);
        
        // Create navigation buttons
        CreateTourButton(panelObj.transform, "PrevButton", "Previous", new Vector2(0, 0), new Vector2(0.3f, 0.1f), new Vector2(80, 40), new Vector2(120, 20));
        CreateTourButton(panelObj.transform, "NextButton", "Next", new Vector2(0.7f, 0), new Vector2(1, 0.1f), new Vector2(80, 40), new Vector2(120, 20));
        CreateTourButton(panelObj.transform, "ExitButton", "Exit Tour", new Vector2(0.35f, 0), new Vector2(0.65f, 0.1f), new Vector2(100, 40), new Vector2(150, 20));
        
        // Connect components to GuidedTourManager
        tourManager.tourCanvas = canvas;
        tourManager.titleText = titleText;
        tourManager.narrationText = narrationText;
        
        // Find and assign buttons
        tourManager.prevButton = panelObj.transform.Find("PrevButton").GetComponent<Button>();
        tourManager.nextButton = panelObj.transform.Find("NextButton").GetComponent<Button>();
        tourManager.exitTourButton = panelObj.transform.Find("ExitButton").GetComponent<Button>();
        
        // Create start tour button (outside the tour panel)
        GameObject startButtonObj = new GameObject("StartTourButton");
        startButtonObj.transform.SetParent(canvasObj.transform);
        Image startButtonImage = startButtonObj.AddComponent<Image>();
        startButtonImage.color = new Color(0.2f, 0.4f, 0.8f);
        Button startButton = startButtonObj.AddComponent<Button>();
        startButton.targetGraphic = startButtonImage;
        
        RectTransform startButtonRect = startButtonObj.GetComponent<RectTransform>();
        startButtonRect.anchorMin = new Vector2(0.5f, 0);
        startButtonRect.anchorMax = new Vector2(0.5f, 0);
        startButtonRect.sizeDelta = new Vector2(200, 60);
        startButtonRect.anchoredPosition = new Vector2(0, 50);
        
        GameObject startTextObj = new GameObject("StartText");
        startTextObj.transform.SetParent(startButtonObj.transform);
        TextMeshProUGUI startText = startTextObj.AddComponent<TextMeshProUGUI>();
        startText.text = "Start Guided Tour";
        startText.fontSize = 22;
        startText.alignment = TextAlignmentOptions.Center;
        startText.color = Color.white;
        
        RectTransform startTextRect = startTextObj.GetComponent<RectTransform>();
        startTextRect.anchorMin = Vector2.zero;
        startTextRect.anchorMax = Vector2.one;
        startTextRect.sizeDelta = Vector2.zero;
        startTextRect.anchoredPosition = Vector2.zero;
        
        tourManager.startTourButton = startButton;
        
        // Configure default tour settings
        tourManager.transitionSpeed = 2f;
        tourManager.autoStart = false;
        tourManager.allowSkipping = true;
        tourManager.loopOnComplete = false;
        
        return tourSystem;
    }
    
    private static void CreateTourButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.4f, 0.8f);
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = position;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 20;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
    }
    
    private static GameObject CreateVRPlayerRig(Transform parent)
    {
        // Create VR Player Rig
        GameObject playerRig = new GameObject("XR Origin");
        playerRig.transform.SetParent(parent);
        
        // Add XR Origin component
        XROrigin xrOrigin = playerRig.AddComponent<XROrigin>();
        
        // Find and add a camera offset
        GameObject cameraOffset = new GameObject("Camera Offset");
        cameraOffset.transform.SetParent(playerRig.transform);
        
        // Add a main camera for VR
        GameObject mainCamera = new GameObject("Main Camera");
        mainCamera.transform.SetParent(cameraOffset.transform);
        mainCamera.tag = "MainCamera";
        Camera camera = mainCamera.AddComponent<Camera>();
        mainCamera.AddComponent<AudioListener>();
        
        // Set up the XR Origin references
        xrOrigin.Camera = camera;
        xrOrigin.CameraFloorOffsetObject = cameraOffset;
        
        // Add XR Ray Interactors for the left and right hands
        GameObject leftHand = new GameObject("LeftHand Controller");
        leftHand.transform.SetParent(cameraOffset.transform);
        
        GameObject rightHand = new GameObject("RightHand Controller");
        rightHand.transform.SetParent(cameraOffset.transform);
        
        // Add tracking for hand controllers
        XRController leftController = leftHand.AddComponent<XRController>();
        leftController.controllerNode = UnityEngine.XR.XRNode.LeftHand;
        
        XRController rightController = rightHand.AddComponent<XRController>();
        rightController.controllerNode = UnityEngine.XR.XRNode.RightHand;
        
        // Add Ray Interactors for teleportation
        XRRayInteractor leftRayInteractor = leftHand.AddComponent<XRRayInteractor>();
        XRRayInteractor rightRayInteractor = rightHand.AddComponent<XRRayInteractor>();
        
        // Set up teleportation system
        GameObject teleportationController = new GameObject("Teleportation System");
        teleportationController.transform.SetParent(playerRig.transform);
        TeleportationProvider teleportProvider = teleportationController.AddComponent<TeleportationProvider>();
        
        // Create a Character Controller for movement
        CharacterController characterController = playerRig.AddComponent<CharacterController>();
        characterController.height = 1.8f; // Default height
        characterController.radius = 0.3f;
        characterController.center = new Vector3(0, 0.9f, 0);
        
        // Add VR Locomotion Manager
        VRLocomotionManager locomotionManager = playerRig.AddComponent<VRLocomotionManager>();
        
        // Configure locomotion manager
        locomotionManager.SetMoveSpeed(2.0f);
        
        // Position the player at a good starting point
        playerRig.transform.position = new Vector3(0, 0, -20f);
        
        return playerRig;
    }
    
    private static void CreateTeleportationSurfaces(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        // Create main ground teleport surface
        GameObject teleportGround = GameObject.CreatePrimitive(PrimitiveType.Plane);
        teleportGround.name = "TeleportGround";
        teleportGround.transform.SetParent(parent);
        teleportGround.transform.position = new Vector3(0, -0.5f, 0);
        teleportGround.transform.localScale = new Vector3(10, 1, 10);
        
        // Add teleportation area component
        TeleportationArea teleportArea = teleportGround.AddComponent<TeleportationArea>();
        
        // Set material to a teleport surface material
        Renderer renderer = teleportGround.GetComponent<Renderer>();
        
        if (materialManager != null && materialManager.teleportSurfaceMaterial != null)
        {
            renderer.material = materialManager.teleportSurfaceMaterial;
        }
        else
        {
            renderer.material.color = new Color(0.1f, 0.1f, 0.2f, 0.5f);
            
            // Make it slightly transparent
            renderer.material.SetFloat("_Mode", 3); // Transparent mode
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            renderer.material.SetInt("_ZWrite", 0);
            renderer.material.DisableKeyword("_ALPHATEST_ON");
            renderer.material.EnableKeyword("_ALPHABLEND_ON");
            renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = 3000;
        }
        
        // Create viewing platforms at different heights for different perspectives
        CreateViewingPlatform(parent, new Vector3(15, 5, 0), "AmbientViewPlatform", materialManager?.ambientPlatformMaterial);
        CreateViewingPlatform(parent, new Vector3(0, 10, 15), "BiologicalViewPlatform", materialManager?.biologicalPlatformMaterial);
        CreateViewingPlatform(parent, new Vector3(-10, 15, 0), "CognitiveViewPlatform", materialManager?.cognitivePlatformMaterial);
    }
    
    private static void CreateViewingPlatform(Transform parent, Vector3 position, string name, Material material)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = name;
        platform.transform.SetParent(parent);
        platform.transform.position = position;
        platform.transform.localScale = new Vector3(3, 0.2f, 3);
        
        // Add teleportation area component
        TeleportationArea teleportArea = platform.AddComponent<TeleportationArea>();
        
        // Set material
        Renderer renderer = platform.GetComponent<Renderer>();
        
        if (material != null)
        {
            renderer.material = material;
        }
        else
        {
            renderer.material.color = new Color(0.2f, 0.3f, 0.5f, 0.7f);
            
            // Make it slightly transparent
            renderer.material.SetFloat("_Mode", 3); // Transparent mode
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            renderer.material.SetInt("_ZWrite", 0);
            renderer.material.DisableKeyword("_ALPHATEST_ON");
            renderer.material.EnableKeyword("_ALPHABLEND_ON");
            renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = 3000;
        }
    }
    
    private static void CreateInteractiveElements(Transform parent, ConsciousnessVisualization controller, ConsciousnessMaterialManager materialManager)
    {
        // Create a few interactive elements for each layer
        CreateInteractiveElement(parent, "AmbientNode", 
            InteractiveElement.ElementLayer.Ambient, 
            "Ambient Consciousness", 
            "This represents the baseline consciousness that exists throughout the universe.",
            new Vector3(15f, 0f, 0f),
            new Vector3(1f, 1f, 1f),
            controller,
            materialManager?.ambientParticleMaterial);
            
        CreateInteractiveElement(parent, "BiologicalNode", 
            InteractiveElement.ElementLayer.Biological, 
            "Biological Amplification", 
            "The first threshold of consciousness amplification occurs in living systems through autocatalytic closure.",
            new Vector3(8f, 3f, 0f),
            new Vector3(0.8f, 0.8f, 0.8f),
            controller,
            materialManager?.biologicalNodeMaterial);
            
        CreateInteractiveElement(parent, "CognitiveNode", 
            InteractiveElement.ElementLayer.Cognitive, 
            "Cognitive Amplification", 
            "The second threshold of amplification through integrated worldviews and conceptual closure.",
            new Vector3(3f, -2f, 0f),
            new Vector3(0.6f, 0.6f, 0.6f),
            controller,
            materialManager?.cognitiveNodeMaterial);
            
        CreateInteractiveElement(parent, "PhilosophicalNode", 
            InteractiveElement.ElementLayer.Philosophical, 
            "Philosophical Implications", 
            "This theory bridges panpsychism and emergentism by showing how consciousness exists at different amplification levels.",
            new Vector3(-5f, 0f, 5f),
            new Vector3(0.7f, 0.7f, 0.7f),
            controller,
            materialManager?.cognitiveCoreMaterial);
    }
    
    private static void CreateInteractiveElement(Transform parent, string name, 
        InteractiveElement.ElementLayer layer, string elementName, string description,
        Vector3 position, Vector3 scale, ConsciousnessVisualization controller, Material material)
    {
        GameObject element = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        element.name = name;
        element.transform.SetParent(parent);
        element.transform.localPosition = position;
        element.transform.localScale = scale;
        
        // Add XR Simple Interactable
        XRSimpleInteractable interactable = element.AddComponent<XRSimpleInteractable>();
        
        // Add Interactive Element component
        InteractiveElement interactiveComponent = element.AddComponent<InteractiveElement>();
        interactiveComponent.layer = layer;
        interactiveComponent.elementName = elementName;
        interactiveComponent.elementDescription = description;
        interactiveComponent.interactable = interactable;
        interactiveComponent.highlightOnHover = true;
        interactiveComponent.rotateOnHover = true;
        
        // Set material properties
        Renderer renderer = element.GetComponent<Renderer>();
        
        if (material != null)
        {
            renderer.material = material;
            
            // Create hover and selected variants
            Material hoveredMaterial = new Material(material);
            hoveredMaterial.color = new Color(
                material.color.r * 1.3f,
                material.color.g * 1.3f,
                material.color.b * 1.3f,
                material.color.a
            );
            
            Material selectedMaterial = new Material(material);
            selectedMaterial.color = new Color(
                material.color.r * 1.5f,
                material.color.g * 1.5f,
                material.color.b * 1.5f,
                material.color.a
            );
            
            // Set emission for glow
            Color emissionColor = new Color(
                Mathf.Clamp01(material.color.r * 2f),
                Mathf.Clamp01(material.color.g * 2f),
                Mathf.Clamp01(material.color.b * 2f),
                material.color.a
            );
            
            hoveredMaterial.EnableKeyword("_EMISSION");
            hoveredMaterial.SetColor("_EmissionColor", emissionColor * 1.5f);
            
            selectedMaterial.EnableKeyword("_EMISSION");
            selectedMaterial.SetColor("_EmissionColor", emissionColor * 2f);
            
            interactiveComponent.defaultMaterial = material;
            interactiveComponent.hoveredMaterial = hoveredMaterial;
            interactiveComponent.selectedMaterial = selectedMaterial;
        }
        else
        {
            // Set color based on layer if no material provided
            switch (layer)
            {
                case InteractiveElement.ElementLayer.Ambient:
                    renderer.material.color = new Color(0.2f, 0.3f, 0.5f, 0.8f);
                    break;
                case InteractiveElement.ElementLayer.Biological:
                    renderer.material.color = new Color(0.3f, 0.5f, 0.8f, 0.8f);
                    break;
                case InteractiveElement.ElementLayer.Cognitive:
                    renderer.material.color = new Color(0.7f, 0.8f, 1.0f, 0.8f);
                    break;
                case InteractiveElement.ElementLayer.Philosophical:
                    renderer.material.color = new Color(0.5f, 0.7f, 0.9f, 0.8f);
                    break;
            }
        }
    }
    
    private static void SetupPostProcessing(Transform parent)
    {
        // Add a post-processing volume for bloom and glow effects
        GameObject postProcessingObj = new GameObject("PostProcessingVolume");
        postProcessingObj.transform.SetParent(parent);
        
        // Note: In a real implementation, this would add a post-processing volume component
        // and configure it for bloom effects, but that would depend on the post-processing
        // stack being used (URP, HDRP, or Post-processing v2)
        
        // Example of parameters that would be set:
        // - Bloom intensity: 1.5
        // - Bloom threshold: 0.8
        // - Bloom diffusion: 6
        // - Bloom color: slight blue tint
    }
    
    private static void CreateVRControlsInfoPanel(Transform parent, ConsciousnessMaterialManager materialManager)
    {
        GameObject controlsInfoObj = new GameObject("VR Controls Info");
        controlsInfoObj.transform.SetParent(parent);
        VRControlsInfoPanel controlsInfoPanel = controlsInfoObj.AddComponent<VRControlsInfoPanel>();
        
        // Create UI canvas
        GameObject canvasObj = new GameObject("ControlsCanvas");
        canvasObj.transform.SetParent(controlsInfoObj.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.AddComponent<CanvasGroup>();
        
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(400, 300);
        canvasRect.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        
        // Create panel background
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.3f, 0.8f);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        // Create title text
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "VR CONTROLS";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.9f, 0.9f, 1.0f);
        titleText.fontStyle = FontStyles.Bold;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 40);
        titleRect.anchoredPosition = new Vector2(0, -20);
        
        // Create instructions text
        GameObject instructionsObj = new GameObject("InstructionsText");
        instructionsObj.transform.SetParent(panelObj.transform);
        TextMeshProUGUI instructionsText = instructionsObj.AddComponent<TextMeshProUGUI>();
        instructionsText.text = 
            "• <b>MOVEMENT</b>: Use joystick to move\n" +
            "• <b>TURNING</b>: Use right joystick to turn\n" +
            "• <b>TELEPORT</b>: Hold grip, point & release\n" +
            "• <b>INTERACT</b>: Point and trigger\n\n" +
            "Teleport to viewing platforms for different perspectives.\n" +
            "Interact with sphere nodes to learn about each consciousness layer.";
        instructionsText.fontSize = 16;
        instructionsText.alignment = TextAlignmentOptions.Left;
        instructionsText.color = Color.white;
        
        RectTransform instructionsRect = instructionsObj.GetComponent<RectTransform>();
        instructionsRect.anchorMin = new Vector2(0, 0.2f);
        instructionsRect.anchorMax = new Vector2(1, 0.9f);
        instructionsRect.sizeDelta = new Vector2(-40, 0);
        instructionsRect.anchoredPosition = new Vector2(0, 0);
        
        // Create close button
        GameObject closeObj = new GameObject("CloseButton");
        closeObj.transform.SetParent(panelObj.transform);
        Image closeImage = closeObj.AddComponent<Image>();
        closeImage.color = new Color(0.7f, 0.2f, 0.2f, 0.8f);
        Button closeButton = closeObj.AddComponent<Button>();
        closeButton.targetGraphic = closeImage;
        
        RectTransform closeRect = closeObj.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.5f, 0);
        closeRect.anchorMax = new Vector2(0.5f, 0);
        closeRect.sizeDelta = new Vector2(100, 30);
        closeRect.anchoredPosition = new Vector2(0, 30);
        
        // Create button text
        GameObject closeTextObj = new GameObject("CloseText");
        closeTextObj.transform.SetParent(closeObj.transform);
        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "CLOSE";
        closeText.fontSize = 16;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = Color.white;
        
        RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        
        // Connect to the controller
        controlsInfoPanel.infoCanvas = canvas;
        controlsInfoPanel.closeButton = closeButton;
        controlsInfoPanel.showOnStart = true;
        controlsInfoPanel.displayTime = 15f;
    }
#endif
} 