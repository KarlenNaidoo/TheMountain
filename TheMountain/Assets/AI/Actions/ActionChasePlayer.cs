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
    bool playerVisible;
    public Transform playerLocation;
    protected override void Awake()
    {
        base.Awake();

        smsGoto = GetComponent<SmsGoTo>();
        

    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        // We should not set precondition to whther the player is visible. We should check that we have confidence of the players last known location

        return base.CheckProceduralCondition(stackData) && stackData.agent.GetMemory().GetWorldState().HasKey("lastKnownPlayerPosition");
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
        if (stackData.agent.GetMemory().GetWorldState().HasKey("lastKnownPlayerPosition"))
        {

            lastKnownPlayerPosition = (Vector3)stackData.agent.GetMemory().GetWorldState().Get("lastKnownPlayerPosition");

        }
        playerVisible = (bool) stackData.agent.GetMemory().GetWorldState().Get("PlayerVisible");
        results.Add(settings.Clone());
        return results;

    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
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
        Vector3 inRangePosition = lastKnownPlayerPosition * .5f;
        smsGoto.SetTargetPath(lastKnownPlayerPosition, OnDoneMovement, OnFailureMovement);
        Debug.Log("Run chase");
        if (smsGoto.MoveToPosition() && playerVisible && Vector3.Distance(transform.position, playerLocation.position) < 4f) // TODO use a raycast to player position
        {
            done(this);
        }
        else // If we get to that position and player is not there we have failed to chase
        {
            fail(this);
        }
    }

    protected void Update()
    {
        // Move to position will only return true if it is within the destination range, then we can consider this movement done
        smsGoto.MoveToPosition();
     
    }
}
