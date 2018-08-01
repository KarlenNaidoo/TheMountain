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
        damage.hitPosition = hitPoint;
        Debug.Log("Applying damage to " + target.gameObject.name);
        target.gameObject.ApplyDamage(damage);
    }
}


