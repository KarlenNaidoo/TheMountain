using UnityEngine;
using System.Collections;
using Player.PlayerController;
using Player;

[RequireComponent(typeof(CameraManager))]
public class StateManager : PlayerMotor, IHitboxResponder
{

    CameraManager _cameraManager;
    public HitboxProfile[] hitboxProfile;
    PlayerHitboxController _hitboxController;
    HitBox _hitbox;

    protected override void Awake()
    {
        base.Awake();
        _cameraManager = GetComponent<CameraManager>();
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
        if (_cameraManager.tpCamera == null)
        {
            _cameraManager.tpCamera = FindObjectOfType<ThirdPersonCamera>();
            if (_cameraManager.tpCamera && _cameraManager.tpCamera.target != transform) _cameraManager.tpCamera.SetMainTarget(this.transform);
        }
    }



    protected virtual void Update()
    {
        //ControlCapsuleHeight();                   // checks the capsule height dependent on crouching, etc
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
