using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;

public class LocatePlayerGoal : ReGoapGoal<string, object> {
    public string targetName;

    protected override void Awake()
    {
        base.Awake();
        goal.Set("locate" + targetName, true);
    }

    public override string ToString()
    {
        return string.Format("GoapGoal('{0}', '{1}')", Name, targetName);
    }
}
