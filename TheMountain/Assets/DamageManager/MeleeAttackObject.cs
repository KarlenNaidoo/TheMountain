using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeAttackObject : MonoBehaviour
{

    public Damage damage;
    public List<HitBox> hitBoxes;
    public int damageModifier;
    [HideInInspector]
    public bool canApplyDamage;
    public delegate void OnHitEnter();
    public static event OnHitEnter onDamageHit;
    public delegate void OnEnableDamage();
    public static event OnEnableDamage onEnableDamage;
    public delegate void OnDisableDamage();
    public static event OnDisableDamage onDisableDamage;
    private Dictionary<HitBox, List<GameObject>> targetColliders;


    protected virtual void Start()
    {
        targetColliders = new Dictionary<HitBox, List<GameObject>>();// init list of targetColliders

        if (hitBoxes.Count > 0)
        {
            /// inicialize the hitBox properties
            foreach (HitBox hitBox in hitBoxes)
            {
                targetColliders.Add(hitBox, new List<GameObject>());
            }
        }
        else
        {
            this.enabled = false;
        }
    }

    // Set Active all hitboxes of the MeleeAttackObject
    public virtual void SetActiveDamage(bool value)
    {
        canApplyDamage = value;
        for (int i = 0; i < hitBoxes.Count; i++)
        {
            var hitCollider = hitBoxes[i];
            if (value == false && targetColliders != null)
                targetColliders[hitCollider].Clear();
        }
        if (value)
        {
            onEnableDamage();
        }
        else
        {
            onDisableDamage();
        }
    }

    public virtual void OnHit(HitBox hitBox, Collider other)
    {
        if (canApplyDamage && !targetColliders[hitBox].Contains(other.gameObject))
        {
            // check the hitPor
        }
    }

    public void ApplyDamage(HitBox hitBox, Collider other, Damage damage)
    {
        Damage _damage = new Damage(damage);
        _damage.receiver = other.transform;
      //  _damage.damageValue = (int)Mathf.RoundToInt(((float)(damage.damageValue + damageModifier) * (((float)hitBox.damagePercentage) * 0.01f)));
        _damage.sender = transform;
        _damage.hitPosition = hitBox.transform.position;
        other.gameObject.ApplyDamage(_damage);
    }
}

public class HitInfo
{
    public MeleeAttackObject attackObject;
    public HitBox hitBox;
    public Vector3 hitPoint;
    public Collider targetCollider;
    public HitInfo(MeleeAttackObject attackObject, HitBox hitBox, Collider targetCollider, Vector3 hitPoint)
    {
        this.attackObject = attackObject;
        this.hitBox = hitBox;
        this.targetCollider = targetCollider;
        this.hitPoint = hitPoint;
    }
}