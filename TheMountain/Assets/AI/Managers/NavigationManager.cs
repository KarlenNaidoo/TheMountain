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
        _anim = GetComponent<Animator>(); // Used to determine root motion
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
            SetTarget(_blackboard.currentTarget.position, _blackboard.onDoneMovement, _blackboard.onFailureMovement);
        }

        MoveToPosition();
    }

    public void AddRootMotionRequest(int rootPosition, int rootRotation)
    {

        _rootPositionRefCount = rootPosition;
        _rootRotationRefCount = rootRotation;
    }
    
    public virtual bool MoveToPosition()
    {
        _blackboard.targetReachedStatus = false;
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
        if (Vector3.Distance(_ai.destination, transform.position) <= _ai.endReachedDistance)
        {
            _blackboard.targetReachedStatus = true;
            DisableMovement();
            return true;
        }

        return false;

    }

    public virtual void SetTarget(Vector3 position, Action onDoneMovement, Action onFailureMovement)
    {
        if (!_ai.hasPath || !_ai.pathPending)  // if we don't have a path or one is not being calculated 
        {
            // Set destination and move to it
            _ai.destination = position;

        }
        GoTo(onDoneMovement, onFailureMovement);
    }
    

    private void DisableMovement()
    {
        _shouldMove = false;
        _angle = 0f;
        _localDesiredVelocity = Vector3.zero;

        _blackboard.SetMoveParameters(_localDesiredVelocity, _angle, _shouldMove);
    }

    private void EnableMovement()
    {
        _shouldMove = true;
        _blackboard.SetMoveParameters(_localDesiredVelocity, _angle, _shouldMove);
    }

    private void GoTo(Action onDoneMovement, Action onFailureMovement)
    {
        _onDoneMovementCallback = onDoneMovement;
        _onFailureMovementCallback = onFailureMovement;
    }

}
