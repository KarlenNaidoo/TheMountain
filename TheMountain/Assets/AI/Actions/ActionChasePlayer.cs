using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;


public class ActionChasePlayer : ReGoapAction<string, object>
{
    protected SmsGoTo smsGoto;
    Vector3 lastKnownPlayerPosition;

    protected override void Awake()
    {
        base.Awake();

        smsGoto = GetComponent<SmsGoTo>();

    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {

        return base.CheckProceduralCondition(stackData);
    }

    protected virtual void OnFailureMovement()
    {
        failCallback(this);
    }

    protected virtual void OnDoneMovement()
    {
        doneCallback(this);

    }


    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        var results = new List<ReGoapState<string, object>>();
        if (stackData.agent.GetMemory().GetWorldState().HasKey("lastKnownPlayerPosition"))
        {

            lastKnownPlayerPosition = (Vector3)stackData.agent.GetMemory().GetWorldState().Get("lastKnownPlayerPosition");

        }
        results.Add(settings.Clone());
        return results;
        //return base.GetSettings(stackData);

    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        preconditions.Set("PlayerVisible", true);
        return base.GetPreconditions(stackData);
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("PlayerInRange", true);
        return base.GetEffects(stackData);
    }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
        smsGoto.SetTargetPath(lastKnownPlayerPosition, OnDoneMovement, OnFailureMovement);
        OnDoneMovement();

    }
}
