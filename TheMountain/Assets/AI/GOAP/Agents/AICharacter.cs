using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

/**
 * A general AI class.
 * Subclass this for specific Character classes and implement the createGoalState() method that will populate the goal for the GOAP planner.
 */ 
public abstract class AICharacter : MonoBehaviour, IGoap
{
    public float moveSpeed = 2f;

    public bool reachedWayPoint = false;
    NavMeshAgent _navAgent;
    public void actionsFinished()
    {
        // everything is done, we completed our actions for this goal
        Debug.Log("ACTIONS COMPLETED");
    }

    public abstract Dictionary<string, object> CreateGoalState();

    // Dictionary that will feed the GOAP actions and system while planning
    public Dictionary<string, object> GetWorldState()
    {
        Dictionary<string, object> worldData = new Dictionary<string, object>();
        worldData.Add("reachWayPoint1", false);
        worldData.Add("reachWayPoint2", false);
        worldData.Add("allWaypoints", false);
        return worldData;

    }

    public void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();  
    }
    public bool MoveAgent(GoapAction nextAction)
    {
        // move towards the Next actions target
        float step = moveSpeed * Time.deltaTime;
        _navAgent.SetDestination(nextAction.target.transform.position);

        if ((gameObject.transform.position - nextAction.target.transform.position).magnitude < 2f)
        {
            //we are at the target location, we are done
            nextAction.inRange = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlanAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("PLAN ABORTED");
    }

    public void PlanFailed(Dictionary<string, object> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void PlanFound(Dictionary<string, object> goal, Queue<GoapAction> actions)
    {
        Debug.Log("PLAN FOUND " + GoapAgent.prettyPrint(actions));
    }
}
