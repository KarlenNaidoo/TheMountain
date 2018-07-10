using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class ChasePlayerAction : ReGoapAction<string, object> {

    //  sometimes a Transform is better (moving target), sometimes you do not have one (last target position)
    //  but if you're using multi-thread approach you can't use a transform or any unity's API
    protected SmsGoTo smsGoto;
    private List<ReGoapState<string, object>> settingsList;

    protected override void Awake()
    {
        base.Awake();

        smsGoto = GetComponent<SmsGoTo>();
        settingsList = new List<ReGoapState<string, object>>();

    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {

        return base.CheckProceduralCondition(stackData) && stackData.agent.GetMemory().GetWorldState().HasKey("objectivePosition");
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
        if (stackData.goalState.HasKey("isAtPosition"))
        {
            //settings.Set("objectivePosition", stackData.goalState.Get("isAtPosition"));
            settings.Set("isAtPosition", stackData.agent.GetMemory().GetWorldState().Get("objectivePosition"));
            return base.GetSettings(stackData);
        }
        else
        {

            Debug.Log("Does not have isAtPosition key");

        }
        return new List<ReGoapState<string, object>>();

    }
    
    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData){
        preconditions.Set("isVisible", true);
        return base.GetPreconditions(stackData);
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("isAtPosition", stackData.agent.GetMemory().GetWorldState().Get("objectivePosition"));
        return base.GetEffects(stackData);
    }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
        if (settings.HasKey("isAtPosition"))
        {
            smsGoto.GoTo((Vector3)settings.Get("isAtPosition"), OnDoneMovement, OnFailureMovement);

        }

        else
        {
            failCallback(this);

        }

    }
}
