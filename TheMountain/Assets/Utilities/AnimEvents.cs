using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour {

    public Collider currentHitBoxTrigger { get; set; }

    public void OpenHitBox()
    {
        // currentHitBoxTrigger should be passed in from the statemachinebehaviour
        if (currentHitBoxTrigger)
        {
            Debug.Log("OPEN HITBOX");
            currentHitBoxTrigger.enabled = true;
        }
    }


    public void CloseHitBox()
    {
        if (currentHitBoxTrigger)
        {

            Debug.Log("CLOSE HITBOX");
            currentHitBoxTrigger.enabled = false;

        }
    }
}
