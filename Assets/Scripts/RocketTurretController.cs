using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTurretController : MonoBehaviour
{
    public GameObject rocketPrefab; // Prefab for the bullet
    public GameObject explosionPrefab;
    public float rocketSpeed = 10.0f; // Speed of the bullet
    public float rotationSpeed = 5.0f; // Speed at which the turret rotates
    public float rocketDestroyAfter = 3.0f;
    public float fireRate = 0.1f;
    public float delayBetweenBursts = 3.0f;
    public float shotDirectionVariation = 10.0f;
    public int burstLength = 3;
    public int rocketDamage = 10; // Damage of the bullet
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Collider2D turretCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        turretCollider = GetComponent<Collider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(StartFiringWithDelay());
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsPlayer();
        if (Helpers.CanSeePlayer(transform, playerTransform))
        {
            Debug.DrawLine(transform.position, playerTransform.position, Color.green);
        }
        else
        {
            Debug.DrawLine(transform.position, playerTransform.position, Color.red);
        }
    }

    void RotateTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Adjust for sprite orientation
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        rb.rotation = Mathf.LerpAngle(rb.rotation, targetRotation.eulerAngles.z, rotationSpeed * Time.deltaTime);
    }

    IEnumerator StartFiringWithDelay()
    {
        float delay = Random.Range(1.0f, 4.0f);
        yield return new WaitForSeconds(delay);
        StartCoroutine(FireRockets());
    }

    IEnumerator FireRockets()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenBursts); // Wait for 3 seconds

            if (Helpers.CanSeePlayer(transform, playerTransform))
            {
                for (int i = 0; i < burstLength; i++)
                {
                    FireRocket();
                    yield return new WaitForSeconds(fireRate); // Small delay between each bullet
                }
            }
        }
    }

    void FireRocket()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Adjust direction by a random angle between -10 and 10 degrees
        float randomAngle = Random.Range(-shotDirectionVariation, shotDirectionVariation);
        float radians = randomAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Vector2 adjustedDirection = new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        );

        GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rocketRb = rocket.GetComponent<Rigidbody2D>();
        rocketRb.velocity = adjustedDirection * rocketSpeed;

        RocketController rocketScript = rocket.GetComponent<RocketController>();
        rocketScript.destroyAfter = rocketDestroyAfter;
        rocketScript.damage = rocketDamage;
        rocketScript.SetSpawnerCollider(turretCollider);
    }
    private void OnDestroy()
    {
        int explosionSize = transform.localScale.x > transform.localScale.y ? (int)transform.localScale.x + 1 : (int)transform.localScale.y + 1;
        Helpers.CreateExplosion(explosionPrefab, transform.position, 2, explosionSize);
    }
}
