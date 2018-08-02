using UnityEngine;

// TODO: Calculate hashes for animator
/* Actually moves the player
 * It accepts input from PlayerInput, it gets the values froom the MovementController about its transitions, and how to move from the PlayerMotor
 */

namespace Player.PlayerController
{
    public abstract class PlayerAnimator : PlayerMotor
    {
        private float randomIdleCount, randomIdle;
        private float _speed = 0;
        
        // get Layers from the Animator Controller
        [HideInInspector]
        public AnimatorStateInfo baseLayerInfo, underBodyInfo, rightArmInfo, leftArmInfo, fullBodyInfo, upperBodyInfo;
        private int baseLayer
        { get { return animator.GetLayerIndex("Base Layer"); } }
        private int underBodyLayer
        { get { return animator.GetLayerIndex("UnderBody"); } }
        private int rightArmLayer
        { get { return animator.GetLayerIndex("RightArm"); } }
        private int leftArmLayer
        { get { return animator.GetLayerIndex("LeftArm"); } }
        private int upperBodyLayer
        { get { return animator.GetLayerIndex("UpperBody"); } }
        private int fullbodyLayer
        { get { return animator.GetLayerIndex("FullBody"); } }
        //Current combo
        public int combo { get; set; }
        public void OnAnimatorMove()
        {
            UpdateAnimator();
            if (useRootMotion)
            {
                transform.position = animator.rootPosition;
                //transform.rotation = animator.rootRotation;
            }
        }

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            LayerControl();

            RandomIdle();

            LocomotionAnimation();

            AttackingAnimation();
        }

        public void LayerControl()
        {
            baseLayerInfo = animator.GetCurrentAnimatorStateInfo(baseLayer);
            rightArmInfo = animator.GetCurrentAnimatorStateInfo(rightArmLayer);
            leftArmInfo = animator.GetCurrentAnimatorStateInfo(leftArmLayer);
            upperBodyInfo = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            fullBodyInfo = animator.GetCurrentAnimatorStateInfo(fullbodyLayer);
        }

        private void RandomIdle()
        {
            if (input != Vector2.zero) return;

            if (input.sqrMagnitude == 0 && !isCrouching && _capsuleCollider.enabled)
            {
                randomIdleCount += Time.fixedDeltaTime;
                if (randomIdleCount > 6)
                {
                    randomIdleCount = 0;
                    animator.SetTrigger(Utility.Constants.IdleRandomTrigger);
                    //animator.SetInteger(Utility.Constants.IdleRandom, Random.Range(0, 1));
                }
            }
            else
            {
                randomIdleCount = 0;
                animator.SetInteger(Utility.Constants.IdleRandom, 0);
            }
        }

        public void LocomotionAnimation()
        {
            animator.SetBool("IsCrouching", IsCrouching);
            animator.SetFloat(Utility.Constants.InputMagnitude, speed, .2f, Time.deltaTime);

            var dir = transform.InverseTransformDirection(Camera.main.transform.position);
            dir.z *= speed;
            animator.SetFloat("InputVertical", Mathf.Clamp(dir.z, -1, 1));
            animator.SetFloat("InputHorizontal", Mathf.Clamp(dir.x, -1, 1));
        }

        public void AttackingAnimation()
        {
            if (_isAttacking)
            {
                if (IsLightAttack)
                { 
                    animator.SetTrigger("LightAttack");
                }
                if (IsHeavyAttack)
                {
                    animator.SetTrigger("HeavyAttack");
                }
                animator.SetInteger("AttackCombo", combo);
            }
            else
            {
                animator.ResetTrigger("LightAttack");
                animator.ResetTrigger("HeavyAttack");
                ResetAttackTriggers = false;
                AttackID = 0;
                //CanLightAttackAgain = false;
                //CanHeavyAttackAgain = false;
            }
            animator.SetInteger("AttackID", (int) AttackID);
        }

        public override void PlayHurtAnimation(bool value)
        {
            animator.Play("Idle_Hit_Strong_Right");
        }

    }
}