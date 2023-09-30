using System;
using System.Collections;
using System.Collections.Generic;
using MovementScripts;
using UnityEngine;


namespace CameraScripts
{
    public class ThirdPersonCamHandler : MonoBehaviour
    {
        /*
         * Camera's settings:
         * followTarget requires a game object to be specified for the camera to follow.
         * camRotSpeed is a float determining the camera's rotation speed.
         * camFollowSpeed is the speed at which the camera chases your player.
         * minPivot is minimum vertical rotation possible on camera based on Y input of the mouse/joystick.
         * maxPivot is maximum vertical rotation possible on camera based on X input of the mouse/joystick.
         */
        [SerializeField] private GameObject followTarget;
        public float camRotSpeedX;
        public float camRotSpeedY;
        public float camFollowSpeed;
        private float minPivot;
        private float maxPivot;
        private float defaultPosition;
        public float cameraCollisionOffset;
        public float minimumCollisionOffset;
        public float cameraCollisionRadius;

        // Camera components:
        public Transform cameraTransform; // Transform of actual camera object.
        private Transform thisTransform;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        public Transform cameraPivot;
        public LayerMask collisionLayers;
        private float lookAngle;
        private float pivotAngle;
        private Vector3 cameraVectorPosition;
        
        // Player inputs
        private M_InputHandler inputHandler;

        // Called before the first frame.
        private void Start()
        {
            thisTransform = transform;

            // Cam rotation offset
            transform.rotation = Quaternion.Euler(6.25f, 0, 0);
            
            // Initialising some components.
            inputHandler = FindObjectOfType<M_InputHandler>();
            
            // Camera Reset Position.
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            defaultPosition = cameraTransform.localPosition.z;
            
            // Setting the default cam settings.
            minPivot = -30f;
            maxPivot = 45f;

            minimumCollisionOffset = 0.2f;
            cameraCollisionOffset = 0.2f;
            cameraCollisionRadius = 0.2f;

            camFollowSpeed = 0.25f;
            camRotSpeedX = 0.4f;
            camRotSpeedY = 0.4f;
        }

        private void Update()
        {
            HandleThirdPersonCam();
        }


        // Camera Logic.

        #region Camera
        
        // Calls the player cam logic methods.
        private void HandleThirdPersonCam()
        {
            float delta = Time.deltaTime;
            FollowTarget();
            RotateCamera(delta);
            HandleCameraCollisions();
        }

        // Make the camera follow the target, with the pre-specified offsets.
        private void FollowTarget()
        {
            Vector3 targetPos = Vector3.SmoothDamp(thisTransform.position, followTarget.transform.position, 
                ref cameraFollowVelocity, camFollowSpeed);

            thisTransform.position = targetPos;
        }

        // Rotates Camera based on Mouse/Controller input.
        private void RotateCamera(float delta)
        {
            lookAngle += (inputHandler.mouseX * camRotSpeedX);
            //lookAngle = Math.Clamp(lookAngle, minLook, maxLook);
            
            pivotAngle -= (inputHandler.mouseY * camRotSpeedY);
            pivotAngle = Math.Clamp(pivotAngle, minPivot, maxPivot);
            
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivot.localRotation = targetRotation;
        }

        private void HandleCameraCollisions()
        {
            float targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivot.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit,
                    Mathf.Abs(targetPosition), collisionLayers))
            {
                float distance = Vector3.Distance(cameraPivot.position, hit.point);
                targetPosition =- distance - cameraCollisionOffset;
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition -= minimumCollisionOffset;
            }

            cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
            cameraTransform.localPosition = cameraVectorPosition;
        }

        #endregion
    }
}
