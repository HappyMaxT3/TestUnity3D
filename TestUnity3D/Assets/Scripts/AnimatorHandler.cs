using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator animator;
        public InputHandler inputHandler;
        public PlayerLocomotion playerLocomotion;
        private int verticalParam;
        private int horizontalParam;
        public bool canRotate;

        public void Initialize()
        {
            animator = GetComponent<Animator>();
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            verticalParam = Animator.StringToHash("Vertical");
            horizontalParam = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (verticalMovement > 0 && verticalMovement <= 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1;
            }
            else if (verticalMovement < 0 && verticalMovement >= -0.55f)
            {
                v = -0.5f;
            }

            if (isSprinting)
            {
                verticalMovement = playerLocomotion.sprintSpeed;
                v = verticalMovement; 
            }
            #endregion

            #region Horizontal 
            float h = 0;
            if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (horizontalMovement > 0 && horizontalMovement <= 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else if (horizontalMovement < 0 && horizontalMovement >= -0.55f)
            {
                h = -0.5f;
            }

            if (isSprinting)
            {
                horizontalMovement = playerLocomotion.sprintSpeed;
                h = horizontalMovement;
            }
            #endregion


            v = Mathf.Clamp(v, -2, 2);
            h = Mathf.Clamp(h, -2, 2);

            animator.SetBool("isSprinting", isSprinting);

            animator.SetFloat(verticalParam, v, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontalParam, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(targetAnim, 0.2f);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotate()
        {
            canRotate = false;
        }

        private void OnAnimatorMove()
        {
            if (!inputHandler.isInteracting)
            {
                return;
            }
            float delta = Time.deltaTime;
            playerLocomotion.rb.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rb.velocity = velocity;
        }
    }
}
