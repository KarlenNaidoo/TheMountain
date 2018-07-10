using ReGoap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWaypointsGoal : ReGoapGoalAdvanced<string, object>
{
    protected override void Awake()
    {
        base.Awake();
        goal.Set("patrol", true);
        //goal.Set("visited", false);
    }
    
    public override string ToString()
    {
        return string.Format("Goal('{0}')", Name);
    }
}