using UnityEngine;
using System.Collections;

public class Hurtbox : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public void ReceiveDamage()
    {
        Debug.Log(gameObject.name + " received damage");
    }
}
