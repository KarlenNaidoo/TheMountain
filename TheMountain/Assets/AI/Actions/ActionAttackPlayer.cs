using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;

public class ActionAttackPlayer : ReGoapAction<string, object>
{

    Animator anim;
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
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

    
    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        preconditions.Set("PlayerVisible", true); 
        preconditions.Set("PlayerInRange", true);
        return base.GetPreconditions(stackData);
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("Attack", true);
        return base.GetEffects(stackData);
    }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
        anim.SetTrigger("Attack");
        anim.SetInteger("AttackID", 1);
        anim.SetInteger("AttackType", 1);
        Debug.Log("Attacking Player");
        done(this);

    }
}
