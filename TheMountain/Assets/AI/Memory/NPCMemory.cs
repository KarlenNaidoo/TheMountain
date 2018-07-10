using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;

public class NPCMemory : ReGoapMemoryAdvanced<string, object> {
    [SerializeField] List<Transform> patrolPoints;
    int i = 0;
    protected override void Start()
    {
        base.Start();
        var worldState = GetWorldState();
        worldState.Set("patrolDestinations", patrolPoints);
    }
    


}
