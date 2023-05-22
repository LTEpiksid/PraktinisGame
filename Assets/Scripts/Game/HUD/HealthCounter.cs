using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthCounter : MonoBehaviour
{
    public Text healthText;
    public Health health;

    private void Start()
    {
        UpdateHealthCounter();
        health.OnHitWithReference.AddListener((GameObject sender) => UpdateHealthCounter());
        health.OnDeathWithReference.AddListener((GameObject sender) => UpdateHealthCounter());
    }

    private void UpdateHealthCounter()
    {
        if (healthText != null && health != null)
        {
            int currentHealth = health.GetCurrentHealth();
            int maxHealth = health.GetMaxHealth();
            healthText.text = "Health: " + currentHealth + " / " + maxHealth;
        }
    }
}

