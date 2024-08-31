using Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    InputHandler inputHandler;
    Animator animator;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    [Header("Flags:")]
    public bool isInteracting;
    public bool isGrounded;
    public bool isInAir;
    public bool isSprinting;
    public bool isJumping;

    private void Awake()
    {
        cameraHandler = CameraHandler.singleton;
    }

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        animator = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        isInteracting = animator.GetBool("isInteracting");

        float delta = Time.deltaTime;

        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollAndSprint(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);

    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        inputHandler.TickInput(delta);

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraLocation(delta, inputHandler.mouseX, inputHandler.mouseY);

        }
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        inputHandler.jumpFlag = false;

        isSprinting = inputHandler.sprintFlag;
        isJumping = inputHandler.jumpFlag;

        if(isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }
}
