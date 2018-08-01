using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(NavigationManager))]
public class ActionChasePlayer : ReGoapAction<string, object>
{
    protected Blackboard blackboard;
    Transform lastKnownPlayerPosition;
    bool playerVisible;
    public Transform playerLocation;
    protected override void Awake()
    {
        base.Awake();
        

        blackboard = GetComponent<Blackboard>();
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        // We should not set precondition to whther the player is visible. We should check that we have confidence of the players last known location
        
        return base.CheckProceduralCondition(stackData) && blackboard.worldState.HasKey("lastKnownPlayerPosition");
    }

    protected virtual void OnFailureMovement()
    {
        failCallback(this);
        Debug.Log("Failed action callback");
    }

    protected virtual void OnDoneMovement()
    {
        doneCallback(this);
        Debug.Log("Done action callback");
    }


    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        var results = new List<ReGoapState<string, object>>();
        if (blackboard.worldState.HasKey("lastKnownPlayerPosition"))
        {

            lastKnownPlayerPosition =  (Transform) blackboard.worldState.Get("lastKnownPlayerPosition");

        }
        if (blackboard.worldState.HasKey("playerVisible"))
        {

            playerVisible = (bool)blackboard.worldState.Get("playerVisible");
        }
        results.Add(settings.Clone());
        return results;

    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        return base.GetPreconditions(stackData);
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("inMeleeRange", true);
        effects.Set("inShootingRange", true);
        return base.GetEffects(stackData);
    }

    
    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
      
        Vector3 inRangePosition = lastKnownPlayerPosition.position * .5f;
        
        blackboard.currentTarget = lastKnownPlayerPosition.position;
        if (blackboard.targetReachedStatus)
        {
            Debug.Log("Chase success");
            done(this);
        }
        fail(this);
    }
    
}
