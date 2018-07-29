using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour
{
    public Collider trigger;
    public int damagePercentage = 100;
    private bool _canHit;
    [HideInInspector]
    public MeleeAttackObject attackObject;

    public LayerMask mask;
    private Vector3 boxSize;

    private void OnDrawGizmos()
    {
        trigger = gameObject.GetComponent<Collider>();
        if (!trigger) trigger = gameObject.AddComponent<BoxCollider>();
        Color color = Color.red;
        if (!Application.isPlaying && trigger && !trigger.enabled)
            trigger.enabled = true;
        if (trigger && trigger.enabled)
        {
            if (trigger as BoxCollider)
            {

                BoxCollider box = trigger as BoxCollider;

                var sizeX = transform.lossyScale.x * box.size.x;
                var sizeY = transform.lossyScale.y * box.size.y;
                var sizeZ = transform.lossyScale.z * box.size.z;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }

    private void Start()
    {
        Debug.Log("Intialising box");
        trigger = GetComponent<Collider>();
        if (!trigger) trigger = gameObject.AddComponent<BoxCollider>();
        if (trigger)
        {
            trigger.isTrigger = true;
            //trigger.enabled = false;
        }
        _canHit = true;


        BoxCollider box = trigger as BoxCollider;

        var sizeX = transform.lossyScale.x * box.size.x;
        var sizeY = transform.lossyScale.y * box.size.y;
        var sizeZ = transform.lossyScale.z * box.size.z;
        boxSize = new Vector3(sizeX, sizeY, sizeZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (attackObject != null)
        // {
        //     attackObject.OnHit(this, other);
        // }
        Debug.Log("Trigger being called on hitboxes");
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize, transform.rotation, mask);

        if (colliders.Length > 0)
        {
            Debug.Log("We hit something");
        }
    }
}
