using System.Collections;
using System.Collections.Generic;
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
        public bool rollFlag;
        public float rollInputTimer;
        public bool sprintFlag;
        public bool isInteracting;

        PlayerControls inputActions;
        CameraHandler cameraHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            TickInput(delta);

            if(cameraHandler != null )
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraLocation(delta, mouseX, mouseY);

            }
        }

        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new PlayerControls();

                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerActions.Roll.performed += i => rollFlag = true;
                inputActions.PlayerActions.Roll.canceled += i => rollFlag = false;
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
            else
            {
                sprintFlag = false;
            }
        }

    }
}

