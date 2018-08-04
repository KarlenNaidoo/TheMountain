using UnityEngine;

/* Calculates a new material dependent on the surface the character is next to.
 * This prevents obstructions and sticking to objects
 * Every humanoid or AI can inherit from this class
 */

 [RequireComponent(typeof(Animator))]
public abstract class Character : HealthController
{
    

    protected Rigidbody _rigidbody;                                // access the Rigidbody component
    protected PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;       // create PhysicMaterial for the Rigidbody
    protected CapsuleCollider _capsuleCollider;                    // access CapsuleCollider information
    protected float colliderRadius, colliderHeight;        // storage capsule collider extra information        ]
    protected Vector3 colliderCenter;                      // storage the center of the capsule collider info
    protected PlayerBlackboard blackboard;
    protected virtual void Awake()
    {
        blackboard = GetComponent<PlayerBlackboard>();
        
    }
    

    public void CalculatePhysicsMaterials()
    {
        blackboard.animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

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


    public void ControlCapsuleHeight()
    {
        if (blackboard.isCrouching)
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


    /// <summary>
    /// Disables rigibody gravity, turn the capsule collider trigger and reset all input from the animator.
    /// </summary>
    public void DisableGravityAndCollision()
    {
        blackboard.animator.SetFloat("InputHorizontal", 0f);
        blackboard.animator.SetFloat("InputVertical", 0f);
        blackboard.animator.SetFloat("VerticalVelocity", 0f);
        _rigidbody.useGravity = false;
        _capsuleCollider.isTrigger = true;
    }


}
