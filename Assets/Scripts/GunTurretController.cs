using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTurretController : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab for the bullet
    public float bulletSpeed = 10.0f; // Speed of the bullet
    public float rotationSpeed = 5.0f; // Speed at which the turret rotates
    public float bulletDestroyAfter = 3.0f;
    public int bulletDamage = 10; // Damage of the bullet
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
        StartCoroutine(FireBullets());
    }

    IEnumerator FireBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f); // Wait for 3 seconds

            for (int i = 0; i < 3; i++)
            {
                FireBullet();
                yield return new WaitForSeconds(0.1f); // Small delay between each bullet
            }
        }
    }

    void FireBullet()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = direction * bulletSpeed;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.destroyAfter = bulletDestroyAfter;
        bulletScript.damage = bulletDamage;
        bulletScript.SetSpawnerCollider(turretCollider);
    }
}