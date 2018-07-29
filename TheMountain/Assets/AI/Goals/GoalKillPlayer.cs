using UnityEngine;
using System.Collections;
using ReGoap.Unity;

public class GoalKillPlayer : ReGoapGoalAdvanced<string, object>
{

    protected override void Awake()
    {
        base.Awake();
        //goal.Set("GoTo" + target.gameObject.tag, true);
        goal.Set("PlayerDead", true);
    }

    public override string ToString()
    {
        return string.Format("Goal('{0}')", Name);
    }
}

