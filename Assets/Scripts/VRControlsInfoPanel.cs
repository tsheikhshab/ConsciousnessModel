using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRControlsInfoPanel : MonoBehaviour
{
    [Header("UI References")]
    public Canvas infoCanvas;
    public Button closeButton;
    
    [Header("Display Settings")]
    public float displayTime = 10f;
    public float fadeOutDuration = 1f;
    public bool showOnStart = true;
    public bool followCamera = true;
    public float followDistance = 1.5f;
    public Vector3 followOffset = new Vector3(0f, -0.3f, 0f);
    
    private Transform cameraTransform;
    private bool isShowing = false;
    private Coroutine autoHideCoroutine;
    private Coroutine followCoroutine;
    
    private void Start()
    {
        // Find camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }
        
        // Initialize hidden
        if (infoCanvas != null)
        {
            infoCanvas.gameObject.SetActive(false);
        }
        
        // Show on start if configured
        if (showOnStart)
        {
            Invoke("ShowPanel", 2f); // Delay a bit for the scene to load
        }
    }
    
    public void ShowPanel()
    {
        if (isShowing || infoCanvas == null) return;
        
        isShowing = true;
        infoCanvas.gameObject.SetActive(true);
        
        // Position in front of camera
        PositionInFrontOfCamera();
        
        // Start following if needed
        if (followCamera && cameraTransform != null)
        {
            if (followCoroutine != null)
            {
                StopCoroutine(followCoroutine);
            }
            followCoroutine = StartCoroutine(FollowCameraCoroutine());
        }
        
        // Start auto-hide timer
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
        }
        autoHideCoroutine = StartCoroutine(AutoHideCoroutine());
    }
    
    public void HidePanel()
    {
        if (!isShowing || infoCanvas == null) return;
        
        StartCoroutine(FadeOutPanel());
    }
    
    private IEnumerator FadeOutPanel()
    {
        // Get canvas group or add one if missing
        CanvasGroup canvasGroup = infoCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = infoCanvas.gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 1f;
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }
        
        // Stop following
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }
        
        infoCanvas.gameObject.SetActive(false);
        isShowing = false;
    }
    
    private IEnumerator AutoHideCoroutine()
    {
        yield return new WaitForSeconds(displayTime);
        HidePanel();
    }
    
    private IEnumerator FollowCameraCoroutine()
    {
        while (isShowing && cameraTransform != null)
        {
            PositionInFrontOfCamera();
            yield return null;
        }
    }
    
    private void PositionInFrontOfCamera()
    {
        if (cameraTransform == null) return;
        
        // Position the canvas in front of the camera
        Vector3 position = cameraTransform.position + cameraTransform.forward * followDistance;
        position += cameraTransform.up * followOffset.y;
        position += cameraTransform.right * followOffset.x;
        position += cameraTransform.forward * followOffset.z;
        
        transform.position = position;
        
        // Make it face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
    
    // Can be called by voice commands or other triggers
    public void TogglePanel()
    {
        if (isShowing)
            HidePanel();
        else
            ShowPanel();
    }
} 