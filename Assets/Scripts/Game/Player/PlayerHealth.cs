using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnHitWithReference.AddListener(HandleHit);
            health.OnDeathWithReference.AddListener(HandleDeath);
        }
    }

    private void HandleHit(GameObject sender)
    {
        // Handle hit logic here
        Debug.Log("Player got hit by: " + sender.name);
    }

    private void HandleDeath(GameObject sender)
    {
        // Handle death logic here
        Debug.Log("Player died. Game over!");
    }
}

