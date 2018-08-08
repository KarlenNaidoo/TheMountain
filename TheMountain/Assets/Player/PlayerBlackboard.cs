using UnityEngine;
using System.Collections;
using Player.PlayerController;
using static Player.Utility;
using System.Collections.Generic;


[RequireComponent(typeof(ControllerActionManager))]
[RequireComponent(typeof(InventoryManager))]
public class PlayerBlackboard : MonoBehaviour, IBlackboard
{
    InventoryManager _inventoryManager;
    ControllerActionManager _actionManager;
    PlayerMotor _motor;
    float _inputX;
    float _inputY;
    
    // general variables to the locomotion
    protected Vector3 _targetDirection;
    protected Quaternion _targetRotation;

    public WeaponAction actionSlot { get; set; }
    public Animator animator { get; set; }

    public float inputX { set { _inputX = value; } }
    public float inputY { set { _inputY = value; } }
    public Vector2 input { get; private set; }
    public Vector2 oldInput { get; set; }


    public bool isCrouching { get; set; }
    public bool isRunning { get; set; }
    public bool isSprinting { get; set; }
    public float maxSprintStamina { get; set; }
    public float currentSprintStamina { get; set; }
    public bool useRootMotion { get; set; } = true;
    public float speed { get; set; }
    public Quaternion targetRotation { get { return _targetRotation; } set { _targetRotation = value; } }
    public Vector3 targetDirection { get { return _targetDirection; } set { _targetDirection = value; } }

    bool shouldAttack;
    public List<HitBoxArea> hitboxes { get; set; }
    public List<HitBox> activeHitboxComponents { get; set; }

    public WeaponStatus currentWeapon { get; set; }
    public WeaponList weaponList { get; set; }
    public bool doOnce { get; set; }
    public bool canAttack { get; set; }
    public void SetPlayerInputParameters(float x, float y)
    {
        input = new Vector2(x, y);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _actionManager = GetComponent<ControllerActionManager>();
        _motor = GetComponent<PlayerMotor>();
        _inventoryManager = GetComponent<InventoryManager>();
    }
    
    public void SetAttackParameters(bool shouldAttack)
    {
        this.shouldAttack = shouldAttack;
    }

    public virtual void RotateToTarget(Transform target)
    {
        _motor.RotateToTarget(target);
    }

    public virtual void RotateWithAnotherTransform(Transform referenceTransform)
    {
        _motor.RotateWithAnotherTransform(referenceTransform);
    }

    public virtual void UpdateTargetDirection(Transform referenceTransform = null)
    {
        _motor.UpdateTargetDirection(referenceTransform);
    }
}
