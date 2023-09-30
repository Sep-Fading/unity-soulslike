using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementScripts
{
    public class M_AnimationHandler : MonoBehaviour
    {
        private Animator animator;
        private int horizontalMovement;
        private int verticalMovement;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            animator = GetComponent<Animator>();
            horizontalMovement = Animator.StringToHash("H_Speed");
            verticalMovement = Animator.StringToHash("V_Speed");
        }

        // Update animation values based on controller input sensitivity.
        public void UpdateAnimationValues(float verticalSpeed, float horizontalSpeed)
        {
            animator.SetFloat(horizontalMovement, horizontalSpeed, 0.1f, Time.deltaTime);
            animator.SetFloat(verticalMovement, verticalSpeed, 0.1f, Time.deltaTime);
        }
    }
}