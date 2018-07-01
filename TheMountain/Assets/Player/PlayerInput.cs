﻿using UnityEngine;
using System.Collections;
using Player.Input;
/* This class is responsible for setting up the player input.
 * It will recive input and store it in a 2D vector.
 * It will move the character and tell the animator to move based on user input
 * Any extension class to deal with player input will be added to this class
 */

namespace Player.PlayerController
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerInput : MonoBehaviour
    {
        protected PlayerActions playerActions;
        string saveData;


        public bool ignoreCameraRotation;
        
        [Header("Camera Input")]
        [HideInInspector]
        public ThirdPersonCamera tpCamera;              // acess camera info                
        [HideInInspector]
        public string desiredCameraState;                    // generic string to change the CameraState        
        [HideInInspector]
        public string customlookAtPoint;                    // generic string to change the CameraPoint of the Fixed Point Mode        
        [HideInInspector]
        public bool changeCameraState;                      // generic bool to change the CameraState        
        [HideInInspector]
        public bool smoothCameraState;                      // generic bool to know if the state will change with or without lerp  
        [HideInInspector]
        public bool keepDirection;                          // keep the current direction in case you change the cameraState
        protected Vector2 oldInput;
        public UnityEngine.Events.UnityEvent OnLateUpdate;
 
        protected bool isInit;

        [HideInInspector]
        public PlayerInputController playerInputController;


        void OnEnable()
        {
            // See PlayerActions.cs for this setup.
            playerActions = PlayerActions.CreateWithDefaultBindings();
            //playerActions.Move.OnLastInputTypeChanged += ( lastInputType ) => Debug.Log( lastInputType );

            LoadBindings();
        }


        void OnDisable()
        {
            // This properly disposes of the action set and unsubscribes it from
            // update events so that it doesn't do additional processing unnecessarily.
            playerActions.Destroy();
        }

        protected virtual void Start()
        {
            playerInputController = GetComponent<PlayerInputController>();

            if (playerInputController != null)
            {
                playerInputController.CalculatePhysicsMaterials();
                StartCoroutine(CharacterInit());
            }
        }
        protected virtual IEnumerator CharacterInit()
        {
            yield return new WaitForEndOfFrame();
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<ThirdPersonCamera>();
                if (tpCamera && tpCamera.target != transform) tpCamera.SetMainTarget(this.transform);
            }
        }

        void SaveBindings()
        {
            saveData = playerActions.Save();
            PlayerPrefs.SetString("Bindings", saveData);
        }


        void LoadBindings()
        {
            if (PlayerPrefs.HasKey("Bindings"))
            {
                saveData = PlayerPrefs.GetString("Bindings");
                playerActions.Load(saveData);
            }
        }


        void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }


        protected virtual void FixedUpdate()
        {
            playerInputController.GetLocomotionType();
        }

        protected virtual void Update()
        {
            if (playerInputController == null || Time.timeScale == 0) return;
            HandleInput();
            playerInputController.UpdateMotor();                   // checks the capsule height dependent on crouching, etc
        }


        protected virtual void LateUpdate()
        {
            if (playerInputController == null || Time.timeScale == 0) return;

            CameraInput();                      // update camera input
            UpdateCameraStates();               // update camera states
            OnLateUpdate.Invoke();
        }

        protected virtual void HandleInput()
        {
            StoreMovement();
            CheckForSprint();
            CheckForCrouch();
        }

        protected virtual void StoreMovement()
        {

            playerInputController.input.x = playerActions.Move.X;
            playerInputController.input.y = playerActions.Move.Y;
            
        }

        public virtual void StoreMovement(Vector3 position, bool rotateToDirection = true)
        {
            var dir = position - transform.position;
            var targetDir = dir.normalized;
            playerInputController.input.x = targetDir.x;
            playerInputController.input.y = targetDir.z;

            targetDir.y = 0;
            playerInputController.TargetDirection = targetDir;
        }

        public virtual void StoreMovement(Transform _transform, bool rotateToDirection = true)
        {
            StoreMovement(_transform.position, rotateToDirection);
        }

        protected virtual void CheckForSprint()
        {
            if (playerActions.Sprint.IsPressed)
            {
                playerInputController.Sprint(true);
            }
            else
            {
                playerInputController.Sprint(false);
                playerInputController.CurrentSprintStamina = (playerInputController.CurrentSprintStamina < playerInputController.MaxSprintStamina) ? playerInputController.CurrentSprintStamina + Time.deltaTime : playerInputController.MaxSprintStamina;
            }
        }

        protected virtual void CheckForCrouch()
        {
                if (playerActions.Crouch)
                {
                    playerInputController.Crouch();
                    AudioManager.instance.Play("Crouch");
            }
                else
                {
                    playerInputController.IsCrouching = false;
                    AudioManager.instance.StopPlaying("Crouch");
                }
        }

        public virtual void CameraInput()
        {
            if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
            if (!ignoreCameraRotation)
            {
                if (!keepDirection) playerInputController.UpdateTargetDirection(Camera.main.transform);
                RotateWithCamera(Camera.main.transform);
            }

            if (tpCamera == null)
                return;

            var Y = playerActions.MoveMouse.Y;
            var X = playerActions.MoveMouse.X ;
            //var zoom = Input.GetAxis("Mouse ScrollWheel");

            tpCamera.RotateCamera(X, Y);
            //tpCamera.Zoom(zoom);

            // change keepDirection from input diference
            if (keepDirection && Vector2.Distance(playerInputController.input, oldInput) > 0.2f) keepDirection = false;
        }

        protected virtual void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<ThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
            if (changeCameraState)
                tpCamera.ChangeState(desiredCameraState, customlookAtPoint, smoothCameraState);
            else if (playerInputController.IsCrouching)
                tpCamera.ChangeState("Crouch", true);
            else
                tpCamera.ChangeState("MovementState", true);
        }

        public void ChangeCameraState(string cameraState)
        {
            changeCameraState = true;
            desiredCameraState = cameraState;
        }

        public void ResetCameraState()
        {
            changeCameraState = false;
            desiredCameraState = string.Empty;
        }

        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
                // smooth align character with aim position               
                if (tpCamera != null && tpCamera.lockTarget)
                {
                    playerInputController.RotateToTarget(tpCamera.lockTarget);
                }
                // rotate the camera around the character and align with when the char move
                else if (playerInputController.input != Vector2.zero)
                {
                    playerInputController.RotateWithAnotherTransform(cameraTransform);
                }
            
        }

    }
}