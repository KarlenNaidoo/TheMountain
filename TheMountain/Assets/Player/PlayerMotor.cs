
using UnityEngine;

/* Calculates the movement around the world and passes this onto the player animator
 */

namespace Player.PlayerController
{
    public abstract class PlayerMotor : Character
    {

        public MovementSpeed freeSpeed;

        [HideInInspector]
        public Vector2 input;

        public LocomotionType locomotionType = LocomotionType.Free;

        protected float currentSprintStamina = 10f;

        protected Quaternion freeRotation;

        protected bool
            isCrouching,
            isSprinting,
            isSliding,
            isRunning,
            _isAttacking;

        [SerializeField] protected float maxSprintStamina = 10f;

        protected float randomIdleTime = 0f;

        // generate input for the controller
        protected bool rotateByWorld = false;

        protected float speed;

        // general variables to the locomotion
        protected Vector3 targetDirection;

        protected Quaternion targetRotation;

        protected bool useRootMotion = true;

        public enum LocomotionType { Free, Strafe }

        public Utility.AttackCategory AttackID { get; set; }

        public float CurrentSprintStamina { get { return currentSprintStamina; } set { currentSprintStamina = value; } }

        public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }

        public bool IsCrouching { get; set; }

        public bool IsHeavyAttack { get; set; }

        public bool IsLightAttack { get; set; }

        public bool IsRunning { get { return isRunning; } }

        public bool IsSprinting { get { return isSprinting; } }

        public float MaxSprintStamina { get { return maxSprintStamina; } }

        public bool ResetAttackTriggers { get; set; }

        public float Speed { get { return speed; } set { speed = value; } }

        public Vector3 TargetDirection { get { return targetDirection; } set { targetDirection = value; } }

        public Quaternion TargetRotation { get { return targetRotation; } set { targetRotation = value; } }

        public bool CanLightAttackAgain { get; set; }
        public bool CanHeavyAttackAgain { get; set; }
        public virtual void GetLocomotionType()
        {
            if (locomotionType.Equals(LocomotionType.Free))
                FreeMovement();
        }

        public virtual void RotateToTarget(Transform target)
        {
            if (target)
            {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
                targetRotation = Quaternion.Euler(newPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), Time.deltaTime);
            }
        }

        /// <summary>
        /// Use another transform as  reference to rotate
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void RotateWithAnotherTransform(Transform referenceTransform)
        {
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), Time.deltaTime);
            targetRotation = transform.rotation;
        }

        //Limits the character to walk only, useful for cutscenes and 'indoor' areas
        public void SetWalkByDefault(bool value)
        {
            freeSpeed.walkByDefault = value;
        }

        public virtual void UpdateMotor()
        {
            ControlCapsuleHeight();
        }

        /// <summary>
        /// Update the targetDirection variable using referenceTransform or just input.Rotate by word  the referenceDirection
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void UpdateTargetDirection(Transform referenceTransform = null)
        {
            if (referenceTransform)
            {
                var forward = referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;

                forward = referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0; //set to 0 because of referenceTransform rotation on the X axis

                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                targetDirection = input.x * right + input.y * forward;
            }
            else
                targetDirection = new Vector3(input.x, 0, input.y);
        }

        protected virtual void FreeMovement()
        {
            ControlMovementSpeed();
            //UpdateTargetDirection(Camera.main.transform);
            //RotateToDirection(targetDirection);

            if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y; // get the distance between your position and the target
                var eulerY = transform.eulerAngles.y;

                if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                var euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), freeSpeed.rotationSpeed * Time.deltaTime);
            }
        }

        protected virtual void RotateToDirection(Vector3 direction, bool ignoreLerp = false)
        {
            Quaternion rot = (direction != Vector3.zero) ? Quaternion.LookRotation(direction) : Quaternion.identity;
            var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
            targetRotation = Quaternion.Euler(newPos);
            if (ignoreLerp)
                transform.rotation = Quaternion.Euler(newPos);
            else transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), 0.1f * Time.deltaTime);
            targetDirection = direction;
        }

        private void ControlCapsuleHeight()
        {
            if (isCrouching)
            {
                _capsuleCollider.center = colliderCenter / 1.5f;
                _capsuleCollider.height = colliderHeight / 1.5f;
            }
            else
            {
                // back to the original values
                _capsuleCollider.center = colliderCenter;
                _capsuleCollider.radius = colliderRadius;
                _capsuleCollider.height = colliderHeight;
            }
        }

        private void ControlMovementSpeed()
        {
            // set speed to both vertical and horizontal inputs
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);
            if (isSprinting || isRunning) speed += 2f;

            if (isRunning)
                speed = Mathf.Clamp(speed, 1f, 2f);

            if (!isSprinting && !isRunning || freeSpeed.walkByDefault)
                speed = Mathf.Clamp(speed, 0f, 1f);
        }


        /// <summary>
        /// Disables rigibody gravity, turn the capsule collider trigger and reset all input from the animator.
        /// </summary>
        public void DisableGravityAndCollision()
        {
            animator.SetFloat("InputHorizontal", 0f);
            animator.SetFloat("InputVertical", 0f);
            animator.SetFloat("VerticalVelocity", 0f);
            _rigidbody.useGravity = false;
            _capsuleCollider.isTrigger = true;
        }

        [System.Serializable]
        public class MovementSpeed
        {
            [Tooltip("Rotation speed of the character")]
            public float rotationSpeed = 1.1f;

            [Tooltip("Character will walk by default and run when the sprint input is pressed. The Sprint animation will not play")]
            public bool walkByDefault = false;
        }
    }
}