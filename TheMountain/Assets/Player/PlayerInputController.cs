using UnityEngine;
using Characters;

/* This class looks at all raycasts and transitions, e.g. Crouch, sprint, strafe and sets their values so that the animations can change states
 */

namespace Player.PlayerController
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : PlayerAnimator, IAttackListener
    {

        public static PlayerInputController instance;
        // Use this for initialization
        public void Start()
        {
            if (instance != null)
            {
                Debug.LogWarning("There is more than one movement controller in the scene");
                return;

            }
            else
            {
                instance = this;
            }
        }

        #region Locomotion Actions

        public virtual void Sprint(bool value)
        {
            if (value)
            {
                if (currentSprintStamina > 0 && input.sqrMagnitude > 0.1f)
                {
                    isSprinting = true;
                    isRunning = false;
                }
                else if (currentSprintStamina <= 0)
                {
                    isRunning = true;
                    isSprinting = false;
                }

                if (input.sqrMagnitude < 0.1f || isCrouching || speed <= 0)
                {
                    isSprinting = false;
                    isRunning = false;
                }
                currentSprintStamina = (currentSprintStamina > 0) ? currentSprintStamina - Time.deltaTime : 0;
            }
            else
            {
                isSprinting = false;
                isRunning = false;
            }
        }

        #endregion Locomotion Actions

        public virtual void Crouch()
        {
            
            IsCrouching = true;

        }

        public virtual bool CanAttack()
        {
            if (isCrouching)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public virtual void LightAttack()
        {

            IsAttacking = CanAttack();
            IsLightAttack = true;

        }

        public virtual void HeavyAttack()
        {
            IsAttacking = CanAttack();
            IsHeavyAttack = true;

        }

        public void OnEnableAttack()
        {
            IsAttacking = true;
            
        }

        public void OnDisableAttack()
        {
            ResetAttackTrigger();

        }

        public void ResetAttackTrigger()
        {
            IsAttacking = false;
            ResetAttackTriggers = true;
            IsLightAttack = false;
            IsHeavyAttack = false;
        }

    }
}