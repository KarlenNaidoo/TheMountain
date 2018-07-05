using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : AICharacter
{
    public override Dictionary<string, object> CreateGoalState()
    {
        Dictionary<string, object> goal = new Dictionary<string, object>();
        goal.Add("reachedDestination", true);
        return goal;
    }
}
