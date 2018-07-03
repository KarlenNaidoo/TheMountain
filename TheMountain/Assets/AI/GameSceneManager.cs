using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Every collider will be stored in a dictionary with a reference to its statemachine
 */ 
public class GameSceneManager : MonoBehaviour {

    private static GameSceneManager _instance = null;

    public static GameSceneManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (GameSceneManager)FindObjectOfType(typeof(GameSceneManager));
            return instance;
        }
    }
    private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();

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
