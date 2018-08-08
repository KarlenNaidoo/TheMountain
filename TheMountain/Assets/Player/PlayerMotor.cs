
using UnityEngine;
using static Player.Utility;

/* Calculates the movement around the world and passes this onto the player animator
 */

namespace Player.PlayerController
{
    public class PlayerMotor : Character
    {
        public MovementSpeed freeSpeed;
        new PlayerBlackboard blackboard;

        public LocomotionType locomotionType = LocomotionType.Free;

        protected Quaternion freeRotation;

        protected override void Awake()
        {
            base.Awake();
            blackboard = GetComponent<PlayerBlackboard>() ;
        }
        [SerializeField] protected float maxSprintStamina = 10f;

       
        // generate input for the controller
        protected bool rotateByWorld = false;
        
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
                blackboard.targetRotation = Quaternion.Euler(newPos);
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
            blackboard.targetRotation = transform.rotation;
        }

        //Limits the character to walk only, useful for cutscenes and 'indoor' areas
        public void SetWalkByDefault(bool value)
        {
            freeSpeed.walkByDefault = value;
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
                blackboard.targetDirection = blackboard.input.x * right + blackboard.input.y * forward;
            }
            else
                blackboard.targetDirection = new Vector3(blackboard.input.x, 0, blackboard.input.y);
        }

        protected virtual void FreeMovement()
        {
            ControlMovementSpeed();

            if (blackboard.input != Vector2.zero && blackboard.targetDirection.magnitude > 0.1f)
            {
                Vector3 lookDirection = blackboard.targetDirection.normalized;
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
            blackboard.targetRotation = Quaternion.Euler(newPos);
            if (ignoreLerp)
                transform.rotation = Quaternion.Euler(newPos);
            else transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), 0.1f * Time.deltaTime);
            blackboard.targetDirection = direction;
        }


        private void ControlMovementSpeed()
        {
            // set speed to both vertical and horizontal inputs
            blackboard.speed = Mathf.Abs(blackboard.input.x) + Mathf.Abs(blackboard.input.y);
            if (blackboard.isSprinting || blackboard.isRunning) blackboard.speed += 2f;

            if (blackboard.isRunning)
                blackboard.speed = Mathf.Clamp(blackboard.speed, 1f, 2f);

            if (!blackboard.isSprinting && !blackboard.isRunning || freeSpeed.walkByDefault)
                blackboard.speed = Mathf.Clamp(blackboard.speed, 0f, 1f);
        }


        protected virtual void FixedUpdate()
        {
            GetLocomotionType();
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