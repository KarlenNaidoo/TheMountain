using UnityEngine;

/* Calculates a new material dependent on the surface the character is next to.
 * This prevents obstructions and sticking to objects
 * Every humanoid or AI can inherit from this class
 */

namespace Thirdperson.Generic
{
    [RequireComponent(typeof(Animator))]
    public abstract class Character : MonoBehaviour
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

        #endregion Physics Material
    }
}