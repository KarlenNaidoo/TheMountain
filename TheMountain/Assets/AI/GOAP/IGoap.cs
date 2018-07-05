using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Any agent that wants to use Goal Oritented Action Planning must implement this interface.
 * It provides information to the GOAP planner so it can plan what actions to use.
 * 
 * It also proves an interface for the planner to give feedback to the Agent and report success/failure
 */ 
public interface IGoap {

    // The starting state of the Agent and the world. Supply what states are needed for the action to run
    Dictionary<string, object> GetWorldState();

    // Give the planner a new goal so it can figure out the actions needed to fufill it
    Dictionary<string, object> CreateGoalState();

    // No sequence of actions could be found for the supplied goal. You will need to try another goal
    void PlanFailed(Dictionary<string, object> failedGoal);

    // A plan was found for the supplied goal. These are the actions the Agent will perform, in order.
    void PlanFound(Dictionary<string, object> goal, Queue<GoapAction> actions);

    void actionsFinished();

    // One of the actions caused the plan to abort. That action is returned
    void PlanAborted(GoapAction aborter);

    /**
	 * Called during Update. Move the agent towards the target in order
	 * for the next action to be able to perform.
	 * Return true if the Agent is at the target and the next action can perform.
	 * False if it is not there yet.
	 */
    bool MoveAgent(GoapAction nextAction);
}
