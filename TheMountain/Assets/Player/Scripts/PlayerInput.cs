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
    public class PlayerInput : MonoBehaviour
    {
        protected PlayerActions playerActions;
        string saveData;
        private Rigidbody _rigidbody;
        [SerializeField] float _wallDistance = 5f;
        protected PlayerBlackboard blackboard;
        public Transform headOffset;
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

        protected virtual void Awake()
        {
            blackboard = GetComponent<PlayerBlackboard>();
            _rigidbody = GetComponent<Rigidbody>();
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

        
        protected virtual void Update()
        {
            HandleInput();
        }



        protected virtual void HandleInput()
        {
            StoreMovement();
            CheckForSprint();
            CheckForCrouch();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(headOffset.position, transform.forward * _wallDistance);
           
        }

        protected virtual void StoreMovement()
        {

            RaycastHit hit;
            if (Physics.Raycast(headOffset.position, transform.forward, out hit, _wallDistance))
            {
                    Debug.Log("Hit a wall");
                    blackboard.SetPlayerInputParameters(playerActions.Move.X, 0);
                
            }
            else
            {

                blackboard.SetPlayerInputParameters(playerActions.Move.X, playerActions.Move.Y);
            }
            if (blackboard.runByDefault)
            {
                blackboard.speed = Mathf.Abs(playerActions.Move.Y) + 1;
            }
            else
            {
                blackboard.speed = Mathf.Abs(playerActions.Move.Y);
            }
        }
        
        
        protected virtual void CheckForSprint()
        {
            if (playerActions.Sprint.IsPressed)
            {
                blackboard.isSprinting = true;
                DecreaseSprintStamina(blackboard.currentSprintStamina);
            }
            else
            {

                blackboard.isSprinting = false;
                IncreaseSprintStamina(blackboard.currentSprintStamina);
            }
        }
        

        protected virtual void DecreaseSprintStamina(float currentSprintStamina)
        {
            currentSprintStamina -= Time.deltaTime;
            if(currentSprintStamina <= 0)
            {
                currentSprintStamina = 0;
            }
            blackboard.currentSprintStamina = currentSprintStamina;
        }

        protected virtual void IncreaseSprintStamina(float currentSprintStamina)
        {
            currentSprintStamina += Time.deltaTime;
            if(currentSprintStamina >= blackboard.maxSprintStamina)
            {
                currentSprintStamina = blackboard.maxSprintStamina;
            }
            blackboard.currentSprintStamina = currentSprintStamina;
        }
        protected virtual void CheckForCrouch()
        {
                if (playerActions.Crouch)
                {
                    blackboard.isCrouching = true;
                    AudioManager.instance.Pause("Theme");
                    AudioManager.instance.Play("Crouch");
                }
                else
                {
                    blackboard.isCrouching = false;
                    AudioManager.instance.StopPlaying("Crouch");
                    AudioManager.instance.Play("Theme");
            }
        }


    }
}