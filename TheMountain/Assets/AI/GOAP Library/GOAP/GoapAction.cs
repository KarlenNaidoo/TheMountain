using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GoapAction : MonoBehaviour
{
    private Dictionary<string, object> _preConditions;
    private Dictionary<string, object> _effects;
    public Dictionary<string, object> preConditions { get { return _preConditions; } }
    public Dictionary<string, object> effects { get { return _effects; } }
    private bool _inRange = false;
    public bool inRange { get { return _inRange; } set { _inRange = value; } }
    // the cost of performing the action. Figure out a weight that suits the action. Changing it will affect what actions are chosen during planning.
    public float _cost = 1f;

    // The target to perform the action on. Can be null
    public GameObject target;

    public GoapAction()
    {
        _preConditions = new Dictionary<string, object>();
        _effects = new Dictionary<string, object>();
    }


    public void doReset()
    {
        _inRange = false;
        target = null;
        Reset();
    }

    /**
     * Reset any variables that need to be reset before planning happens again.
    */
    public abstract void Reset();

    // Is the action done?
    public abstract bool IsDone();

    // Procedurally check if this action can run
    public abstract bool CheckProceduralPreCondtions(GameObject agent);

    /** Runs the action
     * Returns true if the action performed successfully or false if something
     * happened and it can no longer perform. In this case the action queue
     * should clear out and the goal cannot be reached.
     */
    public abstract bool Perform(GameObject agent);

    /**
     * Does this action need to be within range of a target game object?
     * If not then the moveTo state will not need to run for this action
     */
    public abstract bool RequiresInRange();

    public void AddPreConditions(string key, object value)
    {
        _preConditions.Add(key, value);
    }

    public void RemovePreConditons(string key)
    {
        _preConditions.Remove(key);
    }

    public void AddEffect(string key, object value)
    {
        _effects.Add(key, value);
    }

    public void RemoveEffect(string key)
    {
        _effects.Remove(key);
    }

}
