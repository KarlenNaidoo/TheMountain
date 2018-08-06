﻿using UnityEngine;

// TODO: Calculate hashes for animator
/* Actually moves the player
 * It accepts input from PlayerInput, it gets the values froom the MovementController about its transitions, and how to move from the PlayerMotor
 */

namespace Player.PlayerController
{
    public class PlayerAnimator : MonoBehaviour
    {

        PlayerBlackboard blackboard;
        ControllerActionManager controllerActionManager;
        private float randomIdleCount, randomIdle;
        private float _speed = 0;
        
        // get Layers from the Animator Controller
        [HideInInspector]
        public AnimatorStateInfo baseLayerInfo, underBodyInfo, rightArmInfo, leftArmInfo, fullBodyInfo, upperBodyInfo;
        private int baseLayer
        { get { return blackboard.animator.GetLayerIndex("Base Layer"); } }
        private int underBodyLayer
        { get { return blackboard.animator.GetLayerIndex("UnderBody"); } }
        private int rightArmLayer
        { get { return blackboard.animator.GetLayerIndex("RightArm"); } }
        private int leftArmLayer
        { get { return blackboard.animator.GetLayerIndex("LeftArm"); } }
        private int upperBodyLayer
        { get { return blackboard.animator.GetLayerIndex("UpperBody"); } }
        private int fullbodyLayer
        { get { return blackboard.animator.GetLayerIndex("FullBody"); } }
        //Current combo
        public int combo { get; set; }
        string targetAnim;
        protected virtual void Awake()
        {

            blackboard = GetComponent<PlayerBlackboard>();
            controllerActionManager = GetComponent<ControllerActionManager>();

        }
        protected virtual void Start()
        {


        }
        public void OnAnimatorMove()
        {
            UpdateAnimator();
            if (blackboard.useRootMotion)
            {
                transform.position = blackboard.animator.rootPosition;
                //transform.rotation = animator.rootRotation;
            }
        }

        public virtual void UpdateAnimator()
        {
            if (blackboard.animator == null || !blackboard.animator.enabled) return;

            LayerControl();

            SetIdle();

            LocomotionAnimation();

            ResetAnimation();

            PlayTargetAnimation();
        }

        protected virtual void PlayTargetAnimation()
        {
            if (blackboard.actionSlot != null && fullBodyInfo.IsName("ResetState")) // we need to be in the empty state in order to transition
            {
                targetAnim = blackboard.actionSlot.targetAnim;
                blackboard.animator.Play(targetAnim);
            }
        }

        protected void ResetAnimation()
        {
            if (blackboard.canAttack)
            {
                ControllerActionInput a_input = controllerActionManager.GetActionInput();
                if(a_input != ControllerActionInput.None)
                {

                    Debug.Log("Attacking again");
                    blackboard.animator.Play("ResetState");
                    blackboard.canAttack = false;
                    blackboard.attackAgain = false;
                }
            }
        }

        public void LayerControl()
        {
            baseLayerInfo = blackboard.animator.GetCurrentAnimatorStateInfo(baseLayer);
            rightArmInfo = blackboard.animator.GetCurrentAnimatorStateInfo(rightArmLayer);
            leftArmInfo = blackboard.animator.GetCurrentAnimatorStateInfo(leftArmLayer);
            upperBodyInfo = blackboard.animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            fullBodyInfo = blackboard.animator.GetCurrentAnimatorStateInfo(fullbodyLayer);
        }

        private void SetIdle()
        {
            blackboard.animator.SetFloat("IsTwoHanded", (float) blackboard.currentWeapon);
        }

        public void LocomotionAnimation()
        {
            blackboard.animator.SetBool("IsCrouching", blackboard.isCrouching);
            blackboard.animator.SetFloat(Utility.Constants.InputMagnitude, blackboard.speed, .2f, Time.deltaTime);

            var dir = transform.InverseTransformDirection(Camera.main.transform.position);
            dir.z *= blackboard.speed;
            blackboard.animator.SetFloat("InputVertical", Mathf.Clamp(dir.z, -1, 1));
            blackboard.animator.SetFloat("InputHorizontal", Mathf.Clamp(dir.x, -1, 1));
        }

       
        public virtual void PlayHurtAnimation(bool value)
        {
            blackboard.animator.Play("Idle_Hit_Strong_Right");
        }

        public virtual void Sprint(bool value)
        {
            if (value)
            {
                if (blackboard.currentSprintStamina > 0 && blackboard.input.sqrMagnitude > 0.1f)
                {
                    blackboard.isSprinting = true;
                    blackboard.isRunning = false;
                }
                else if (blackboard.currentSprintStamina <= 0)
                {
                    blackboard.isRunning = true;
                    blackboard.isSprinting = false;
                }

                if (blackboard.input.sqrMagnitude < 0.1f || blackboard.isCrouching || blackboard.speed <= 0)
                {
                    blackboard.isSprinting = false;
                    blackboard.isRunning = false;
                }
                blackboard.currentSprintStamina = (blackboard.currentSprintStamina > 0) ? blackboard.currentSprintStamina - Time.deltaTime : 0;
            }
            else
            {
                blackboard.isSprinting = false;
                blackboard.isRunning = false;
            }
        }

        public virtual void Crouch()
        {

            blackboard.isCrouching = true;

        }
    }
}