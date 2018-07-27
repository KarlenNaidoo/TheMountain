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
    protected int _rootPositionRefCount = 0;
    protected int _rootRotationRefCount = 0;

    public bool useRootPosition { get { return _rootPositionRefCount > 0; } }
    public bool useRootRotation { get { return _rootRotationRefCount > 0; } }

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
    [SerializeField] float _slerpSpeed = 5f;
    

    int waypointIndex = 0;
    Vector3 nextPosition;
    int waypointsVisited = 0;
    int circuitComplete = 0;
    bool shouldMove = true;

    Quaternion nextRotation;
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
    private Vector3 localDesiredVelocity;
    private float angle;

    protected override void Awake()
    {
        base.Awake();
        ai = GetComponent<RichAI>();
        anim = GetComponent<Animator>();

        if (UseRigidBody)
        {

            body = GetComponentInChildren<Rigidbody>();
        }

    }

    protected override void Start()
    {
        base.Start();


        // Disable the AIs own movement code
        ai.canMove = false;

        if (anim)
        {
            RootMotionConfigurator[] behaviourScripts = anim.GetBehaviours<RootMotionConfigurator>();
            foreach (RootMotionConfigurator script in behaviourScripts)
            {
                script.smsGoTo = this;
            }
        }
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
       // MoveToPosition();
    }
    
    
    
    public virtual bool MoveToPosition()
    {

        // Calculate how the AI wants to move
        ai.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);

        localDesiredVelocity = transform.InverseTransformDirection(ai.desiredVelocity);

        localDesiredVelocity.y = 0; // we don't want our player floating so ignore y axis

        if (!useRootPosition)
        {
            ai.FinalizeMovement(transform.position, nextRotation);
       
        }
        else
        {
            transform.position = anim.rootPosition;
        }
        if (!useRootRotation)
        {
            transform.rotation = nextRotation;
        }
        else
        {
            transform.rotation = anim.rootRotation;
        }

        angle = Player.Utility.FindSignedAngle(transform.forward, (ai.steeringTarget - transform.position));
        
        EnableMovement();
        // Update animation parameters
        UpdateMoveAnimations(localDesiredVelocity, angle, shouldMove);
        if (Vector3.Distance(ai.destination, transform.position) <= ai.endReachedDistance)
        {
            DisableMovement();
            Debug.Log("Reached Objective");
            return true;
        }

        return false;
        
    }


    private void UpdateMoveAnimations(Vector3 localDesiredVelocity, float angle, bool shouldMove)
    {
        anim.SetBool("Move", shouldMove);
        anim.SetFloat("TurnAngle", angle);
        anim.SetFloat("Speed", localDesiredVelocity.magnitude);
    }

    public virtual void SetTargetPath(Vector3 position, Action onDoneMovement, Action onFailureMovement)
    {
        if (!ai.hasPath || !ai.pathPending)  // if we don't have a path or one is not being calculated 
        {
            // Set destination and move to it
            ai.destination = position;

        }
        //GoTo(onDoneMovement, onFailureMovement); // will always default to failure because we are not calling the Exit function. Not making use of the GOTO statemachine
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, ai.destination);
    }

    public virtual IEnumerator SetTargetPath(List<Transform> transform, Action onDoneMovement, Action onFailureMovement, int numberOfLoops, bool loopWaypoints = true )
    {
        while(circuitComplete < numberOfLoops || loopWaypoints) // If we set a predefined loop this will run or if we want it to run forever
        {

            if (!ai.hasPath || !ai.hasPath)  // if we don't have a path or one is not being calculated 
            {
                // Set destination and move to it
                location = transform[waypointIndex];
                ai.destination = location.position;
         
            }
            if (ai.hasPath)
            {

                MoveToPosition();
            }


            //We've reached our waypoint, time to move on
            if (Vector3.Distance(this.transform.position, ai.destination) < ai.endReachedDistance)
            {
                
                waypointsVisited++;
                // If we are still less than the total number of waypoints then increase the count, if not, loop back around
                if (waypointIndex < (transform.Count - 1))
                {
                    waypointIndex++;
                }
                else
                {
                    waypointIndex = 0;
                }
                location = transform[waypointIndex];
                ai.destination = location.position;

            }

            // if we reached the last waypoint we have completed one loop around the circuit. Last waypoint found by comparing to length of waypoint array
            if (waypointsVisited == transform.Count)
            {
                circuitComplete++;
                waypointsVisited = 0;
            }
            yield return null;
           
        }
        if(!loopWaypoints || circuitComplete >= numberOfLoops)
        {
            DisableMovement();
            UpdateMoveAnimations(localDesiredVelocity, angle, shouldMove);
            GoTo(onDoneMovement, onFailureMovement);
            
        }
    }

    private void DisableMovement()
    {
        shouldMove = false;
        angle = 0f;
        localDesiredVelocity = Vector3.zero;

        UpdateMoveAnimations(localDesiredVelocity, angle, shouldMove);
    }

    private void EnableMovement()
    {
        shouldMove = true;
    }


    public void AddRootMotionRequest(int rootPosition, int rootRotation)
    {

        _rootPositionRefCount = rootPosition;
        _rootRotationRefCount = rootRotation;
    }

    protected virtual void MoveTo(Vector3 position)
    {
        var delta = position - transform.position;
        var movement = delta.normalized * Speed;
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
    
}