using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(CharacterController))]
public class VRLocomotionManager : MonoBehaviour
{
    [Header("Movement References")]
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform headTransform; // Usually the camera
    
    [Header("Teleportation")]
    [SerializeField] private XRRayInteractor leftRayInteractor;
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private TeleportationProvider teleportationProvider;
    [SerializeField] private InputActionProperty leftActivate;
    [SerializeField] private InputActionProperty rightActivate;
    [SerializeField] private InputActionProperty leftCancel;
    [SerializeField] private InputActionProperty rightCancel;
    
    [Header("Continuous Movement")]
    [SerializeField] private InputActionProperty moveAction;
    [SerializeField] private InputActionProperty turnAction;
    [SerializeField] private bool enableContinuousMovement = true;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float turnSpeed = 60.0f;
    [SerializeField] private bool snapTurning = true;
    [SerializeField] private float snapTurnAngle = 30f;
    private bool isSnapping = false;
    
    [Header("Gravity")]
    [SerializeField] private bool applyGravity = true;
    [SerializeField] private float gravityMultiplier = 1.0f;
    [SerializeField] private float additionalHeight = 0.2f; // Extra height to prevent "stepping" on uneven ground
    private float fallingSpeed;
    
    private bool leftTeleportActive;
    private bool rightTeleportActive;
    
    private void Awake()
    {
        // Get required components
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        
        if (xrOrigin == null)
            xrOrigin = GetComponent<XROrigin>();
            
        if (headTransform == null && xrOrigin != null)
            headTransform = xrOrigin.Camera.transform;
    }
    
    private void OnEnable()
    {
        // Setup teleport actions
        if (leftActivate != null)
            leftActivate.action.performed += OnLeftTeleportActivate;
        if (rightActivate != null)
            rightActivate.action.performed += OnRightTeleportActivate;
        if (leftCancel != null)
            leftCancel.action.performed += OnLeftTeleportCancel;
        if (rightCancel != null)
            rightCancel.action.performed += OnRightTeleportCancel;
            
        // Enable input actions
        EnableActions();
    }
    
    private void OnDisable()
    {
        // Remove teleport actions
        if (leftActivate != null)
            leftActivate.action.performed -= OnLeftTeleportActivate;
        if (rightActivate != null)
            rightActivate.action.performed -= OnRightTeleportActivate;
        if (leftCancel != null)
            leftCancel.action.performed -= OnLeftTeleportCancel;
        if (rightCancel != null)
            rightCancel.action.performed -= OnRightTeleportCancel;
            
        // Disable input actions
        DisableActions();
    }
    
    private void EnableActions()
    {
        leftActivate.action?.Enable();
        rightActivate.action?.Enable();
        leftCancel.action?.Enable();
        rightCancel.action?.Enable();
        moveAction.action?.Enable();
        turnAction.action?.Enable();
    }
    
    private void DisableActions()
    {
        leftActivate.action?.Disable();
        rightActivate.action?.Disable();
        leftCancel.action?.Disable();
        rightCancel.action?.Disable();
        moveAction.action?.Disable();
        turnAction.action?.Disable();
    }
    
    private void Update()
    {
        if (enableContinuousMovement)
        {
            HandleContinuousMovement();
            HandleContinuousTurn();
        }
        
        HandleTeleportationRayUpdate();
        
        if (applyGravity)
            ApplyGravity();
        
        // Update the character height based on the XR Rig's height
        UpdateCharacterHeight();
    }
    
    private void HandleContinuousMovement()
    {
        if (moveAction.action == null) return;
        
        Vector2 inputAxis = moveAction.action.ReadValue<Vector2>();
        
        // Get movement orientation from head
        Vector3 moveDirection = headTransform.forward * inputAxis.y + headTransform.right * inputAxis.x;
        moveDirection.y = 0f; // Keep movement flat on XZ plane
        
        // Apply movement
        if (moveDirection != Vector3.zero)
        {
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
    
    private void HandleContinuousTurn()
    {
        if (turnAction.action == null) return;
        
        float turnValue = turnAction.action.ReadValue<Vector2>().x;
        
        if (Mathf.Abs(turnValue) > 0.1f)
        {
            if (snapTurning)
            {
                if (!isSnapping)
                {
                    isSnapping = true;
                    float snapAngle = snapTurnAngle * Mathf.Sign(turnValue);
                    transform.Rotate(Vector3.up, snapAngle);
                }
            }
            else
            {
                // Smooth turning
                transform.Rotate(Vector3.up, turnValue * turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            isSnapping = false;
        }
    }
    
    private void ApplyGravity()
    {
        // Simple gravity implementation
        if (characterController.isGrounded)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            characterController.Move(Vector3.up * fallingSpeed * Time.deltaTime);
        }
    }
    
    private void UpdateCharacterHeight()
    {
        if (xrOrigin == null) return;
        
        // Get the actual user height
        float headHeight = Mathf.Clamp(headTransform.localPosition.y, 0.2f, 2f);
        
        // Update character controller height and center
        characterController.height = headHeight + additionalHeight;
        Vector3 center = Vector3.zero;
        center.y = characterController.height / 2;
        characterController.center = center;
    }
    
    // Teleportation methods
    private void OnLeftTeleportActivate(InputAction.CallbackContext context)
    {
        if (leftRayInteractor != null)
        {
            leftTeleportActive = true;
            leftRayInteractor.gameObject.SetActive(true);
        }
    }
    
    private void OnRightTeleportActivate(InputAction.CallbackContext context)
    {
        if (rightRayInteractor != null)
        {
            rightTeleportActive = true;
            rightRayInteractor.gameObject.SetActive(true);
        }
    }
    
    private void OnLeftTeleportCancel(InputAction.CallbackContext context)
    {
        if (leftRayInteractor != null)
        {
            leftTeleportActive = false;
            leftRayInteractor.gameObject.SetActive(false);
            TryTeleport(leftRayInteractor);
        }
    }
    
    private void OnRightTeleportCancel(InputAction.CallbackContext context)
    {
        if (rightRayInteractor != null)
        {
            rightTeleportActive = false;
            rightRayInteractor.gameObject.SetActive(false);
            TryTeleport(rightRayInteractor);
        }
    }
    
    private void HandleTeleportationRayUpdate()
    {
        // You might want to update ray visualizations here
        // or add custom teleport visualization effects
    }
    
    private void TryTeleport(XRRayInteractor rayInteractor)
    {
        if (teleportationProvider != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Check if hit a valid teleport area
            if (IsValidTeleportDestination(hit.point))
            {
                TeleportRequest request = new TeleportRequest
                {
                    destinationPosition = hit.point,
                    destinationRotation = transform.rotation, // Keep current rotation or calculate from hit normal
                    matchOrientation = MatchOrientation.None, // Or other options
                    requestTime = Time.time
                };
                
                teleportationProvider.QueueTeleportRequest(request);
            }
        }
    }
    
    private bool IsValidTeleportDestination(Vector3 position)
    {
        // Check if it's a valid teleport surface/location
        // This can be extended based on your specific requirements
        
        // Example: Check if the position is on a designated teleport area or within bounds
        return true; // For now, all positions are valid
    }
    
    // Public methods to control movement options
    public void SetContinuousMovement(bool enable)
    {
        enableContinuousMovement = enable;
    }
    
    public void SetSnapTurning(bool enable)
    {
        snapTurning = enable;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0.1f, speed);
    }
} 