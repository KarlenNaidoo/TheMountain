using UnityEngine;

// TODO: Calculate hashes for animator
/* Actually moves the player
 * It accepts input from PlayerInput, it gets the values froom the MovementController about its transitions, and how to move from the PlayerMotor
 */

namespace Player.PlayerController
{
    public class PlayerAnimator : MonoBehaviour
    {

        PlayerBlackboard blackboard;
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

            RandomIdle();

            LocomotionAnimation();
            if(blackboard.actionSlot != null)
            {
                targetAnim = blackboard.actionSlot.targetAnim;
                blackboard.animator.CrossFade(targetAnim, 0.2f);
            }
                
            Debug.Log(targetAnim);
            //AttackingAnimation();
        }

        public void LayerControl()
        {
            baseLayerInfo = blackboard.animator.GetCurrentAnimatorStateInfo(baseLayer);
            rightArmInfo = blackboard.animator.GetCurrentAnimatorStateInfo(rightArmLayer);
            leftArmInfo = blackboard.animator.GetCurrentAnimatorStateInfo(leftArmLayer);
            upperBodyInfo = blackboard.animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            fullBodyInfo = blackboard.animator.GetCurrentAnimatorStateInfo(fullbodyLayer);
        }

        private void RandomIdle()
        {
            if (blackboard.input != Vector2.zero) return;

            if (blackboard.input.sqrMagnitude == 0 && !blackboard.isCrouching)
            {
                randomIdleCount += Time.fixedDeltaTime;
                if (randomIdleCount > 6)
                {
                    randomIdleCount = 0;
                    blackboard.animator.SetTrigger(Utility.Constants.IdleRandomTrigger);
                    //animator.SetInteger(Utility.Constants.IdleRandom, Random.Range(0, 1));
                }
            }
            else
            {
                randomIdleCount = 0;
                blackboard.animator.SetInteger(Utility.Constants.IdleRandom, 0);
            }
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