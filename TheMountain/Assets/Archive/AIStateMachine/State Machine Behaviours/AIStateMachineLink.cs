using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachineLink : StateMachineBehaviour {

    // Store a reference to a AI state machine

    protected AIStateMachine _stateMachine;
    public AIStateMachine stateMachine { set { _stateMachine = value; } }
}
