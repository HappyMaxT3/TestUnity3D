using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class PlayerLocomotion : MonoBehaviour
    {

        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        public Transform myTransform;
        public AnimatorHandler animatorHandler;

        public Rigidbody rb;
        public GameObject normalCamera;

        [SerializeField] float movementSpeed = 5;
        [SerializeField] float sprintSpeed = 8;
        [SerializeField] float rotationSpeed = 10;

        public bool isSprinting;


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            isSprinting = inputHandler.sprintFlag;

            inputHandler.TickInput(delta);

            HandleMovement(delta);

            HandleRollAndSprint(delta);
        }

        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if(targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        private void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
            {
                return;
            }

            //moveDirection = cameraObject.forward * inputHandler.vertical;
            //moveDirection = cameraObject.right * inputHandler.horizontal;
            moveDirection = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);
            moveDirection.y = 0;
            moveDirection.Normalize();

            float speed = movementSpeed;
            if (inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rb.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollAndSprint(float delta)
        {
            if (animatorHandler.animator.GetBool("isInteracting"))
            {
                return;
            }

            if (inputHandler.rollFlag)
            {
                Debug.Log("Roll!");
                moveDirection = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);

                if(inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        #endregion

    }
}