﻿using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;

public class PatrolWaypointsAction : ReGoapAction<string, object>
{

    //  sometimes a Transform is better (moving target), sometimes you do not have one (last target position)
    //  but if you're using multi-thread approach you can't use a transform or any unity's API
    protected SmsGoTo smsGoto;

    protected override void Awake()
    {
        base.Awake();

        smsGoto = GetComponent<SmsGoTo>();

    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        
        return base.CheckProceduralCondition(stackData); // Don't proceed if we dont have at least one waypoint
    }

    protected virtual void OnFailureMovement()
    {
        failCallback(this);
    }

    protected virtual void OnDoneMovement()
    {
        Debug.Log("This movement is done. Move destination to last point");
        doneCallback(this);

    }
    

    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        List<Transform> patrolPoints = (List<Transform>) stackData.agent.GetMemory().GetWorldState().Get("patrolDestinations");
        Vector3 destination = patrolPoints.First().position;
        settings.Set("patrolDestination", patrolPoints);

        return base.GetSettings(stackData);
        
    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        //preconditions.Set("visited", false);
        return base.GetPreconditions(stackData);
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("patrol", true);
        return base.GetEffects(stackData);
    }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
        if (settings.HasKey("patrolDestination"))
        {
            //StartCoroutine(smsGoto.GoTo((List<Transform>)settings.Get("patrolDestination"), OnDoneMovement, OnFailureMovement));
            smsGoto.GoTo((List<Transform>)settings.Get("patrolDestination"), OnDoneMovement, OnFailureMovement);

        }

        else
        {
            failCallback(this);

        }

    }
}
