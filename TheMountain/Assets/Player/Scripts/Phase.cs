using UnityEngine;
using System.Collections;

public class Phase : MonoBehaviour 
{
    PlayerBlackboard _blackboard;
    Rigidbody _rb;
    float offsetY;
    [SerializeField] float phaseDistance = 10f;
    int layerMask = 1 << 16; // ground
    // Use this for initialization
    void Start()
    {
        _blackboard = GetComponent<PlayerBlackboard>();
        _rb = GetComponent<Rigidbody>();
        
        // But instead we want to collide against everything except layermask. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;


    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        float contextualPhaseDistance = phaseDistance;
        if (_blackboard.actionSlot != null && _blackboard.actionSlot.inputButton == ControllerActionInput.L1 && _blackboard.input.sqrMagnitude > 0.3f)
        {
            contextualPhaseDistance = CheckForObjectCollision();
            Debug.Log("Phase: " + contextualPhaseDistance);
            _rb.position = transform.position + transform.forward * contextualPhaseDistance ;
            _blackboard.animator.Play("Phase");
        }
    }

    float CheckForObjectCollision()
    {

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.forward, out hit, phaseDistance, layerMask))
        {
            offsetY = hit.point.y;
            return hit.distance > 0.5 ? hit.distance - 0.5f : 0;
        }
        else
        {
            return phaseDistance;
        }
    }
}
