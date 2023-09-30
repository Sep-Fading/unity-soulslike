using System.Collections;
using System.Collections.Generic;
using CameraScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

namespace MovementScripts
{
    public class PlayerLocomotion : MonoBehaviour
    {
        // Player Object & Delta Time.
        [SerializeField] private GameObject playerObject;
        private float delta;

        // Locomotion fields (Settings for run speed etc...)

        [SerializeField] private float sprintSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float jumpAmount;
        [SerializeField] private float playerFallingRaycastRadius;

        private Vector3 moveDirection;
        private Vector3 projectedVelocity;
        private Vector3 jumpVector;

        // Camera & Rigid body.
        private ThirdPersonCamHandler camHandler;
        public Transform cameraObject;
        public Rigidbody player_rigidBody;
        
        // Ground layer for jump detection.
        public LayerMask groundLayer;
        public float maxDistanceCast;

        // -------------------------------------------------------

        // Calling the script file related to handling inputs.
        private M_InputHandler inputHandler;

        [HideInInspector] public Transform thisTransform;

        // Start is called before the first frame update
        private void Start()
        {
            inputHandler = GetComponent<M_InputHandler>();
            player_rigidBody = GetComponentInChildren<Rigidbody>();
            cameraObject = Camera.main.transform;
            camHandler = GetComponent<ThirdPersonCamHandler>();
            thisTransform = transform;

            jumpVector.x = 0;
            jumpVector.y = jumpAmount;
            jumpVector.z = 0;
            
            // Intialising Settings
            sprintSpeed = 6.5f;
            runSpeed = 5f;
            walkSpeed = 2f;
            rotationSpeed = 10f;
            jumpAmount = 0.1f;
            playerFallingRaycastRadius = 0.2f;

            maxDistanceCast = 1;

        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            delta = Time.deltaTime;
            HandleMovement();
            HandleRotation(delta);
            HandleJumping();
        }


        // Locomotion Physics.

        #region Movement

        private Vector3 targetPos = Vector3.zero;
        private Vector3 normalVector;

        private void HandleMovement()
        {
            moveDirection = inputHandler.verticalInput * cameraObject.forward;
            moveDirection += inputHandler.horizontalInput * cameraObject.right;

            moveDirection.Normalize();
            moveDirection.y = 0;

            // Speed of movement given to the direction. (Mostly relevant to joysticks)
            if (inputHandler.moveAmount == 0.5f)
            {
                moveDirection *= walkSpeed;
            }
            else if (inputHandler.moveAmount == 1f)
            {
                moveDirection *= runSpeed;
            }

            // Actually moving the player.
            projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            player_rigidBody.velocity = projectedVelocity;
        }


        private void HandleRotation(float delta)
        {
            Vector3 targetDir;
            targetDir = inputHandler.verticalInput * cameraObject.forward;
            targetDir += inputHandler.horizontalInput * cameraObject.right;
            

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = thisTransform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            Quaternion trSmoothed = Quaternion.Lerp(thisTransform.rotation, targetRotation, delta * rotationSpeed);

            thisTransform.rotation = trSmoothed;
        }


        private void HandleJumping()
        {
            if (inputHandler.jumpActivated)
            {
                playerObject.transform.Translate(jumpVector, playerObject.transform);
            }
        }

        private void HandleFallingAndLanding()
        {
            RaycastHit hit;
            Vector3 rayCastOrigin = playerObject.transform.position;
            bool detectedGround = Physics.SphereCast(rayCastOrigin, playerFallingRaycastRadius, 
                -Vector3.up, out hit, maxDistanceCast, groundLayer);
        }

        #endregion
    }
}