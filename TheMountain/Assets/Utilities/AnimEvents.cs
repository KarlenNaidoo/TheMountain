using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour {

    public Collider currentHitBoxTrigger { get; set; }

    public bool OpenHitBox()
    {
        // currentHitBoxTrigger should be passed in from the statemachinebehaviour
        if (currentHitBoxTrigger)
        {
            Debug.Log("OPEN HITBOX");
            currentHitBoxTrigger.enabled = true;
            return true;
        }

        return false;
    }


    public bool CloseHitBox()
    {
        if (currentHitBoxTrigger)
        {

            Debug.Log("CLOSE HITBOX");
            currentHitBoxTrigger.enabled = false;
            return true;
        }

        return false;
    }
}
