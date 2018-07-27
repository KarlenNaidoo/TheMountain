using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RichAI))]
public class NavigationManager : MonoBehaviour
{
    private RichAI _ai;
    private Animator _anim;
    private Blackboard _blackboard;
    private Vector3 _nextPosition;
    private Quaternion _nextRotation;
    private Vector3 _localDesiredVelocity;
    private float _angle;
    private bool _shouldMove = true;
    private int _waypointIndex = 0;
    private int _waypointsVisited = 0;
    private int _circuitComplete = 0;
    private Transform _location;
    private Action _onDoneMovementCallback;
    private Action _onFailureMovementCallback;
   
    protected int _rootPositionRefCount = 0;
    protected int _rootRotationRefCount = 0;

    public bool useRootPosition { get { return _rootPositionRefCount > 0; } }
    public bool useRootRotation { get { return _rootRotationRefCount > 0; } }


    protected void Awake()
    {
        _ai = GetComponent<RichAI>();
        _anim = GetComponent<Animator>();
        _blackboard = GetComponent<Blackboard>();
    }

    protected void Start()
    {
        // Disable the _ais own movement code
        _ai.canMove = false;

        if (_anim)
        {
            RootMotionConfigurator[] behaviourScripts = _anim.GetBehaviours<RootMotionConfigurator>();
            foreach (RootMotionConfigurator script in behaviourScripts)
            {
                script.navManager = this;
            }
        }
    }


    protected void Update()
    {
        if (_blackboard.currentTarget)
        {
            SetTargetPath(_blackboard.currentTarget.position, _blackboard.onDoneMovement, _blackboard.onFailureMovement);
        }
        //StartCoroutine(SetTargetPath(_blackboard.listOfTargets, _blackboard.onDoneMovement, _blackboard.onFailureMovement, _blackboard.numberOfLoops, _blackboard.continuouslyLoopWaypoints));
     
        MoveToPosition();
    }
    public void AddRootMotionRequest(int rootPosition, int rootRotation)
    {

        _rootPositionRefCount = rootPosition;
        _rootRotationRefCount = rootRotation;
    }



    public virtual bool MoveToPosition()
    {
        _blackboard.targetVisitedStatus = false;
        // Calculate how the _ai wants to move
        _ai.MovementUpdate(Time.deltaTime, out _nextPosition, out _nextRotation);

        _localDesiredVelocity = transform.InverseTransformDirection(_ai.desiredVelocity);

        _localDesiredVelocity.y = 0; // we don't want our player floating so ignore y axis

        if (!useRootPosition)
        {
            _ai.FinalizeMovement(transform.position, _nextRotation);

        }
        else
        {
            transform.position = _anim.rootPosition;
        }
        if (!useRootRotation)
        {
            transform.rotation = _nextRotation;
        }
        else
        {
            transform.rotation = _anim.rootRotation;
        }

        _angle = Player.Utility.FindSignedAngle(transform.forward, (_ai.steeringTarget - transform.position));

        EnableMovement();
        // Update animation parameters
        UpdateMoveAnimations(_localDesiredVelocity, _angle, _shouldMove);
        if (Vector3.Distance(_ai.destination, transform.position) <= _ai.endReachedDistance)
        {
            _blackboard.targetVisitedStatus = true;
            DisableMovement();
            Debug.Log("Reached Objective");
            return true;
        }

        return false;

    }


    public virtual IEnumerator SetTargetPath(List<Transform> transform, Action onDoneMovement, Action onFailureMovement, int numberOfLoops, bool loopWaypoints = true)
    {

        while (_circuitComplete < numberOfLoops || loopWaypoints) // If we set a predefined loop this will run or if we want it to run forever
        {

            if (!_ai.hasPath || !_ai.hasPath)  // if we don't have a path or one is not being calculated 
            {
                // Set destination and move to it
                _location = transform[_waypointIndex];
                _ai.destination = _location.position;

            }
            if (_ai.hasPath)
            {

                MoveToPosition();
            }


            //We've reached our waypoint, time to move on
            if (Vector3.Distance(this.transform.position, _ai.destination) < _ai.endReachedDistance)
            {

                _waypointsVisited++;
                // If we are still less than the total number of waypoints then increase the count, if not, loop back around
                if (_waypointIndex < (transform.Count - 1))
                {
                    _waypointIndex++;
                }
                else
                {
                    _waypointIndex = 0;
                }
                _location = transform[_waypointIndex];
                _ai.destination = _location.position;

            }

            // if we reached the last waypoint we have completed one loop around the circuit. Last waypoint found by comparing to length of waypoint array
            if (_waypointsVisited == transform.Count)
            {
                _circuitComplete++;
                _waypointsVisited = 0;
            }
            yield return null;

        }
        if (!loopWaypoints || _circuitComplete >= numberOfLoops)
        {
            DisableMovement();
            UpdateMoveAnimations(_localDesiredVelocity, _angle, _shouldMove);
            GoTo(onDoneMovement, onFailureMovement);

        }
    }

    public virtual void SetTargetPath(Vector3 position, Action onDoneMovement, Action onFailureMovement)
    {
        if (!_ai.hasPath || !_ai.pathPending)  // if we don't have a path or one is not being calculated 
        {
            // Set destination and move to it
            _ai.destination = position;

        }
        GoTo(onDoneMovement, onFailureMovement);
    }

    private void UpdateMoveAnimations(Vector3 localDesiredVelocity, float angle, bool shouldMove)
    {
        _anim.SetBool("Move", shouldMove);
        _anim.SetFloat("TurnAngle", angle);
        _anim.SetFloat("Speed", localDesiredVelocity.magnitude);
    }


    private void DisableMovement()
    {
        _shouldMove = false;
        _angle = 0f;
        _localDesiredVelocity = Vector3.zero;

        UpdateMoveAnimations(_localDesiredVelocity, _angle, _shouldMove);
    }

    private void EnableMovement()
    {
        _shouldMove = true;
    }

    private void GoTo(Action onDoneMovement, Action onFailureMovement)
    {
        _onDoneMovementCallback = onDoneMovement;
        _onFailureMovementCallback = onFailureMovement;
    }

}
