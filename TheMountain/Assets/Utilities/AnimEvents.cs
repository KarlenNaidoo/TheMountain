using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour {
    

    Blackboard blackboard;
    NPCProfile npcProfile;
    Collider _hitBoxTrigger;
    public enum ColliderState { Closed, Open, Colliding }

    private ColliderState _state;


    private void Start()
    {
        blackboard = GetComponent<Blackboard>();
        npcProfile = GetComponent<NPCProfile>();
    }
    

    public bool OpenHitBox()
    {

        _state = ColliderState.Open;
        Debug.Log("animation event open hitbox");
        // currentHitBoxTrigger should be passed in from the statemachinebehaviour
        if (blackboard.hitboxes != null)
        {

            for (int i = 0; i < blackboard.hitboxes.Length; i++)
            {
                for (int j = 0; j < npcProfile.hitboxProfile.Length; j++)
                {
                    if (npcProfile.hitboxProfile[j].hitboxArea == blackboard.hitboxes[i])
                    {
                        npcProfile.hitboxProfile[j].hitBox.enabled = true;
                        _hitBoxTrigger = npcProfile.hitboxProfile[j].hitBox;
                    }
                }

            }

            return true;
        }
        return false;
    }


    public bool CloseHitBox()
    {
        Debug.Log("animation event close hitbox");
        _state = ColliderState.Closed;
        if (blackboard.hitboxes != null)
        {

            for (int i = 0; i < blackboard.hitboxes.Length; i++)
            {
                for (int j = 0; j < npcProfile.hitboxProfile.Length; j++)
                {
                    if (npcProfile.hitboxProfile[j].hitboxArea == blackboard.hitboxes[i])
                    {
                        npcProfile.hitboxProfile[j].hitBox.enabled = false;
                    }
                }
            }
            return true;
        }
        return false;
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
        //_hitBoxTrigger = gameObject.GetComponent<Collider>();
        checkGizmoColor();
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
