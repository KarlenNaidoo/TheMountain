using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GoToSecondWaypointAction : GoapAction
{
    private bool _reached = false;
    private Waypoint _targetWaypoint;

    float _startTime = 0;

    public GoToSecondWaypointAction()
    {
        AddPreConditions("reachWayPoint1", true); // waypoint must be visible
        AddEffect("reachWayPoint2", true); // we reached our destination
    }

    public override bool CheckProceduralPreCondtions(GameObject agent)
    {
        return true;
    }

    public override bool IsDone()
    {
        return _reached;
    }

    public override bool Perform(GameObject agent)
    {
        _reached = true;

        return true;
    }

    public override bool RequiresInRange()
    {
        return true; // yes we need to be in range of the waypoint to have reached it
    }

    public override void Reset()
    {
        _reached = false;
        _targetWaypoint = null;
    }
}
