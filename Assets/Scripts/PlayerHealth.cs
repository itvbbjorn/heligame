using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public void TakeDamage(int damage)
    {
        if (playerController != null && !playerController.damageCoolingDown)
        {
            Debug.Log("Took damage");
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                playerController.StartCoroutine(playerController.DamageCoolDown());
            }
        }
    }
}