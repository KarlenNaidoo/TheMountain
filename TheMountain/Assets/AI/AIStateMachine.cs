using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* The base class common to all AI entities
 */

public enum AIStateType { None, Idle, Alerted, Patrol, Dead} 
public enum AITargetType { None, Waypoint, Visual_Target, Visual_Light, Audio_Target}
public enum AITriggerEventType { Enter, Stay, Exit }
// A potential target to the AI system
public struct AITarget
{
    private AITargetType _type;
    private Collider _collider;
    private Vector3 _position;
    private float _distance;
    private float _time;

    public AITargetType type { get { return _type; } }
    public Collider collider { get { return _collider; } }
    public Vector3 position { get { return _position; } }
    public float distance { get { return _distance; } set { _distance = value; } }
    public float time { get { return _time; } }

    public void Set(AITargetType t, Collider c, Vector3 p, float d)
    {
        _type = t;
        _collider = c;
        _position = p;
        _distance = d;
        _time = Time.time;
    }

    public void Clear()
    {
        _type = AITargetType.None;
        _collider = null;
        _position = Vector3.zero;
        _distance = Mathf.Infinity;
        _time = 0;
    }
}

public abstract class AIStateMachine : MonoBehaviour {

    public AITarget visualThreat = new AITarget();
    public AITarget audioThreat = new AITarget();

    protected AIState _currentState = null;
    protected Dictionary<AIStateType, AIState> _states = new Dictionary<AIStateType, AIState>();
    protected AITarget _target = new AITarget(); // protected because we don't want states to update the target directly
    protected int _rootPositionRefCount = 0;
    protected int _rootRotationRefCount = 0;

    [SerializeField] protected AIStateType _currentStateType = AIStateType.Idle; // default state
    [SerializeField] protected SphereCollider _targetTrigger = null;
    [SerializeField] protected SphereCollider _sensorTrigger = null;

    [SerializeField] [Range(0, 15)] protected float _stoppingDistance = 1.0f;

    protected Animator _animator = null;
    protected NavMeshAgent _navAgent = null;
    protected Collider _collider = null;

    public Animator animator { get { return _animator; } }
    public NavMeshAgent navAgent { get { return _navAgent; } }

    public Vector3 sensorPosition
    {
        get
        {
            if (_sensorTrigger == null) return Vector3.zero;
            Vector3 point = _sensorTrigger.transform.position;
            point.x += _sensorTrigger.center.x * _sensorTrigger.transform.lossyScale.x;
            point.y += _sensorTrigger.center.y * _sensorTrigger.transform.lossyScale.y;
            point.z += _sensorTrigger.center.z * _sensorTrigger.transform.lossyScale.z;
            return point;
        }
    }

    public float sensorRadius
    {
        get
        {
            if (_sensorTrigger == null) return 0f;
            float radius = Mathf.Max(_sensorTrigger.radius * _sensorTrigger.transform.lossyScale.x, _sensorTrigger.radius * _sensorTrigger.transform.lossyScale.y);
            return Mathf.Max(radius, _sensorTrigger.radius * _sensorTrigger.transform.lossyScale.z);
        }
    }

    public bool useRootPosition{get { return _rootPositionRefCount > 0; } }
    public bool useRootRotation { get { return _rootRotationRefCount > 0; } }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();

        if (GameSceneManager.instance != null)
        {
            // Register State Machines with Scene Database
            if (_collider) GameSceneManager.instance.RegisterAIStateMachine(_collider.GetInstanceID(), this);
            if (_sensorTrigger) GameSceneManager.instance.RegisterAIStateMachine(_sensorTrigger.GetInstanceID(), this);
        }
    }

    protected virtual void Start()
    {
        if(_sensorTrigger != null)
        {
            AISensor sensorScript = _sensorTrigger.GetComponent<AISensor>();
            if (sensorScript!= null)
            {
                sensorScript.parentStateMachine = this;
            }
        }
        AIState[] states = GetComponents<AIState>();
        foreach (AIState state in states)
        {
            if (state != null & !_states.ContainsKey(state.GetStateType()))
            {
                // Add this state to the state dictionary
                _states[state.GetStateType()] = state;
                state.SetStateMachine(this);
            }
        }
        if (_states.ContainsKey(_currentStateType))
        {
            _currentState = _states[_currentStateType];
            _currentState.OnEnterState();
        }
        else
        {
            _currentState = null;
            Debug.LogWarning("Cannot find state");
        }
    }

    // Sets the target and moves the target trigger to that location
    public void SetTarget(AITargetType t, Collider c, Vector3 p, float d)
    {
        _target.Set(t, c, p, d);
        if (_targetTrigger != null)
        {
            _targetTrigger.radius = _stoppingDistance;
            _targetTrigger.transform.position = _target.position;
            _targetTrigger.enabled = true;
        }
    }

    // Sets the target and moves the target trigger to that location
    public void SetTarget(AITargetType t, Collider c, Vector3 p, float d, float s)
    {
        _target.Set(t, c, p, d);
        if (_targetTrigger != null)
        {
            _targetTrigger.radius = s;
            _targetTrigger.transform.position = _target.position;
            _targetTrigger.enabled = true;
        }
    }

    // Sets the target and moves the target trigger to that location
    public void SetTarget(AITarget t)
    {
        _target = t;
        if (_targetTrigger != null)
        {
            _targetTrigger.radius = _stoppingDistance;
            _targetTrigger.transform.position = _target.position;
            _targetTrigger.enabled = true;
        }
    }

    public void ClearTarget()
    {
        _target.Clear();
        if (_targetTrigger != null)
        {
            _targetTrigger.enabled = false;
        }
    }

    // Clears the audio and visual threats each update and re-calculates the distance to the current target
    protected virtual void FixedUpdate()
    {
        visualThreat.Clear();
        audioThreat.Clear();

        if (_target.type != AITargetType.None)
        {
            _target.distance = Vector3.Distance(transform.position, _target.position);
        }
    }

    /* Calls the update function of the state, if after updating we see that we are in a different state
     * then we need to transition. Look at our dictionary which stores the list of known states and then transition into
     * that
     */
    protected virtual void Update()
    {
        if (_currentState == null) return;

        AIStateType newStateType = _currentState.OnUpdate();
        if (newStateType != _currentStateType)
        {
            AIState newState = null;
            if (_states.TryGetValue(newStateType, out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }
            else if (_states.TryGetValue(AIStateType.Idle, out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }
        }
        _currentStateType = newStateType;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_targetTrigger == null || other != _targetTrigger) return; // we only care if we reached OUR target trigger not someone else's

        if (_currentState)
        {
            _currentState.OnDesitinationReached(true);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (_targetTrigger == null || other != _targetTrigger) return; // we only care if we reached OUR target trigger not someone else's

        if (_currentState!=null)
        {
            _currentState.OnDesitinationReached(false);
        }
    }

    public virtual void OnTriggerEvent(AITriggerEventType type, Collider other)
    {
        if (_currentState != null)
            _currentState.OnTriggerEvent(type, other);
    }

    // Call the animator move only on the current state and not all states
    protected virtual void OnAnimatorMove()
    {
        if (_currentState != null)
            _currentState.OnAnimatorUpdated();
    }

    protected virtual void OnAnimatorIK(int layerIndex)
    {
        if (_currentState != null)
            _currentState.OnAnimatorIKUpdated();
    }

    // determine whether we want root motion or not
    public void NavAgentControl(bool positonUpdate, bool rotationUpdate)
    {
        if (_navAgent)
        {
            _navAgent.updatePosition = positonUpdate;
            _navAgent.updateRotation = rotationUpdate;
        }
    }

    // Called by the State Machine Behaviour to Enable/Disable root motion
    public void AddRootMotionRequest (int rootPosition, int rootRotation)
    {
        _rootPositionRefCount += rootPosition;
        _rootRotationRefCount += rootRotation;
    }

}
