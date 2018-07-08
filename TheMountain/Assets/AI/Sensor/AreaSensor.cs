using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class AreaSensor : ReGoapSensor<string, object> {
    
    private Vector3 objectivePosition;
    public override void Init(IReGoapMemory<string, object> memory)
    {
        base.Init(memory);
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        //worldState.Set("objectivePosition", objectivePosition != null ? objectivePosition : transform.position);
        //worldState.Set("objectivePosition", objectivePosition != null ? objectivePosition : transform.position);

    }

    public override void UpdateSensor()
    {
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        worldState.Set("objectivePosition", objectivePosition != null ? objectivePosition : transform.position);
        base.UpdateSensor();
    }

    private void OnTriggerEnter(Collider other)
    {
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
    
        if (other.gameObject.tag.Equals("Player"))
        {
            objectivePosition = other.gameObject.transform.position;
            worldState.Set("objectivePosition", objectivePosition);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        if (other.gameObject.tag.Equals("Player"))
        {
            objectivePosition = other.gameObject.transform.position;

            worldState.Set("objectivePosition", objectivePosition);
        }
    }
}
