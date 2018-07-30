using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour
{
    private IHitboxResponder _responder = null;
    private Collider _hitBoxTrigger;
    private ColliderState _state;
    private Vector3 _hitBoxSize;
    private AnimEvents _animEvents;
    
    public enum ColliderState { Closed, Open, Colliding }

    private void Awake()
    {
        _hitBoxTrigger = GetComponent<Collider>();
        _animEvents = GetComponentInParent<AnimEvents>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _animEvents.currentHitBoxTrigger = _hitBoxTrigger;
        _responder?.CollidedWith(other);
        _state = ColliderState.Open;
    }

    private void OnTriggerStay(Collider other)
    {
        _state = ColliderState.Colliding;
    }

    private void OnTriggerExit(Collider other)
    {
        _state = ColliderState.Closed;
    }
    

    public void setResponder(IHitboxResponder responder)
    {
        _responder = responder;

    }


    private void OnDrawGizmos()
    {
        //_hitBoxTrigger = gameObject.GetComponent<Collider>();
        checkGizmoColor();
        if (_hitBoxTrigger && _hitBoxTrigger.enabled)
        {
            if (_hitBoxTrigger as BoxCollider)
            {

                BoxCollider box = _hitBoxTrigger as BoxCollider;
                var sizeX = transform.lossyScale.x * box.size.x;
                var sizeY = transform.lossyScale.y * box.size.y;
                var sizeZ = transform.lossyScale.z * box.size.z;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }

    private void checkGizmoColor()
    {
        switch (_state)
        {

            case ColliderState.Closed:

                Gizmos.color = Color.blue;

                break;

            case ColliderState.Open:

                Gizmos.color = Color.green;

                break;

            case ColliderState.Colliding:

                Gizmos.color = Color.yellow;

                break;

        }

    }

}
