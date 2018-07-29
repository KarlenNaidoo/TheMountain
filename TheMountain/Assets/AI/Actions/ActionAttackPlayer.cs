using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;

public class ActionAttackPlayer : ReGoapAction<string, object>
{

    Animator _anim;
    Blackboard _blackboard;
    protected override void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
        _blackboard = GetComponent<Blackboard>();

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
        _blackboard.SetAttackParameters(1, (int) Blackboard.AIAttackType.NoWeapon, true);
        Debug.Log("Attacking Player");
        done(this);

    }
}
