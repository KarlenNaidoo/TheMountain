using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour {

    public abstract AIStateType GetStateType();
    public abstract AIStateType OnUpdate();

    public virtual void OnEnterState() { }
    public virtual void OnExitState() { }
    public virtual void OnAnimatorUpdated() { } // so Unity does not call OnAnimatorMove for all states that arent even active
    public virtual void OnAnimatorIKUpdated() { }
    public virtual void OnTriggerEvent(AITriggerEventType eventType, Collider other) { }
    public virtual void OnDesitinationReached(bool isReached) { }
    // store a reference to its parent state
    public void SetStateMachine(AIStateMachine statemachine) { statemachine = _statemachine; }

    protected AIStateMachine _statemachine;
}
