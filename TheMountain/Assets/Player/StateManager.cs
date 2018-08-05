using UnityEngine;
using System.Collections;
using Player.PlayerController;
using Player;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerBlackboard))]
[RequireComponent(typeof(CameraManager))]
public class StateManager : PlayerMotor, IHitboxResponder
{

    CameraManager cameraManager;
    public HitboxProfile[] hitboxProfile;

    PlayerHitboxController _hitboxController;
    HitBox _hitbox;

    protected override void Awake()
    {
        base.Awake();
        cameraManager = GetComponent<CameraManager>();

        _hitbox = GetComponentInChildren<HitBox>();
        _hitboxController = GetComponent<PlayerHitboxController>();
    }
    protected override void Start()
    {
        base.Start();
        CalculatePhysicsMaterials();
        StartCoroutine(CharacterInit());
    }


    protected virtual IEnumerator CharacterInit() // TODO: Performance issues on this function
    {
        yield return new WaitForEndOfFrame();
        if (cameraManager.tpCamera == null)
        {
            cameraManager.tpCamera = FindObjectOfType<ThirdPersonCamera>();
            if (cameraManager.tpCamera && cameraManager.tpCamera.target != transform) cameraManager.tpCamera.SetMainTarget(this.transform);
        }
    }



    protected virtual void Update()
    {
        ControlCapsuleHeight();                   // checks the capsule height dependent on crouching, etc
    }


    public override void PlayHurtAnimation(bool value)
    {
        blackboard.animator.Play("Idle_Hit_Strong_Right");
    }
    

    public void CollidedWith(Collider collider)
    {
        Debug.Log("Player collided with " + collider.gameObject.name);
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        IHealthController hurtBoxController = hurtbox.GetComponentInParent<IHealthController>(); // the parent gameobject will implement the health and damage
        Damage attackDamage = new Damage(15);
        hurtBoxController?.ReceiveDamage(attackDamage);
    }


    private void SetResponderToHitbox()
    {
        Debug.Log("Setting player as responder to hitbox");
        _hitbox.SetResponder(this);
    }
}
