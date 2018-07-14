using com.ootii.Geometry;
using Pathfinding;
using ReGoap.Unity.FSM;
using ReGoap.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// generic goto state, can be used in most games, override Tick and Enter if you are using
//  a navmesh / pathfinding library
//  (ex. tell the library to search a path in Enter, when done move to the next waypoint in Tick)

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(SmsIdle))]
[RequireComponent(typeof(RichAI))]
public class SmsGoTo : SmState
{
    private Vector3? objective;
    private Transform objectiveTransform;
    private Action onDoneMovementCallback;
    private Action onFailureMovementCallback;

    // Pathfinding
    private RichAI ai;

    private float smoothAngle;

    private enum GoToState
    {
        Disabled, Pulsed, Active, Success, Failure
    }

    private GoToState currentState;
    private Rigidbody body;
    private Animator anim;
    private Transform location;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;


    int waypointNumber = 0;
    Vector3 nextPosition;
    
    Quaternion nextRotation;
    [SerializeField] [Range(0.1f, 3f)] float distanceToTargetReached = 0.5f; 
    public bool WorkInFixedUpdate;
    public bool UseRigidBody;
    public bool UseRigidbodyVelocity;
    public float Speed;


    bool searchForNewPath = true;
    // when the magnitude of the difference between the objective and self is <= of this then we're done
    public float MinPowDistanceToObjective = 0.5f;

    // additional feature, check for stuck, userful when using rigidbody or raycasts for movements
    private Vector3 lastStuckCheckUpdatePosition;

    private float stuckCheckCooldown;
    public bool CheckForStuck;
    public float StuckCheckDelay = 1f;
    public float MaxStuckDistance = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        ai = GetComponent<RichAI>();
        if (UseRigidBody)
        {

            body = GetComponentInChildren<Rigidbody>();
        }

        // Disable the AIs own movement code
        ai.updatePosition = false;
        ai.canMove = false;
        anim = GetComponent<Animator>();
    }

    // if your games handle the speed from something else (ex. stats class) you can override this function
    protected virtual float GetSpeed()
    {
        return Speed;
    }
    

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!WorkInFixedUpdate) return;
        Tick();
        
    }

    protected override void Update()
    {
        base.Update();
        if (WorkInFixedUpdate) return;
        Tick();

    }

    // if you're using an animation just override this, call base function (base.Tick()) and then
    //  set the animator variables (if you want to use root motion then also override MoveTo)
    protected virtual void Tick()
    {
        var objectivePosition = objectiveTransform != null ? objectiveTransform.position : objective.GetValueOrDefault();
        MoveTo();
    }
    
    void OnAnimatorMove()
    {
        transform.position = anim.rootPosition + anim.deltaPosition;
    }
    
    
    protected virtual void MoveTo()
    {
        // Calculate how the AI wants to move
        ai.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
        //transform.rotation = nextRotation;
        ai.FinalizeMovement(transform.position, nextRotation);
        Vector3 localDesiredVelocity = transform.InverseTransformVector(ai.desiredVelocity);
        float angle = Mathf.Atan2(localDesiredVelocity.x, localDesiredVelocity.z) * Mathf.Rad2Deg;
        localDesiredVelocity.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(localDesiredVelocity, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);

        bool shouldMove = ai.remainingDistance > distanceToTargetReached;
        searchForNewPath = !shouldMove;
        Debug.Log("Search: " + searchForNewPath);
        // Update animation parameters
        anim.SetBool("Move", shouldMove);
        anim.SetFloat("TurnAngle", angle);
        anim.SetFloat("Speed", 1);
  
    }



    public virtual IEnumerator GoTo(List<Transform> transform, Action onDoneMovement, Action onFailureMovement)
    {
        while (true)
        {
            if (!ai.hasPath && !ai.pathPending || ai.reachedEndOfPath) // if we don't have a path and one is not being calculated
            {

                location = transform[waypointNumber];
                ai.destination = location.position;
                Debug.Log("Destination: " + ai.destination);
                ai.SearchPath();
                searchForNewPath = false;
                if(waypointNumber < transform.Count - 1)
                {
                    waypointNumber++;
                }
                else
                {
                    waypointNumber = transform.Count - 1;
                }
            }
            if (searchForNewPath)
            {
                location = transform[waypointNumber];
                ai.destination = location.position;
                Debug.Log("Destination: " + ai.destination);
                ai.SearchPath();
                if (waypointNumber < transform.Count -1)
                {
                    waypointNumber++;
                }
                else
                {
                    waypointNumber = transform.Count - 1;
                }

            }
            yield return null;
            GoTo(onDoneMovement, onFailureMovement);
        }
    }

    protected virtual void MoveTo(Vector3 position)
    {
        var delta = position - transform.position;
        var movement = delta.normalized * GetSpeed();
        if (UseRigidBody)
        {
            if (UseRigidbodyVelocity)
            {
                body.velocity = movement;
            }
            else
            {
                body.MovePosition(transform.position + movement * Time.deltaTime);
            }
        }
        else
        {
            transform.position += movement * Time.deltaTime;
        }
        if (delta.sqrMagnitude <= MinPowDistanceToObjective)
        {
            currentState = GoToState.Success;
        }
        if (CheckForStuck && CheckIfStuck())
        {
            currentState = GoToState.Failure;
        }
    }

    private bool CheckIfStuck()
    {
        if (Time.time > stuckCheckCooldown)
        {
            stuckCheckCooldown = Time.time + StuckCheckDelay;
            if ((lastStuckCheckUpdatePosition - transform.position).magnitude < MaxStuckDistance)
            {
                ReGoapLogger.Log("[SmsGoTo] '" + name + "' is stuck.");
                return true;
            }
            lastStuckCheckUpdatePosition = transform.position;
        }
        return false;
    }
    
    #region StateHandler

    public override void Init(StateMachine stateMachine)
    {
        base.Init(stateMachine);
        var transition = new SmTransition(GetPriority(), Transition);
        var doneTransition = new SmTransition(GetPriority(), DoneTransition);
        stateMachine.GetComponent<SmsIdle>().Transitions.Add(transition);
        Transitions.Add(doneTransition);
    }

    private Type DoneTransition(ISmState state)
    {
        if (currentState != GoToState.Active)
            return typeof(SmsIdle);
        return null;
    }

    private Type Transition(ISmState state)
    {
        if (currentState == GoToState.Pulsed)
            return typeof(SmsGoTo);
        return null;
    }

    public void GoTo(Vector3? position, Action onDoneMovement, Action onFailureMovement)
    {
        objective = position;
        GoTo(onDoneMovement, onFailureMovement);
    }

    public void GoTo(Transform transform, Action onDoneMovement, Action onFailureMovement)
    {
        objectiveTransform = transform;
        GoTo(onDoneMovement, onFailureMovement);
    }

    private void GoTo(Action onDoneMovement, Action onFailureMovement)
    {
        currentState = GoToState.Pulsed;
        onDoneMovementCallback = onDoneMovement;
        onFailureMovementCallback = onFailureMovement;
    }

    public override void Enter()
    {
        base.Enter();
        currentState = GoToState.Active;
    }

    public override void Exit()
    {
        base.Exit();
        if (currentState == GoToState.Success)
            onDoneMovementCallback();
        else
            onFailureMovementCallback();
    }

    #endregion StateHandler
}