﻿using UnityEngine;
using System.Collections;
using ReGoap.Unity;

public class GoalAttackPlayer : ReGoapGoalAdvanced<string, object>
{

    protected override void Awake()
    {
        base.Awake();
        //goal.Set("GoTo" + target.gameObject.tag, true);
        goal.Set("Attack", true);
    }

    public override string ToString()
    {
        return string.Format("Goal('{0}')", Name);
    }
}
