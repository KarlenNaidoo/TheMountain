using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public sealed class GoapAgent : MonoBehaviour
{
    private FSM stateMachine;
    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    private List<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

    private GoapPlanner planner;

    private void Awake()
    {
        stateMachine = new FSM();
        availableActions = new List<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        FindDataProvider();
        CreateIdleState();
        CreateMoveToState();
        CreatePerformActionState();
        stateMachine.PushState(idleState);
        LoadActions();

    }

    private void Update()
    {
        stateMachine.Update(this.gameObject);
    }

    public void AddAction(GoapAction action)
    {
        availableActions.Add(action);
    }

    public GoapAction GetAction (Type action)
    {
        return availableActions.Find(obj => obj.name.Equals(action));
    }

    public void RemoveAction(GoapAction action)
    {
        availableActions.Remove(action);
    }

    private bool HasActionPlan()
    {
        return currentActions.Count > 0;
    }

    private void CreateIdleState()
    {
        idleState = (fsm, gameObj) =>
        {
            // GOAP planning

            // get the world state and the goal we want to plan for
            Dictionary<string, object> worldState = dataProvider.GetWorldState();
            Dictionary<string, object> goal = dataProvider.CreateGoalState();

            // Plan
            Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal); // Being passed the right parameters
            /**
            foreach (var item in availableActions)
            {
                Debug.Log("Available action: " + item);
            }
            foreach (var item in worldState)
            {
                Debug.Log("World state key:" + item.Key + " Value: " + item.Value.ToString());
            }
            foreach (var item in goal)
            {
                Debug.Log("Goal key:" + item.Key + " Value: " + item.Value.ToString());
            }
            */
            if (plan != null)
            {
                currentActions = plan;
                dataProvider.PlanFound(goal, plan);
                fsm.PopState(); // move to the Perform action state
                fsm.PushState(performActionState);
            }
            else
            {
                Debug.Log("GoapAgent Failed Plan");
                dataProvider.PlanFailed(goal);
                fsm.PopState(); // move back to the idle state
                fsm.PushState(idleState);
            }
        };
    }

    private void CreateMoveToState()
    {
        moveToState = (fsm, gameObj) =>
        {
            // move the gameobject
            GoapAction action = currentActions.Peek();
            if (action.RequiresInRange() && action.target == null)
            {
                Debug.Log("Action requires a target but has none. Planning failed");
                fsm.PopState(); // move
                fsm.PopState(); // perform
                fsm.PushState(idleState);
                return;
            }

            // get the agent to move itself
            if (dataProvider.MoveAgent(action))
            {
                fsm.PopState();
            }


            /*MovableComponent movable = (MovableComponent) gameObj.GetComponent(typeof(MovableComponent));
			if (movable == null) {
				Debug.Log("<color=red>Fatal error:</color> Trying to move an Agent that doesn't have a MovableComponent. Please give it one.");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			float step = movable.moveSpeed * Time.deltaTime;
			gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);

			if (gameObj.transform.position.Equals(action.target.transform.position) ) {
				// we are at the target location, we are done
				action.setInRange(true);
				fsm.popState();
			}*/
        };
    }

    private void CreatePerformActionState()
    {
        performActionState = (fsm, gameObj) =>
        {
            // perform the action
            if (!HasActionPlan())
            {
                // no actions to perform
                Debug.Log("Done Actions");
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            GoapAction action = currentActions.Peek(); // returns the first item in the queue without removing it
            if (action.IsDone())
            {
                Debug.Log("Action done, removing action so we can perform the next one");
                // the action is done. Remove it so we can perform the next one
                currentActions.Dequeue();
            }

            if (HasActionPlan())
            {

                action = currentActions.Peek();
                bool inRange = action.RequiresInRange() ? action.inRange : true;

                if (inRange)
                {
                    
                    // we are in range, so perform the action
                    bool success = action.Perform(gameObj);

                    if (!success)
                    {
                        
                        fsm.PopState();
                        fsm.PushState(idleState);
                        dataProvider.PlanAborted(action);
                    }
                }
                else
                {
                    
                    // we need to move there first
                    // push moveTo state
                    fsm.PushState(moveToState);
                }
            }
            else
            {
                // no actions left, move to plan state

                Debug.Log("No actions left, moving to plan state");
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.actionsFinished();
            }
        };
    }

    private void FindDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void LoadActions()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions)
        {
            availableActions.Add(a);
            Debug.Log("Load actions: " + a);
        }

    }

    public static string prettyPrint(Queue<GoapAction> actions)
    {
        String s = "";
        foreach (GoapAction a in actions)
        {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

}
