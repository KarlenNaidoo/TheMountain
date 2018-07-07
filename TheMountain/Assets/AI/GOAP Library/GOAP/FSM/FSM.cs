using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Stack - based finite state machine
 * Push and pop states to the FSM
 * 
 * States should push other states onto the stack
 * and pop themselves off
 */ 

public class FSM
{

    public delegate void FSMState(FSM fsm, GameObject gameObject);

    private Stack<FSMState> stateStack = new Stack<FSMState>();

    public void Update(GameObject gameObject)
    {
        if(stateStack.Peek() != null)
        {
            stateStack.Peek().Invoke(this, gameObject);
        }
    }

    public void PushState(FSMState state)
    {
        stateStack.Push(state);
    }

    public void PopState()
    {
        stateStack.Pop();
    }
}
