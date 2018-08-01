using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;

public class ActionAttackPlayer : ReGoapAction<string, object>
{
    
    NPCProfile _npcProfile;
    Blackboard _blackboard;
    const int AGGRESSION_THRESHOLD = 4;
    protected override void Awake()
    {
        base.Awake();
        _blackboard = GetComponent<Blackboard>();
        _npcProfile = GetComponent<NPCProfile>();
       
    }

    protected override void Start()
    {
        base.Start();
        
    }
    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {

        return base.CheckProceduralCondition(stackData) && (_blackboard.aggression < AGGRESSION_THRESHOLD);
    }

    protected virtual void OnFailureMovement()
    {
        //failCallback(this);
        Debug.Log("Running fail for attack player");
        _blackboard.aggression++;
    }

    protected virtual void OnDoneMovement()
    {
        doneCallback(this);

    }

    
    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        preconditions.Set("inMeleeRange", true);
        //preconditions.Set("meleeWeaponEquipped", false);
        //preconditions.Set("shootingWeaponEquipped", false);
        //preconditions.Set("isCharging", true);
        return base.GetPreconditions(stackData);
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("playerDead", true);
        return base.GetEffects(stackData);
    }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
        
        _blackboard.SetAttackParameters(1, (int) Blackboard.AIAttackType.NoWeapon, true);

        _npcProfile.SetAsResponder();
        // OnFailureMovement();
        fail(this);

    }
}
