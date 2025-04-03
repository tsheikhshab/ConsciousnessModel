using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class GuidedTourManager : MonoBehaviour
{
    [System.Serializable]
    public class TourStep
    {
        public string title;
        [TextArea(3, 10)]
        public string narration;
        public AudioClip narrationAudio;
        public float duration = 15f;
        public Transform cameraPosition;
        public string layerToShow;
        public bool pauseForInteraction = false;
    }
    
    [Header("Tour Configuration")]
    public TourStep[] tourSteps;
    public float transitionSpeed = 2f;
    public bool autoStart = false;
    public bool allowSkipping = true;
    public bool loopOnComplete = false;
    
    [Header("UI References")]
    public Canvas tourCanvas;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI narrationText;
    public Button nextButton;
    public Button prevButton;
    public Button skipButton;
    public Button startTourButton;
    public Button exitTourButton;
    public Slider progressSlider;
    
    [Header("Audio")]
    public AudioSource narrationAudioSource;
    public AudioClip transitionSound;
    
    [Header("Camera Control")]
    public Transform cameraRig;
    public bool smoothCameraTransition = true;
    
    [Header("Components")]
    public ConsciousnessVisualization visualizationManager;
    public InfoPanelSystem infoPanelSystem;
    
    private int currentStepIndex = -1;
    private bool isTourActive = false;
    private bool isTransitioning = false;
    private Coroutine tourCoroutine;
    private Coroutine transitionCoroutine;
    
    private void Start()
    {
        // Initialize UI
        SetupUI();
        
        // Find required components if not set
        if (visualizationManager == null)
            visualizationManager = FindObjectOfType<ConsciousnessVisualization>();
            
        if (infoPanelSystem == null)
            infoPanelSystem = FindObjectOfType<InfoPanelSystem>();
            
        if (narrationAudioSource == null)
            narrationAudioSource = GetComponent<AudioSource>();
        
        // Start tour if auto-start is enabled
        if (autoStart)
        {
            StartTour();
        }
        else
        {
            // Hide tour UI
            SetTourUIActive(false);
        }
    }
    
    private void SetupUI()
    {
        if (nextButton)
            nextButton.onClick.AddListener(GoToNextStep);
            
        if (prevButton)
            prevButton.onClick.AddListener(GoToPreviousStep);
            
        if (skipButton)
            skipButton.onClick.AddListener(EndTour);
            
        if (startTourButton)
            startTourButton.onClick.AddListener(StartTour);
            
        if (exitTourButton)
            exitTourButton.onClick.AddListener(EndTour);
            
        if (progressSlider)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = tourSteps != null ? tourSteps.Length - 1 : 0;
            progressSlider.value = 0;
        }
        
        // Initially hide tour UI
        SetTourUIActive(false);
    }
    
    public void StartTour()
    {
        if (isTourActive || tourSteps == null || tourSteps.Length == 0)
            return;
            
        isTourActive = true;
        currentStepIndex = -1;
        
        // Show tour UI
        SetTourUIActive(true);
        
        // Start tour coroutine
        GoToNextStep();
    }
    
    public void EndTour()
    {
        if (!isTourActive)
            return;
            
        isTourActive = false;
        
        // Stop any active coroutines
        if (tourCoroutine != null)
            StopCoroutine(tourCoroutine);
            
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
            
        // Hide tour UI
        SetTourUIActive(false);
        
        // Stop narration audio
        if (narrationAudioSource != null && narrationAudioSource.isPlaying)
            narrationAudioSource.Stop();
    }
    
    public void GoToNextStep()
    {
        if (!isTourActive || isTransitioning)
            return;
            
        int nextIndex = currentStepIndex + 1;
        
        // Check if we've reached the end
        if (nextIndex >= tourSteps.Length)
        {
            if (loopOnComplete)
                nextIndex = 0;
            else
            {
                EndTour();
                return;
            }
        }
        
        TransitionToStep(nextIndex);
    }
    
    public void GoToPreviousStep()
    {
        if (!isTourActive || isTransitioning || currentStepIndex <= 0)
            return;
            
        TransitionToStep(currentStepIndex - 1);
    }
    
    private void TransitionToStep(int stepIndex)
    {
        if (isTransitioning || stepIndex < 0 || stepIndex >= tourSteps.Length)
            return;
            
        isTransitioning = true;
        
        // Stop any active coroutines
        if (tourCoroutine != null)
            StopCoroutine(tourCoroutine);
            
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
            
        // Start transition to next step
        transitionCoroutine = StartCoroutine(TransitionStepCoroutine(stepIndex));
    }
    
    private IEnumerator TransitionStepCoroutine(int stepIndex)
    {
        // Play transition sound
        if (narrationAudioSource != null && transitionSound != null)
        {
            narrationAudioSource.Stop();
            narrationAudioSource.clip = transitionSound;
            narrationAudioSource.Play();
        }
        
        // Fade out current UI
        if (tourCanvas != null)
        {
            // Fade out text - could be implemented with a CanvasGroup
            StartCoroutine(FadeText(titleText, false, 0.5f));
            StartCoroutine(FadeText(narrationText, false, 0.5f));
        }
        
        // Move to next step's position
        TourStep nextStep = tourSteps[stepIndex];
        if (nextStep.cameraPosition != null && cameraRig != null)
        {
            if (smoothCameraTransition)
            {
                float elapsedTime = 0f;
                Vector3 startPosition = cameraRig.position;
                Quaternion startRotation = cameraRig.rotation;
                
                while (elapsedTime < 1f)
                {
                    elapsedTime += Time.deltaTime * transitionSpeed;
                    float t = Mathf.SmoothStep(0f, 1f, elapsedTime);
                    
                    cameraRig.position = Vector3.Lerp(startPosition, nextStep.cameraPosition.position, t);
                    cameraRig.rotation = Quaternion.Slerp(startRotation, nextStep.cameraPosition.rotation, t);
                    
                    yield return null;
                }
                
                // Ensure we reach the exact target
                cameraRig.position = nextStep.cameraPosition.position;
                cameraRig.rotation = nextStep.cameraPosition.rotation;
            }
            else
            {
                // Instant transition
                cameraRig.position = nextStep.cameraPosition.position;
                cameraRig.rotation = nextStep.cameraPosition.rotation;
            }
        }
        
        // Show the appropriate layer
        if (visualizationManager != null && !string.IsNullOrEmpty(nextStep.layerToShow))
        {
            switch (nextStep.layerToShow.ToLower())
            {
                case "ambient":
                    visualizationManager.ShowAmbientLayer();
                    break;
                case "biological":
                    visualizationManager.ShowBiologicalLayer();
                    break;
                case "cognitive":
                    visualizationManager.ShowCognitiveLayer();
                    break;
                case "all":
                    visualizationManager.ShowAllLayers();
                    break;
            }
        }
        
        // Update current step index
        currentStepIndex = stepIndex;
        
        // Update progress slider
        if (progressSlider != null)
            progressSlider.value = currentStepIndex;
        
        // Short pause before showing the UI again
        yield return new WaitForSeconds(0.5f);
        
        // Update UI with new step content
        if (titleText != null)
            titleText.text = nextStep.title;
            
        if (narrationText != null)
            narrationText.text = nextStep.narration;
        
        // Fade in UI
        StartCoroutine(FadeText(titleText, true, 0.5f));
        StartCoroutine(FadeText(narrationText, true, 0.5f));
        
        // Play narration audio if available
        if (narrationAudioSource != null && nextStep.narrationAudio != null)
        {
            narrationAudioSource.clip = nextStep.narrationAudio;
            narrationAudioSource.Play();
        }
        
        // Start timer for the step
        isTransitioning = false;
        tourCoroutine = StartCoroutine(StepTimerCoroutine(nextStep));
    }
    
    private IEnumerator StepTimerCoroutine(TourStep step)
    {
        // If we need to wait for user interaction
        if (step.pauseForInteraction)
        {
            // Wait until user presses the next button
            yield break;
        }
        
        // Otherwise, automatically proceed after the specified duration
        yield return new WaitForSeconds(step.duration);
        
        // Move to next step if tour is still active
        if (isTourActive && !isTransitioning)
        {
            GoToNextStep();
        }
    }
    
    private IEnumerator FadeText(TextMeshProUGUI text, bool fadeIn, float duration)
    {
        if (text == null) yield break;
        
        float elapsedTime = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float targetAlpha = fadeIn ? 1f : 0f;
        
        Color currentColor = text.color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            text.color = currentColor;
            
            yield return null;
        }
        
        // Ensure we reach the target alpha
        currentColor.a = targetAlpha;
        text.color = currentColor;
    }
    
    private void SetTourUIActive(bool active)
    {
        if (tourCanvas != null)
            tourCanvas.gameObject.SetActive(active);
            
        if (startTourButton != null)
            startTourButton.gameObject.SetActive(!active);
    }
    
    // For external triggering (e.g., voice commands)
    public void ActivateSpecificTourStep(int stepIndex)
    {
        if (!isTourActive)
            StartTour();
            
        if (stepIndex >= 0 && stepIndex < tourSteps.Length)
            TransitionToStep(stepIndex);
    }
    
    public void ToggleTour()
    {
        if (isTourActive)
            EndTour();
        else
            StartTour();
    }
} 