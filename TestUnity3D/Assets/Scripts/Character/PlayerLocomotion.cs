using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private PlayerManager playerManager;
        private Transform cameraObject;
        private InputHandler inputHandler;
        public Vector3 moveDirection;

        public Transform myTransform;
        public AnimatorHandler animatorHandler;

        public Rigidbody rb;
        public GameObject normalCamera;

        [Header("Ground detect:")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundDetectRayStartPoint = 0.5f;
        [SerializeField] float minDistanceToFall = 1f;
        [SerializeField] float groundDetectRayDistance = 0.2f;
        private LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement:")]
        [SerializeField] float movementSpeed = 5;
        [SerializeField] public float sprintSpeed = 15;
        [SerializeField] float rotationSpeed = 10;

        [Header("Jump:")]
        [SerializeField] float jumpForce = 165f;
        [SerializeField] float fallSpeed = 45;

        [Header("Roll collider settings:")]
        [SerializeField] float originalColliderHeight;
        [SerializeField] Vector3 originalColliderCenter;
        [SerializeField] float rollColliderHeight = 0.5f; 
        [SerializeField] Vector3 rollColliderCenter = new Vector3(0, 0.5f, 0); 
        public CapsuleCollider capsuleCollider;

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();  
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler = GetComponentInChildren<AnimatorHandler>();

            capsuleCollider = GetComponent<CapsuleCollider>(); 

            originalColliderHeight = capsuleCollider.height;
            originalColliderCenter = capsuleCollider.center;

            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);

        }

        #region Movement

        private Vector3 normalVector;
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

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
            {
                return;
            }

            if(playerManager.isInteracting)
            {
                return;
            }

            moveDirection = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);
            moveDirection.y = 0;
            moveDirection.Normalize();

            float speed = movementSpeed;
            if (inputHandler.sprintFlag)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
            }
            else
            {
                playerManager.isSprinting = false;
            }
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rb.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }

            HandleJumping(delta);
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

                    StartCoroutine(ChangeCollider());
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        private IEnumerator ChangeCollider()
        {
            capsuleCollider.height = rollColliderHeight;
            capsuleCollider.center = rollColliderCenter;

            yield return new WaitForSeconds(1.8f);

            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectRayDistance;

            if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }
            if(playerManager.isInAir)
            {
                rb.AddForce(-Vector3.up * fallSpeed);
                rb.AddForce(moveDirection * fallSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDetectRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minDistanceToFall, Color.red, 0.1f, false);
            if(Physics.Raycast(origin, -Vector3.up, out hit, minDistanceToFall, ignoreForGroundCheck)){
                normalVector = hit.normal;
                Vector3 tarpos = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tarpos.y;

                if (playerManager.isInAir)
                {
                    if(inAirTimer > 0.5f)
                    {
                        Debug.Log("In Air:" + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Land", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Locomotion", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {
                if(playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (!playerManager.isInAir)
                {
                    if (!playerManager.isInteracting)
                    {
                        animatorHandler.PlayTargetAnimation("Fall", true );
                    }

                    Vector3 velocity = rb.velocity;
                    velocity.Normalize();
                    rb.velocity = velocity * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            if (playerManager.isGrounded)
            {
                if(playerManager.isInteracting || inputHandler.moveAmount >0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            }
        }

        private void HandleJumping(float delta)
        {
            if (playerManager.isGrounded && inputHandler.jumpFlag)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z); 
                playerManager.isGrounded = false; 
                inputHandler.jumpFlag = false;
                playerManager.isInAir = true;
                animatorHandler.PlayTargetAnimation("Jump", true);
            }

            HandleFalling(delta, moveDirection);
        }



        #endregion




    }
}