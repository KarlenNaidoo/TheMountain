using UnityEngine;
using System.Collections;

public class OnDeadTrigger : MonoBehaviour
{
    public HealthController healthController;
    public HealthController.OnDead onDead;
    // Use this for initialization
    void Start()
    {
        healthController = GetComponent<HealthController>();
        onDead = OnDeadHandle;
    }


    public void OnDeadHandle()
    {
        Debug.Log("I've been called to die");
    }
}
