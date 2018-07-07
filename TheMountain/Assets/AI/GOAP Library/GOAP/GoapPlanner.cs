using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Plans what actions can be completed in order to fufill a goal state
 */
public class GoapPlanner
{
    /**
     * Plan what sequence of actions can fufill the goal.
     * Returns null if a plan could not be found, or a list of actions that must be performed in order to fufill the goal
     */ 
    public Queue<GoapAction> Plan(GameObject agent, List<GoapAction> availableActions, Dictionary<string, object> worldState, Dictionary<string, object> goal)
    {
        // reset the actions so we can start fresh 
        foreach(GoapAction action in availableActions)
        {
            action.Reset();
        }

        // check what actions can run using their CheckProceduralPreCondition
        List<GoapAction> usableActions = new List<GoapAction>();
        foreach (GoapAction action in availableActions)
        {
            if (action.CheckProceduralPreCondtions(agent))
            {
                //Debug.Log("Action found: " + action); 
                usableActions.Add(action); // We know have all the actions that can run
            }
        }

        // build up a tree and record the lead nodes that provide a solution to the goal.
        List<Node> leaves = new List<Node>();

        // build graph
        Node start = new Node(null, 0, worldState, null);
        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            Debug.Log("GoapPlanner could not find a plan");
            return null;
        }

        // get the cheapest leaf
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else
            {
                if (leaf.runningCost < cheapest.runningCost)
                {
                    cheapest = leaf;
                }
            }
        }

        // get its node and work back through the parents
        List<GoapAction> result = new List<GoapAction>();
        Node n = cheapest;
        while (n!= null)
        {
            if(n.action != null)
            {
                result.Insert(0, n.action); // insert the action in the front
            }
            n = n.parent;
        } // we now have the action list in the correct order

        Queue<GoapAction> plan = new Queue<GoapAction>();
        foreach (GoapAction action in result)
        {
            plan.Enqueue(action);
        }

        // we have a plan
        return plan;

        
    }

    /** Returns true if at least one solution was found
     *  The possible paths are stored in the leaves list. Each leaf has a 'runningCost' value where the lowest cost will be the best action sequence
     */
     private bool BuildGraph(Node parent, List<Node> leaves, List<GoapAction> usableActions, Dictionary<string, object> goal)
    {
        bool foundOne = false;

        // go through each action available at this node and see if we can use it here
        foreach (GoapAction action in usableActions)
        {
            Debug.Log("action in use: " + action);
            //if the parent state has the conditions for this action's preconditions, we can use it here
            if (InState(action.preConditions, parent.state))
            {
                // apply the action's effects to the parent state
                Dictionary<string, object> currentState = populateState(parent.state, action.effects);
                Node node = new Node(parent, parent.runningCost + action._cost, currentState, action);

                if(InState(goal, currentState))
                {
                    // we found a solution
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    //not at a solution yet, so test all the remaining actions and branch out the tree
                    List<GoapAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }

        return foundOne;
    }

    /** 
     * Creates a subset of the actions excluding the removeMe one. Creates a new set
     */
     private List<GoapAction> ActionSubset(List<GoapAction> actions, GoapAction removeMe)
    {
        List<GoapAction> subset = new List<GoapAction>();
        foreach (GoapAction action in actions)
        {
            if (!action.Equals(removeMe))
            {
                subset.Add(action);
            }
        }
        return subset;
    }

    /**
     * Check that all items in 'test' are in 'state'. If just one does not match or is not there then this returns false.
     */
     private bool InState(Dictionary<string, object> dic1, Dictionary<string, object> dic2)
    {

        bool allMatch = true;
        foreach (var item in dic1)
        {
            bool match = false;
            foreach (var obj in dic2)
            {
                if (item.Key.Equals(obj.Key) && item.Value.Equals(obj.Value))
                {
                    match = true;
                    break;
                }
            }

            if (!match)
                allMatch = false;
        }
        return allMatch;
    }

    private Dictionary<string, object> populateState(Dictionary<string, object> currentState, Dictionary<string, object> stateChange)
    {
        Dictionary<string, object> state = new Dictionary<string, object>();
        state = currentState;
        foreach (var change in stateChange.Keys)
        {
            bool exists = false;
            foreach (var s in state.Keys)
            {
                if (s.Equals(change))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                state.Remove(change); // Remove old state value and replace with the statechange value
                object changedValue;
                stateChange.TryGetValue(change, out changedValue);
                state.Add(change, changedValue);
            }
            else // Add without removing
            {
                object changedValue;
                stateChange.TryGetValue(change, out changedValue);
                state.Add(change, changedValue);
            }
        }
        return state;
    }

    private class Node
    {
        public Node parent;
        public float runningCost;
        public Dictionary<string, object> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, Dictionary<string, object> state, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
    
}
