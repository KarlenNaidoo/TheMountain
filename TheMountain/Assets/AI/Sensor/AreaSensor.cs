using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class AreaSensor : ReGoapSensor<string, object> {

    public bool enteredTrigger = false;
    private Vector3 targetPosition;
    public override void Init(IReGoapMemory<string, object> memory)
    {
        base.Init(memory);
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        worldState.Set("isAtPosition", transform.position);
        
    }

    public override void UpdateSensor()
    {
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        worldState.Set("isAtPosition", transform.position);
      
    }

    private void OnTriggerEnter(Collider other)
    {
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        if (other.gameObject.tag.Equals("Player"))
        {
            targetPosition = other.gameObject.transform.position;
            worldState.Set("playerPosition", targetPosition);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        if (other.gameObject.tag.Equals("Player"))
        {
            targetPosition = other.gameObject.transform.position;
            worldState.Set("playerPosition", targetPosition);
        }
    }
    
    /**  private void OnTriggerExit(Collider other)
      {
          var worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
          if (other.gameObject.tag.Equals("Player"))
          {
              worldState.Remove("playerPosition");
          }
      }
      */
}
