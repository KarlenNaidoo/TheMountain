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


    int waypointIndex = 0;
    Vector3 nextPosition;
    [SerializeField] int numberOfLoops = 3;
    int countLoops = 0;
    bool shouldMove = true;

    Quaternion nextRotation;
    [SerializeField] [Range(0.1f, 3f)] float distanceToTargetReached = 0.5f; 
    public bool WorkInFixedUpdate;
    public bool UseRigidBody;
    public bool UseRigidbodyVelocity;
    public float Speed;


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
        MoveToPosition();
    }
    
    void OnAnimatorMove()
    {
        transform.position = anim.rootPosition + anim.deltaPosition;
    }
    
    
    protected virtual void MoveToPosition()
    {

        // Calculate how the AI wants to move
        ai.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            transform.rotation = nextRotation;
            ai.FinalizeMovement(transform.position, nextRotation);
        }
        else
        {
            transform.rotation = anim.rootRotation * anim.deltaRotation;
        }

        Vector3 localDesiredVelocity = transform.InverseTransformVector(ai.desiredVelocity);
      
        localDesiredVelocity.y = 0;
        float angle = FindDegree(localDesiredVelocity.x, localDesiredVelocity.z);
        //Debug.Log("Angle: " + angle);
        //Debug.Log("steering: " + ai.steeringTarget);
        Quaternion lookRotation = Quaternion.LookRotation(localDesiredVelocity, Vector3.up);
        shouldMove = ai.remainingDistance > distanceToTargetReached;
       
        // Update animation parameters
        UpdateMoveAnimations(localDesiredVelocity, angle, shouldMove);

    }

    private float FindDegree(float x, float y)
    {

        float angleBetweenTwoPoints = (float)((Mathf.Atan2(x, y) / Math.PI) * 180f);
        //if (angleBetweenTwoPoints < 0) angleBetweenTwoPoints += 360f;

        return angleBetweenTwoPoints;
    }

    private void UpdateMoveAnimations(Vector3 localDesiredVelocity, float angle, bool shouldMove)
    {
        anim.SetBool("Move", shouldMove);
        anim.SetFloat("TurnAngle", angle);
        anim.SetFloat("Speed", localDesiredVelocity.magnitude);
    }


    public virtual IEnumerator SetTargetPath(List<Transform> transform, Action onDoneMovement, Action onFailureMovement, bool loopWaypoints = true )
    {
        for (int i = 0; i < numberOfLoops; i++)
        {
            if (!ai.hasPath || !ai.pathPending)  // if we don't have a path and one is not being calculated or if we reached the end and we need to loop back
            {

                location = transform[waypointIndex];
                ai.destination = location.position;
            }

            if (ai.reachedEndOfPath && loopWaypoints)
            {
                if (waypointIndex < (transform.Count - 1))
                {
                    waypointIndex++;
                }
                else
                {
                    countLoops++;
                    waypointIndex = 0;
                }
            }
            yield return null;
        }
        Debug.Log("Player went around " + countLoops + " times"); // TODO: Not looping correctly, we will go in an infinite loop and never Complete this movement
        if(!loopWaypoints)
            GoTo(onDoneMovement, onFailureMovement);


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