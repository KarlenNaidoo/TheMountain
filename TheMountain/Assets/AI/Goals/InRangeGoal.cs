using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;

public class InRangeGoal : ReGoapGoalAdvanced<string, object> {
    public GameObject target;

    protected override void Awake()
    {
        base.Awake();
        //goal.Set("GoTo" + target.gameObject.tag, true);
        goal.Set("isAtPosition", target.transform.position);
    }

    protected override void Update()
    {
        base.Update();

        goal.Set("isAtPosition", target.transform.position);

    }
    public override string ToString()
    {
        return string.Format("Goal('{0}', '{1}')", Name, target.gameObject.tag);
    }
}
