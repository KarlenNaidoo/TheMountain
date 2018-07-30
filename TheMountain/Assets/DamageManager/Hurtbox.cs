using UnityEngine;
using System.Collections;

public class Hurtbox : MonoBehaviour
{
    HealthController healthController;
    Damage damage;
    // Use this for initialization
    float currHealth;
    void Start()
    {
        healthController = GetComponentInParent<HealthController>();
        currHealth = healthController.currentHealth;
    }

    public void ReceiveDamage()
    {
        // Tell health controller to receive damage
        Debug.Log(gameObject.name + " received damage");
        currHealth -= 10;
        Debug.Log("Current health: " + currHealth);
    }
}
