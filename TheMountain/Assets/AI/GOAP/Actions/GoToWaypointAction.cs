using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GoToWaypointAction : GoapAction
{
    private bool _reached = false;
    private Waypoint _targetWaypoint;

    float _startTime = 0;
    
    public GoToWaypointAction()
    {
        AddPreConditions("visible", true); // waypoint must be visible
        AddEffect("reachWayPoint", true); // we reached our destination
    }

    public override bool CheckProceduralPreCondtions(GameObject agent)
    {
        Debug.Log("Reached here");
        //find the nearest waypoint
        List<Waypoint> waypointNetwork = new List<Waypoint>();
        foreach (Waypoint point in Resources.FindObjectsOfTypeAll<Waypoint>())
        {
            waypointNetwork.Add(point);
        }
        
        Debug.Log("Number of waypoints is " + waypointNetwork.Count);
        Waypoint _closest = null;
        float _closestDist = 0;
        foreach (Waypoint point in waypointNetwork)
        {
            if(_closest == null)
            {
                _closest = point;
                _closestDist = (point.gameObject.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                float _dist = (point.gameObject.transform.position - agent.transform.position).magnitude;
                if(_dist < _closestDist)
                {
                    _closest = point;
                    _closestDist = _dist;
                }
            }
        }
        if (_closest == null)
            return false;
        _targetWaypoint = _closest;
        target = _closest.gameObject;
        Debug.Log("Removing " + target);
        waypointNetwork.Remove(_closest);
        return _closest != null;
    }

    public override bool IsDone()
    {
        return _reached;
    }

    public override bool Perform(GameObject agent)
    {
        Debug.Log("Finished this action");
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
