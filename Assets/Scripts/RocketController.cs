using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public int damage;
    public int splashDamage;
    public float splashSize;
    public float destroyAfter;
    public GameObject explosionPrefab;
    private Collider2D spawnerCollider;
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float targetDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        // Destroy the rocket after the specified time if it hasn't traveled the target distance
        Destroy(gameObject, destroyAfter);
    }

    public void SetSpawnerCollider(Collider2D collider)
    {
        spawnerCollider = collider;
        Collider2D rocketCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(rocketCollider, spawnerCollider);
    }

    public void SetInitialRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void SetTargetDistance(float distance)
    {
        targetDistance = distance;
    }

    void Update()
    {
        // Rotate the rocket to face the direction it is traveling
        Vector2 direction = rb.velocity;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Adjust for sprite orientation
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // Check if the rocket has traveled the target distance
        float traveledDistance = Vector2.Distance(startPosition, transform.position);
        if (traveledDistance >= targetDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Explosion"))
        {
            // Handle collision with other game objects
            Debug.Log("Rocket collided with " + collision.gameObject.name);

            // Reduce health of the unit hit
            var healthComponent = collision.gameObject.GetComponent<Health>();
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damage);
            }

            // Destroy the rocket on collision
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Create an explosion effect when the rocket is destroyed
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        ExplosionController explosionController = explosion.GetComponent<ExplosionController>();

        explosionController.damage = splashDamage;
        explosion.transform.localScale = new Vector3(splashSize, splashSize, 1);
    }
}
