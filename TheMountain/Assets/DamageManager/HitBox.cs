using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour
{
    private IHitboxResponder _responder = null;
    private AnimEvents _animEvents;
    

    private void Awake()
    {
        _animEvents = GetComponentInParent<AnimEvents>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " collided with " + other.gameObject.name);
        _responder?.CollidedWith(other);
    }
    
    public void SetResponder(IHitboxResponder responder)
    {
        _responder = responder;

    }


    


}
