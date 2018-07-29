using UnityEngine;
using System.Collections;

public class ObjectDamage : MonoBehaviour
{
    public Damage damage;

    protected virtual void OnTriggerEnter(Collider hit)
    {
        ApplyDamage(hit.transform, transform.position);
    }

    protected virtual void OnTriggerExit(Collider hit)
    {

    }

    protected virtual void ApplyDamage(Transform target, Vector3 hitPoint)
    {
        damage.sender = transform;
        damage.hitPosition = hitPoint;
        Debug.Log("Applying damage to " + target.gameObject.name);
        target.gameObject.ApplyDamage(damage);
    }
}

    [System.Serializable]
    public class Damage
    {
        public int damageValue = 15;
        public bool ignoreDefense;
        [HideInInspector]
        public Transform sender;
        [HideInInspector]
        public Transform receiver;
        [HideInInspector]
        public Vector3 hitPosition;
        public bool hitReaction = true;
        public string attackName;

        public Damage(int value)
        {
            this.damageValue = value;
            this.hitReaction = true;
        }

        public Damage(Damage damage)
        {

            this.damageValue = damage.damageValue;
            this.ignoreDefense = damage.ignoreDefense;
            this.sender = damage.sender;
            this.receiver = damage.receiver;
            this.attackName = damage.attackName;
            this.hitPosition = damage.hitPosition;
        }

    }
}
