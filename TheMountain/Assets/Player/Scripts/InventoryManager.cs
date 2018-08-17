using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    
    StateManager _states;
    PlayerBlackboard _blackboard;
    [SerializeField] WeaponList _weaponList;

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
public class WeaponList
{
     public List<WeaponAction> oneHandedSwordActions;
     public List<WeaponAction> twoHandedSwordActions;
}