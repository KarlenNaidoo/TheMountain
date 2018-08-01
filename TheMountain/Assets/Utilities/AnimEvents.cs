using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour {

    public Collider currentHitBoxTrigger { get; set; }

    Blackboard blackboard;
    NPCProfile npcProfile;

    private void Start()
    {
        blackboard = GetComponent<Blackboard>();
        npcProfile = GetComponent<NPCProfile>();
    }
    

    public bool OpenHitBox()
    {
        Debug.Log("animation event openhitbox");
        // currentHitBoxTrigger should be passed in from the statemachinebehaviour
        if (blackboard.hitboxes != null)
        {

            for (int i = 0; i < blackboard.hitboxes.Length; i++)
            {
                Debug.Log("Hitboxes on blackboard: " + blackboard.hitboxes[i]);
                Debug.Log("Hitboxes on profile: " + npcProfile.hitboxProfile[i].hitBox);
                if (npcProfile.hitboxProfile[i].hitboxArea == blackboard.hitboxes[i])
                {
                    npcProfile.hitboxProfile[i].hitBox.enabled = true;
                }
            }

            return true;
        }
        return false;
    }


    public bool CloseHitBox()
    {
        Debug.Log("animation event closehitbox");
        if (blackboard.hitboxes != null)
        {
            for (int i = 0; i < blackboard.hitboxes.Length; i++)
            {
                if (npcProfile.hitboxProfile[i].hitboxArea == blackboard.hitboxes[i])
                {
                    npcProfile.hitboxProfile[i].hitBox.enabled = true;
                }
            }

            return true;
        }
        return false;
    }
}
