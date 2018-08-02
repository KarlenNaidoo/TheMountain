using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimEvents : MonoBehaviour {
    

    Blackboard blackboard;
    NPCProfile npcProfile;
    List<Collider> _hitBoxTriggers;
    Hurtbox[] _hurtboxes;
    public enum ColliderState { Closed, Open, Colliding }

    private ColliderState _state;


    private HitBox[] _hitboxes;

   
    private void Awake()
    {
        blackboard = GetComponent<Blackboard>();
        npcProfile = GetComponent<NPCProfile>();
        _hitboxes = GetComponentsInChildren<HitBox>(true);
        _hurtboxes = GetComponentsInChildren<Hurtbox>(true);
    }

    private void Start()
    {
        IgnoreOwnHurtboxes();
    }

    private void IgnoreOwnHurtboxes()
    {
        foreach (Hurtbox hurtbox in _hurtboxes)
        {

            foreach (HitBox hitbox in _hitboxes)
            {
                Debug.Log("Setting ignore collision for " + hitbox.gameObject.name);
                Physics.IgnoreCollision(hurtbox.gameObject.GetComponent<Collider>(), hitbox.gameObject.GetComponent<Collider>());
            }
        }
    }

    public List<HitBox> GetActiveHitboxes()
    {
        List<HitBox> activeHitboxes = new List<HitBox>();
        if (blackboard.hitboxes != null)
        {

            for (int i = 0; i < blackboard.hitboxes.Count; i++)
            {
                for (int j = 0; j < npcProfile.hitboxProfile.Length; j++)
                {
                    if (npcProfile.hitboxProfile[j].hitboxArea == blackboard.hitboxes[i])
                    {
                        activeHitboxes.Add(npcProfile.hitboxProfile[j].hitBox.GetComponent<HitBox>());
                    }
                }
            }
        }
        return activeHitboxes;
    }


    public List<Collider> GetActiveHitboxTriggers()
    {
        List<Collider> activeHitboxTriggers = new List<Collider>();
        if (blackboard.hitboxes != null && blackboard.hitboxes.Count > 0)
        {
            for (int i = 0; i < blackboard.hitboxes.Count; i++)
            {
                for (int j = 0; j < npcProfile.hitboxProfile.Length; j++)
                {
                    if (npcProfile.hitboxProfile[j].hitboxArea == blackboard.hitboxes[i])
                    {
                        activeHitboxTriggers.Add(npcProfile.hitboxProfile[j].hitBox);
                    }
                }
            }
        }
        return activeHitboxTriggers;
    }

    public void EnableHitbox(bool value)
    {
        List<Collider> hitboxTriggers = new List<Collider>();
        hitboxTriggers = GetActiveHitboxTriggers();
        foreach (var hitbox in hitboxTriggers)
        {
            hitbox.enabled = value;
        }
    }

    public bool OpenHitBox()
    {

        _state = ColliderState.Open;
        Debug.Log("OPEN HITBOX");
        EnableHitbox(true);
        blackboard.activeHitboxTriggers = GetActiveHitboxes();
        return true;
    }


    public bool CloseHitBox()
    {
        Debug.Log("CLOSE HITBOX");
        EnableHitbox(false);
        return true;
    }


    private void checkGizmoColor()
    {
        switch (_state)
        {

            case ColliderState.Closed:

                Gizmos.color = Color.blue;

                break;

            case ColliderState.Open:

                Gizmos.color = Color.green;

                break;

            case ColliderState.Colliding:

                Gizmos.color = Color.yellow;

                break;

        }

    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        //_hitBoxTrigger = gameObject.GetComponent<Collider>();
        checkGizmoColor();
        
        _hitBoxTriggers = GetActiveHitboxTriggers(); // TODO: Causes a null pointer error upon exiting play mode

        if (_hitBoxTriggers != null)
        {
            foreach (var _hitBoxTrigger in _hitBoxTriggers)
            {

                if (_hitBoxTrigger && _hitBoxTrigger.enabled)
                {
                    if (_hitBoxTrigger as BoxCollider)
                    {

                        BoxCollider box = _hitBoxTrigger as BoxCollider;
                        var sizeX = transform.lossyScale.x * box.size.x;
                        var sizeY = transform.lossyScale.y * box.size.y;
                        var sizeZ = transform.lossyScale.z * box.size.z;
                        Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));
                        Gizmos.matrix = rotationMatrix;
                        Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    }
                }
            }
        }
    }
}
