using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllWayPointReachedAction : GoapAction
{
    private bool _reachedDestination = false;

    public AllWayPointReachedAction() {
        AddPreConditions("reachWayPoint1", true); // we reached our waypoint
        AddPreConditions("reachWayPoint2", true); // we reached our waypoint
        AddEffect("allWaypoints", true);
   }

    public override bool CheckProceduralPreCondtions(GameObject agent)
    {
        return true;
    }

    public override bool IsDone()
    {
        return _reachedDestination;
    }

    public override bool Perform(GameObject agent)
    {

        _reachedDestination = true;
        Debug.Log("Patrolled all waypoints");
        return _reachedDestination;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        _reachedDestination = false;
    }
}
