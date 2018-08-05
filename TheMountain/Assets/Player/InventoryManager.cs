using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    
    StateManager _states;
    PlayerBlackboard _blackboard;
    [SerializeField] Weapon _weaponList;

    private void Awake()
    {
        _blackboard = GetComponent<PlayerBlackboard>();
    }

    private void Start()
    {
        _blackboard.weaponList = _weaponList;
    }
}


[System.Serializable]
public class Weapon
{
     public List<ControllerAction> oneHandedSwordActions;
     public List<ControllerAction> twoHandedSwordActions;
}