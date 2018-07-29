using UnityEngine;

/* Calculates a new material dependent on the surface the character is next to.
 * This prevents obstructions and sticking to objects
 * Every humanoid or AI can inherit from this class
 */

 [RequireComponent(typeof(Animator))]
    public abstract class Character : HealthController
    {
        // get the animator component of character
        protected Animator animator { get; private set; }

        #region Physics Variables

        protected Rigidbody _rigidbody;                                // access the Rigidbody component
        protected PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;       // create PhysicMaterial for the Rigidbody
        protected CapsuleCollider _capsuleCollider;                    // access CapsuleCollider information
        protected float colliderRadius, colliderHeight;        // storage capsule collider extra information        ]
        protected Vector3 colliderCenter;                      // storage the center of the capsule collider info

        #endregion Physics Variables

        protected void Awake()
        {
            animator = GetComponent<Animator>();
        }

        #region Physics Material

        public void CalculatePhysicsMaterials()
        {
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            // slides the character through walls and edges
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // air physics
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();

            // capsule collider info
            _capsuleCollider = GetComponent<CapsuleCollider>();

            // save your collider preferences
            colliderCenter = GetComponent<CapsuleCollider>().center;
            colliderRadius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;

            // avoid collision detection with inside colliders
            Collider[] AllColliders = this.GetComponentsInChildren<Collider>();
            Collider thisCollider = GetComponent<Collider>();
            for (int i = 0; i < AllColliders.Length; i++)
            {
                Physics.IgnoreCollision(thisCollider, AllColliders[i]);
            }
        }
    
        #endregion Physics Material

        public override void TakeDamage(Damage damage)
        {
            base.TakeDamage(damage);
            //TriggerDamageReaction(damage);
        }
    }
