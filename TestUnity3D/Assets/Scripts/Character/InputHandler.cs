using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Main
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool buttonInputRoll;
        public bool buttonInputSprint;
        public bool buttonInputJump;   

        public bool rollFlag;
        public float rollInputTimer;
        public bool sprintFlag;
        public bool jumpFlag;
        public bool hitFlag;

        PlayerControls inputActions;

        Vector2 movementInput;
        Vector2 cameraInput;

        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new PlayerControls();

                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerActions.Roll.performed += i => rollFlag = true;
                inputActions.PlayerActions.Roll.canceled += i => rollFlag = false;

                inputActions.PlayerActions.Jump.performed += i => jumpFlag = true;
                inputActions.PlayerActions.Jump.canceled += i => jumpFlag = false;

                inputActions.PlayerActions.Hit.performed += i => hitFlag = true;
                inputActions.PlayerActions.Hit.canceled += i => hitFlag = false;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleJumpInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            buttonInputRoll = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            buttonInputSprint = inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            buttonInputJump = inputActions.PlayerActions.Jump.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            if (buttonInputRoll)
            {
                Debug.Log("Player Action!");
                rollFlag = true;
                sprintFlag = false;
            }
            else if (buttonInputSprint)
            {
                sprintFlag = true;
            }
            else if (buttonInputJump)
            {
                rollFlag = false;
                jumpFlag = true;
            }
            else
            {
                jumpFlag = false;
                sprintFlag = false;
                rollFlag = false;
            }
        }

        private void HandleJumpInput(float delta)
        {
            buttonInputJump = inputActions.PlayerActions.Jump.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            if (buttonInputJump && !jumpFlag)
            {
                jumpFlag = true;
            }
        }

    }
}

