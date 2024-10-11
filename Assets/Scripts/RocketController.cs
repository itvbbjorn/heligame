using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public int damage;
    public float splashSize;
    public float destroyAfter;
    private Collider2D spawnerCollider;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destroy the rocket after the specified time
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

    void Update()
    {
        // Rotate the rocket to face the direction it is traveling
        Vector2 direction = rb.velocity;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Adjust for sprite orientation
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
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