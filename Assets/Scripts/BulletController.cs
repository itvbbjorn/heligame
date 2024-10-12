using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float destroyAfter;
    public float ignoreCollisionsTime = 0.2f;
    private Collider2D spawnerCollider;
    private Collider2D bulletCollider;

    void Start()
    {
        bulletCollider = GetComponent<Collider2D>();

        // Start the coroutine to ignore collisions for the first 0.2 seconds
        StartCoroutine(IgnoreCollisionsTemporarily());

        // Destroy the bullet after the specified time
        Destroy(gameObject, destroyAfter);
    }

    public void SetSpawnerCollider(Collider2D collider)
    {
        spawnerCollider = collider;
        Physics2D.IgnoreCollision(bulletCollider, spawnerCollider);
    }

    private IEnumerator IgnoreCollisionsTemporarily()
    {
        // Disable the bullet's collider
        bulletCollider.enabled = false;

        yield return new WaitForSeconds(ignoreCollisionsTime);

        // Re-enable the bullet's collider
        bulletCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bullet collided with " + collision.gameObject.name);

        // Reduce health of the unit hit
        var healthComponent = collision.gameObject.GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
        else
        {
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
