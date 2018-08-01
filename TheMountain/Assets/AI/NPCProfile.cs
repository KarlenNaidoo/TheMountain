using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player.Utility;

[RequireComponent(typeof(Blackboard))]
[RequireComponent(typeof(AnimEvents))]
[RequireComponent(typeof(HitBox))]
[RequireComponent(typeof(IHitboxResponder))]
public class NPCProfile : Character, IHitboxResponder {

    [SerializeField][Range(0,10)] float _aggression;
    [SerializeField] [Range(0, 10)] float _intelligence;
    Blackboard _blackboard;
    List<HitBox> activeHitboxes;
    AnimEvents animEvents;
    public HitboxProfile[] hitboxProfile;

    

    protected override void Awake()
    {
        _blackboard = GetComponent<Blackboard>();
        animEvents = GetComponent<AnimEvents>();
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _blackboard.aggression = _aggression;
        _blackboard.intelligence = _intelligence;
    }
    

    public void CollidedWith(Collider collider)
    {
        Debug.Log("Running collided with method");
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (animEvents.OpenHitBox())
        {
            Damage attackDamage = new Damage(15);
            hurtbox?.ReceiveDamage(attackDamage);
        }
    }


    public void SetAsResponder()
    {
        Debug.Log("Setting responder");
        activeHitboxes = _blackboard.activeHitboxTriggers;
        if (activeHitboxes != null)
        {
            foreach (HitBox activeHitbox in activeHitboxes)
            {
                Debug.Log("Active hitbox: " + activeHitbox);
                activeHitbox.SetResponder(this);
            }
        }
    }
}

[System.Serializable]
public class HitboxProfile
{
    public HitBoxArea hitboxArea;
    public Collider hitBox;

}