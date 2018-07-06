using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GoToFirstWaypointAction : GoapAction
{
    private bool _reached = false;
    private Waypoint _targetWaypoint;
    float _startTime = 0;
    
    public GoToFirstWaypointAction()
    {
        //AddPreConditions("reachWayPoint1", false); // waypoint must be visible
        AddEffect("reachWayPoint1", true); // we reached our destination
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
