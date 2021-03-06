﻿using UnityEngine;

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

        [SerializeField] float turnSensitivity = 0.2f; // Animator turning sensitivity
        [SerializeField] float turnSpeed = 5f; // Animator turning interpolation speed
        [SerializeField] float runCycleLegOffset = 0.2f; // The offset of leg positions in the running cycle
        [Range(0.1f, 3f)] [SerializeField] float animSpeedMultiplier = 1; // How much the animation of the character will be multiplied by
        private Vector3 lastForward;
        private float deltaAngle;

        [SerializeField] GameObject weapon;
        [SerializeField] GameObject weaponParentDestination;
        [SerializeField] GameObject weaponParentOrigin;

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

        protected virtual void Awake()
        {
            blackboard = GetComponent<PlayerBlackboard>();
            controllerActionManager = GetComponent<ControllerActionManager>();
        }

        protected virtual void Start()
        {

            lastForward = transform.forward;
        }

        public void OnAnimatorMove()
        {

            if (blackboard.useRootMotion)
            {
                transform.position = blackboard.animator.rootPosition;
                //transform.rotation = animator.rootRotation;
            }

            Move(blackboard.animator.deltaPosition, blackboard.animator.deltaRotation);
        }


        // When the Animator moves
        public virtual void Move(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            // Accumulate delta position, update in FixedUpdate to maintain consitency
            blackboard.fixedDeltaPosition += deltaPosition;
            blackboard.fixedDeltaRotation *= deltaRotation;
        }


        public virtual void UpdateAnimator()
        {
            if (blackboard.animator == null || !blackboard.animator.enabled) return;

            LayerControl();
            EquipWeapon();
            LocomotionAnimation();
            PlayTargetAnimation();
            CheckForCombo();
        }

        protected virtual void PlayTargetAnimation()
        {

            string targetAnim;
            if (blackboard.actionSlot != null && fullBodyInfo.IsName("ResetState")) // we need to be in the empty state in order to transition
            {
                targetAnim = blackboard.actionSlot.targetAnim;
                blackboard.animator.Play(targetAnim);

            }
            else if (blackboard.actionSlot == null && fullBodyInfo.IsName("ResetState"))
            {
                blackboard.canAttack = false;
                blackboard.doOnce = false;
            }
        }

        protected void CheckForCombo()
        {
            if (blackboard.canAttack)
            {
                ControllerActionInput a_input = controllerActionManager.GetActionInput();
                if (a_input == ControllerActionInput.R1 && !blackboard.doOnce)
                {
                    blackboard.animator.SetTrigger("LightAttack");
                    blackboard.doOnce = true;
                    return;
                }
                if (a_input == ControllerActionInput.R2 && !blackboard.doOnce)
                {
                    blackboard.animator.SetTrigger("HeavyAttack");
                    blackboard.doOnce = true;
                    return;
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

        private void EquipWeapon()
        {
            if (blackboard.currentWeapon == WeaponStatus.OneHanded && !blackboard.weaponEquipped)
            {
                blackboard.animator.CrossFade("Sword1h_Equip", 0.4f);
                blackboard.weaponEquipped = true;
                weapon.transform.parent = weaponParentDestination.transform;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.localPosition = Vector3.zero;
            }
                
            blackboard.animator.SetFloat("IsTwoHanded", Mathf.Lerp(blackboard.animator.GetFloat("IsTwoHanded"), (float)blackboard.currentWeapon, Time.deltaTime));
        }

        public void LocomotionAnimation()
        {


            // Calculate the angular delta in character rotation
            float angle = -GetAngleFromForward(lastForward) - deltaAngle;
            deltaAngle = 0f;
            lastForward = transform.forward;
            angle *= turnSensitivity * 0.01f;
            angle = Mathf.Clamp(angle / Time.deltaTime, -1f, 1f);

            // Update Animator params
            blackboard.animator.SetFloat("Turn", Mathf.Lerp(blackboard.animator.GetFloat("Turn"), angle, Time.deltaTime * turnSpeed));
            blackboard.animator.SetFloat("InputVertical", blackboard.input.y);
            blackboard.animator.SetFloat("InputHorizontal", blackboard.input.x);
            blackboard.animator.SetBool("Crouch", blackboard.animState.crouch);
            blackboard.animator.SetBool("OnGround", blackboard.animState.onGround);
            blackboard.animator.SetBool("IsStrafing", blackboard.animState.isStrafing);
            blackboard.animator.SetFloat("MovementMagnitude", blackboard.input.magnitude);
            if (blackboard.isSprinting && blackboard.currentSprintStamina > 0)
            {
                blackboard.speed = PlayerBlackboard.SPRINT_SPEED;
            }
            if (blackboard.currentSprintStamina <= 0 && blackboard.isSprinting)
            {
                blackboard.speed = PlayerBlackboard.RUN_SPEED;
            }
            blackboard.animator.SetFloat("Speed", blackboard.speed, .2f, Time.deltaTime);

        }


        public virtual void PlayHurtAnimation(bool value)
        {
            blackboard.animator.Play("Idle_Hit_Strong_Right");
        }

        

        public virtual void Crouch()
        {

            blackboard.isCrouching = true;

        }

        // Gets angle around y axis from a world space direction
        public float GetAngleFromForward(Vector3 worldDirection)
        {
            Vector3 local = transform.InverseTransformDirection(worldDirection);
            return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
        }
        protected virtual void Update()
        {
            UpdateAnimator();
        }

    }
    
}