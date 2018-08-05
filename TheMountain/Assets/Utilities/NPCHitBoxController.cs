using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Player.Utility;

public class NPCHitBoxController : HitboxControllerBase {
    
    
    NPCProfile npcProfile;
   
    protected override void Awake()
    {
        base.Awake();
        npcProfile = GetComponent<NPCProfile>();
    }
    
    
    protected override List<HitBox> GetActiveHitboxComponents()
    {
        List<HitBox> activeHitboxes = new List<HitBox>();
        if (_blackboard.hitboxes != null)
        {
            for (int i = 0; i < _blackboard.hitboxes.Count; i++)
            {
                for (int j = 0; j < npcProfile.hitboxProfile.Length; j++)
                {
                    if (npcProfile.hitboxProfile[j].hitboxArea == _blackboard.hitboxes[i])
                    {
                        activeHitboxes.Add(npcProfile.hitboxProfile[j].hitBox.GetComponent<HitBox>());
                    }
                }
            }
        }
        return activeHitboxes;
    }


    protected override List<Collider> GetActiveHitboxTriggers()
    {
        List<Collider> activeHitboxTriggers = new List<Collider>();
        if (_blackboard.hitboxes != null && _blackboard.hitboxes.Count > 0)
        {
            for (int i = 0; i < _blackboard.hitboxes.Count; i++)
            {
                for (int j = 0; j < npcProfile.hitboxProfile.Length; j++)
                {
                    if (npcProfile.hitboxProfile[j].hitboxArea == _blackboard.hitboxes[i])
                    {
                        activeHitboxTriggers.Add(npcProfile.hitboxProfile[j].hitBox);
                    }
                }
            }
        }
        return activeHitboxTriggers;
    }

    
}
