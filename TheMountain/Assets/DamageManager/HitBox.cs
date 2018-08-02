using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour
{
    private IHitboxResponder _responder = null;
    private AnimEvents _animEvents;
    private Animator _anim;

    private void Awake()
    {
        _animEvents = GetComponentInParent<AnimEvents>();
        _anim = GetComponentInParent<Animator>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTER");
        Debug.Log(gameObject.name + " collided with " + other.gameObject.name);
        _responder?.CollidedWith(other); // ? means it wont throw a Null Reference if _responder is null
        //other.GetComponentInParent<Animator>().Play("Idle_Hit_Strong_Right");
    }
    


    public void SetResponder(IHitboxResponder responder)
    {
        _responder = responder;

    }


    


}
