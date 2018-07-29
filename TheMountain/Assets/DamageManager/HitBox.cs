using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour
{
    private IHitboxResponder _responder = null;
    public Collider hitBoxTrigger;
    public int damagePercentage = 100;
    private bool _canHit;
    [HideInInspector]
    public MeleeAttackObject attackObject;
    private ColliderState _state;
    public enum ColliderState
    {

        Closed,

        Open,

        Colliding

    }


    public LayerMask mask;
    private Vector3 hitBoxSize;

    private void OnDrawGizmos()
    {
        hitBoxTrigger = gameObject.GetComponent<Collider>();
        if (!hitBoxTrigger) hitBoxTrigger = gameObject.AddComponent<BoxCollider>();
        checkGizmoColor();
        if (!Application.isPlaying && hitBoxTrigger && !hitBoxTrigger.enabled)
            hitBoxTrigger.enabled = true;
        if (hitBoxTrigger && hitBoxTrigger.enabled)
        {
            if (hitBoxTrigger as BoxCollider)
            {

                BoxCollider box = hitBoxTrigger as BoxCollider;

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
        hitBoxTrigger = GetComponent<Collider>();
        _canHit = true;


        BoxCollider hitBox = hitBoxTrigger as BoxCollider;

        var sizeX = transform.lossyScale.x * hitBox.size.x;
        var sizeY = transform.lossyScale.y * hitBox.size.y;
        var sizeZ = transform.lossyScale.z * hitBox.size.z;
        hitBoxSize = new Vector3(sizeX, sizeY, sizeZ);
    }

    private void checkGizmoColor()
    {
        switch (_state)
        {

            case ColliderState.Closed:

                Gizmos.color = Color.gray;

                break;

            case ColliderState.Open:

                Gizmos.color = Color.green;

                break;

            case ColliderState.Colliding:

                Gizmos.color = Color.yellow;

                break;

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger being called on hitboxes");
        Collider[] colliders = Physics.OverlapBox(transform.position, hitBoxSize, transform.rotation, mask);

        if (colliders.Length > 0)
        {
            Debug.Log("We hit something");
        }
    }

    public void startCheckingCollision()
    {
        _state = ColliderState.Open;

    }

    public void stopCheckingCollision()
    {
        _state = ColliderState.Closed;

    }

    private void Update()
    {
        if (_state == ColliderState.Closed) { return; }

        Collider[] colliders = Physics.OverlapBox(transform.position, hitBoxSize, transform.rotation, mask);


        if (colliders.Length > 0)
        {

            _state = ColliderState.Colliding;

            // We should do something with the colliders

        }
        else
        {

            _state = ColliderState.Open;

        }
    }

    public void hitboxUpdate()
    {
        if (_state == ColliderState.Closed) { return; }


        Collider[] colliders = Physics.OverlapBox(transform.position, hitBoxSize, transform.rotation, mask);


        for (int i = 0; i < colliders.Length; i++)
        {

            Collider aCollider = colliders[i];

            _responder?.collisionedWith(aCollider);

        }


        _state = colliders.Length > 0 ? ColliderState.Colliding : ColliderState.Open;


    }

    public void useResponder(IHitboxResponder responder)
    {
        _responder = responder;

    }

}
