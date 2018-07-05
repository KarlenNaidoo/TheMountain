using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointReachedAction : GoapAction
{
    private bool _reachedDestination = false;

    public WayPointReachedAction() {
        AddPreConditions("reachWayPoint", true); // we reached our waypoint
        AddEffect("reachedDestination", true);
        AddEffect("reachWayPoint", false);
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
        Debug.Log("Destination reached");
        return _reachedDestination;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        _reachedDestination = false;
    }
}
