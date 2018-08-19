using UnityEngine;
using System.Collections;

public class Phase : MonoBehaviour 
{
    PlayerBlackboard _blackboard;
    Rigidbody _rb;
    [SerializeField] float phaseDistance = 10f;
    
    // Use this for initialization
    void Start()
    {
        _blackboard = GetComponent<PlayerBlackboard>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (_blackboard.actionSlot != null && _blackboard.actionSlot.inputButton == ControllerActionInput.L1 && _blackboard.input.sqrMagnitude > 0.3f)
        {
            _rb.position = transform.position + transform.forward * phaseDistance;
            //_rb.MovePosition(transform.position + (transform.forward * phaseDistance));
        }
    }
}
