using UnityEngine;
using System.Collections;
using ReGoap.Unity;
using ReGoap.Core;
using System.Collections.Generic;
using System;
using System.Linq;


[RequireComponent(typeof(NavigationManager))]
public class ActionPatrolWaypoints : ReGoapAction<string, object>
{

    //  sometimes a Transform is better (moving target), sometimes you do not have one (last target position)
    //  but if you're using multi-thread approach you can't use a transform or any unity's API
    protected NavigationManager navManager;
    protected Blackboard blackboard;
    [SerializeField] bool continuouslyLoopWaypoints;
    [SerializeField] [Range(0,20)] int numberOfLoops = 3;

    private int _waypointIndex = 0;
    private int _waypointsVisited = 0;
    private int _circuitComplete = 0;
    private Transform _location;
    private bool _setOncePerUpdate = true;
    protected override void Awake()
    {
        base.Awake();
        navManager = GetComponent<NavigationManager>();
        blackboard = GetComponent<Blackboard>();

    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {

        List<Transform> patrolPoints = (List<Transform>)blackboard.worldState.Get("patrolDestinations");
        return base.CheckProceduralCondition(stackData) && (patrolPoints.Count > 0); // Proceed only if we have at least one waypoint
    }

    protected virtual void OnFailureMovement()
    {
        failCallback(this);
    }

    protected virtual void OnDoneMovement()
    {
        settings.Set("VisitedAllWaypoints", true);
        doneCallback(this);

    }
    

    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        List<Transform> patrolPoints = (List<Transform>) blackboard.worldState.Get("patrolDestinations");
        Vector3 destination = patrolPoints.First().position;
        settings.Set("patrolDestination", patrolPoints);

        return base.GetSettings(stackData);
        
    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
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
        List<Transform> patrolDestinations = (List<Transform>)settings.Get("patrolDestination");
        if (settings.HasKey("patrolDestination"))
        {
            blackboard.onDoneMovement = OnDoneMovement;
            blackboard.onFailureMovement = OnFailureMovement;
            
            blackboard.currentTarget = patrolDestinations[_waypointIndex];

            //Debug.Log("Setting currentTarget to: " + blackboard.currentTarget.position);

            if (blackboard.targetReachedStatus && _setOncePerUpdate)
            {

                //Debug.Log("Reached waypoint, time to plot another");
                if (_waypointIndex < (patrolDestinations.Count - 1))
                {
                    _waypointIndex++;
                }
                else
                {
                    _waypointIndex = 0;
                }
                _setOncePerUpdate = false;
            }
            else
            {
                fail(this);
                _setOncePerUpdate = true;
            }
           // done(this);
        }
        else
        {
            OnFailureMovement();
        }

    }

}
