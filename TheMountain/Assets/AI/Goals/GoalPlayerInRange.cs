using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;

public class GoalPlayerInRange : ReGoapGoalAdvanced<string, object> {

    protected override void Awake()
    {
        base.Awake();
        //goal.Set("GoTo" + target.gameObject.tag, true);
        goal.Set("PlayerInRange", true);
    }
    
    public override string ToString()
    {
        return string.Format("Goal('{0}')", Name);
    }
}
