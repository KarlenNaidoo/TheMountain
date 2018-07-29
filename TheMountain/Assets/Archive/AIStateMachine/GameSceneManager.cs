using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Every collider will be stored in a dictionary with a reference to its statemachine
 */ 
public class GameSceneManager : MonoBehaviour {

    public static GameSceneManager instance = null;
    
    private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("More than one game manager found in the scene");
        }
    }
    // stores the passed state machine in the dictionary with the supplied key
    public void RegisterAIStateMachine(int key, AIStateMachine stateMachine)
    {
        if (!_stateMachines.ContainsKey(key))
        {
            _stateMachines[key] = stateMachine;
        }
    }
    // returns an AI State Machine reference searched on by the instance ID of an object
    public AIStateMachine GetAIStateMachine (int key)
    {
        AIStateMachine machine = null;
        if (_stateMachines.TryGetValue(key, out machine))
        {
            return machine;
        }
        return null;
    }
}
