using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour {

    public abstract AIStateType GetStateType();
    public abstract AIStateType OnUpdate();

    public virtual void OnEnterState() { }
    public virtual void OnExitState() { }
    // so Unity does not call OnAnimatorMove for all states that arent even active
    public virtual void OnAnimatorUpdated()
    {
        if (_stateMachine.useRootPosition)
            _stateMachine.navAgent.velocity = _stateMachine.animator.deltaPosition / Time.deltaTime;

        if (_stateMachine.useRootRotation)
            _stateMachine.transform.rotation = _stateMachine.animator.rootRotation;
    }
    public virtual void OnAnimatorIKUpdated() { }
    public virtual void OnTriggerEvent(AITriggerEventType eventType, Collider other) { }
    public virtual void OnDesitinationReached(bool isReached) { }
    // store a reference to its parent state
    public void SetStateMachine(AIStateMachine statemachine) { statemachine = _stateMachine; }

    protected AIStateMachine _stateMachine;
}
