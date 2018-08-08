using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Player.Utility;

// This lists shared variables by any object wanting to have it's own blackboard
public interface IBlackboard 
{
     Animator animator { get; set; }
     bool isCrouching { get; set; }
     List<HitBoxArea> hitboxes { get; set; }
     List<HitBox> activeHitboxComponents { get; set; }
     void SetAttackParameters(bool shouldAttack);
}
