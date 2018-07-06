using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public Enemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Reached " + gameObject.name);
    }
}
