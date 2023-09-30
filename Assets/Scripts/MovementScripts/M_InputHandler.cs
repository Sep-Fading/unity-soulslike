using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using Vector2 = UnityEngine.Vector2;


namespace MovementScripts
{
    public class M_InputHandler : MonoBehaviour
    {
        // Setting up the defined controls via the Input System.
        private PlayerControls playerControls;
        private InputAction moveControls;

        // Keyboard Inputs.
        public float moveAmount;
        public float horizontalInput;
        public float verticalInput;
        public bool jumpActivated;
        public Vector2 moveInput = Vector2.zero;
        
        // Mouse Inputs.
        public float mouseY; // <-- Don't really want to use vertical camera movement yet, so this is unused.
        public float mouseX;
        public InputAction mouseControls;
        public Vector2 mouseInput;
        
        [HideInInspector]
        public M_AnimationHandler animHandler;


        void OnEnable()
        {
            // Only create an instance of PlayerControls if it doesnt already exist.
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => moveInput = i.ReadValue<Vector2>();
                playerControls.PlayerMovement.Camera.performed += i => mouseInput = i.ReadValue<Vector2>();
            }
            
            playerControls.Enable();
            animHandler = GetComponentInChildren<M_AnimationHandler>();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        // Gets called every frame.
        public void Update()
        {
            HandleMovementControls();
            HandleCameraControls();
            HandleJumpButton();
        }

        // Handles input for movement binds.
        private void HandleMovementControls()
        {
            moveControls = playerControls.PlayerMovement.Movement;
            horizontalInput = moveInput.x;
            verticalInput = moveInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            // Smoothing the controls by setting boundaries.
            if (moveAmount <= 0.5f && moveAmount > 0f)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5)
            {
                moveAmount = 1f;
            }
            
            animHandler.UpdateAnimationValues(0, moveAmount);
        }
        
        // Handle input for jumping bind.
        private void HandleJumpButton()
        {
            if (playerControls.PlayerMovement.Jumping.IsPressed())
            {
                jumpActivated = true;
            }
            else
            {
                jumpActivated = false;
            }
        }
        
        // Handles input for Camera control binds.
        private void HandleCameraControls()
        {
            mouseControls = playerControls.PlayerMovement.Camera;
            mouseX = mouseInput.x;
            mouseY = mouseInput.y;
        }
    }
}