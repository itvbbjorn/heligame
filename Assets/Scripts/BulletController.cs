using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float destroyAfter;
    private Collider2D spawnerCollider;
    
    void Start()
    {
        // Destroy the bullet after the specified time
        Destroy(gameObject, destroyAfter);
    }

    public void SetSpawnerCollider(Collider2D collider)
    {
        spawnerCollider = collider;
        Collider2D bulletCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(bulletCollider, spawnerCollider);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Bullet collided with " + collision.gameObject.name);

        // Reduce health of the unit hit
        var healthComponent = collision.gameObject.GetComponent<Health>();
        if (healthComponent != null)
        {
            
            healthComponent.TakeDamage(damage);
        }
        else {
            var playerHealthComponent = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealthComponent != null)
            {
                playerHealthComponent.TakeDamage(damage);
            }
        }

        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}