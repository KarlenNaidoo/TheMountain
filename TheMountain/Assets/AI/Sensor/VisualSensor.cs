using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class VisualSensor : ReGoapSensor<string, object> {

    ReGoapState<string, object> worldState;
    private Vector3 lastKnownPlayerPosition;
    private Vector3 dirToPlayer;
    [SerializeField][Range(0,1)] float totalFieldOfView = 70f;
    float halfFOV;


    private void Start()
    {
        worldState = GetComponentInParent<ReGoapMemoryAdvanced<string, object>>().GetWorldState();
        halfFOV = totalFieldOfView / 2f;
    }
    public override void Init(IReGoapMemory<string, object> memory)
    {
        base.Init(memory);
        //worldState.Set("objectivePosition", objectivePosition != null ? objectivePosition : transform.position);
        //worldState.Set("objectivePosition", objectivePosition != null ? objectivePosition : transform.position);

    }

    // is called only if the object the script is attached to is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        float rayRange = 30f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.forward, dirToPlayer);
    }

    public override void UpdateSensor()
    {
        worldState.Set("objectivePosition", lastKnownPlayerPosition != null ? lastKnownPlayerPosition : transform.position);
        base.UpdateSensor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            lastKnownPlayerPosition = other.gameObject.transform.position;
            // Get the direction to the target
            // We do not need to take into account which direction we are facing i.e transform.forward because we will next calculate the angle between our forward direction and this distance
            dirToPlayer =  lastKnownPlayerPosition - transform.position;
            //Debug.Log("Angle is " + Vector3.Angle(transform.forward, dirToPlayer));
            // We do not care about -ve values, we just need to know whether we are within the half field of view
            if (Mathf.Abs(Vector3.Angle(transform.forward, dirToPlayer)) < halfFOV)
            {
                worldState.Set("PlayerVisible", true);
                worldState.Set("lastKnownPlayerPosition", lastKnownPlayerPosition);
            }
            else
            {
                worldState.Set("PlayerVisible", false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            lastKnownPlayerPosition = other.gameObject.transform.position;

            // Get the direction to the target
            // We do not need to take into account which direction we are facing i.e transform.forward because we will next calculate the angle between our forward direction and this distance
            dirToPlayer = lastKnownPlayerPosition - transform.position;            
            if (Mathf.Abs(Vector3.Angle(transform.forward, dirToPlayer)) < halfFOV)
            {
                worldState.Set("PlayerVisible", true);
                worldState.Set("lastKnownPlayerPosition", lastKnownPlayerPosition);
            }
            else
            {
                worldState.Set("PlayerVisible", false);
            }
        }
    }
}
