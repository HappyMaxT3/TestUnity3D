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
        [SerializeField] float rotationSpeed = 10;


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
            //moveDirection = cameraObject.forward * inputHandler.vertical;
            //moveDirection = cameraObject.right * inputHandler.horizontal;
            moveDirection = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);
            moveDirection.y = 0;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rb.velocity = projectedVelocity;

            animatorHandler.UpdatteAnimatorValues(inputHandler.moveAmount, 0);

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