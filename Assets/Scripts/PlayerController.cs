using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float bounceForce = 10.0f;
    public float speed = 5.0f;
    public float maxSpeed = 10.0f;
    public float rotationSpeed = 3.0f;
    public int gunBurstLength = 3;
    public float gunFireRate = 0.3f;
    public float gunCoolDownTime = 1.0f;
    public float gunBulletSpeed = 10.0f; // Speed of the bullet
    public float gunBulletDestroyAfter = 3.0f; // Time after which the bullet is destroyed
    public int gunBulletDamage = 10; // Damage of the bullet
    public int terrainDamage = 5;
    public float damageCoolDownTime = 2.0f;
    public bool damageCoolingDown = false;
    public int rocketBurstLength = 8;
    public float rocketFireRate = 0.3f;
    public float rocketCoolDownTime = 1.0f;
    public float rocketSpeed = 15.0f;
    public float rocketDestroyAfter = 3.0f;
    public int rocketDamage = 50;
    public float rocketOffset = 0.3f;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private PlayerHealth healthComponent;
    private bool gunCoolingDown = false;
    private bool rocketsCoolingDown = false;
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public GameObject rocketPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        healthComponent = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = 0.0f;
        float moveVertical = 0.0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1.0f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1.0f;
        }

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.AddForce(movement * speed);

        // Clamp the velocity to the max speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        // Rotate the player to face the mouse cursor
        RotateTowardsMouse();

        // Check if the player's health is zero or less
        if (healthComponent != null && healthComponent.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }

        

        // Fire bullet when spacebar or left mouse button is pressed
        if (!gunCoolingDown && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            StartCoroutine(GunBurst());
            StartCoroutine(StartGunCoolDown());
        }
        if (!rocketsCoolingDown && (Input.GetMouseButtonDown(1)))
        {
            StartCoroutine(RocketBurst());
            StartCoroutine(StartRocketCoolDown());
        }
    }

    void FixedUpdate() 
    {
        RotateTowardsMouse();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain") && !damageCoolingDown)
        {
            // Take damage
            healthComponent.TakeDamage(terrainDamage);
            StartCoroutine(DamageCoolDown());
        }

        if (collision.gameObject.CompareTag("Terrain"))
        {
            // Calculate bounce direction
            Vector2 collisionNormal = collision.contacts[0].normal;
            Vector2 bounceDirection = -collisionNormal;

            // Completely stop the object
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Apply bounce force
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
    }
    
    void RotateTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Adjust for sprite orientation
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
    public IEnumerator DamageCoolDown()
    {
        damageCoolingDown = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        float elapsedTime = 0f;
        while (elapsedTime < damageCoolDownTime)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.2f; // Increment by the total duration of one blink cycle
        }

        spriteRenderer.color = originalColor;
        damageCoolingDown = false;
    }
    IEnumerator StartGunCoolDown()
    {
        gunCoolingDown = true;
        yield return new WaitForSeconds(gunCoolDownTime);
        gunCoolingDown = false;
    }
    IEnumerator StartRocketCoolDown()
    {
        rocketsCoolingDown = true;
        yield return new WaitForSeconds(rocketCoolDownTime);
        rocketsCoolingDown = false;
    }
    IEnumerator RocketBurst()
    {
        bool offsetPositive = true;
        for (int i = 0; i < rocketBurstLength; i++)
        {
            FireRocket(rocketOffset, offsetPositive);
            offsetPositive = !offsetPositive;
            yield return new WaitForSeconds(rocketFireRate); // Small delay between each rocket
        }
    }

    IEnumerator GunBurst()
    {
        for (int i = 0; i < gunBurstLength; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(gunFireRate); // Small delay between each bullet
        }
    }

    void FireBullet()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = direction * gunBulletSpeed;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.destroyAfter = gunBulletDestroyAfter;
        bulletScript.damage = gunBulletDamage;
        bulletScript.SetSpawnerCollider(playerCollider);
    }
    void FireRocket(float offsetBy, bool offsetPositive)
    {
        // Calculate the direction based on the player's current rotation
        Vector2 direction = transform.up; // Assuming the player sprite's forward direction is up

        // Calculate the position to instantiate the rocket
        float offset = offsetPositive ? offsetBy : -offsetBy;
        Vector3 offsetVector = Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * new Vector3(offset, 0f, 0f);
        Vector3 rocketPosition = transform.position + offsetVector;

        GameObject rocket = Instantiate(rocketPrefab, rocketPosition, transform.rotation); // Set initial rotation to player's rotation
        Rigidbody2D rocketRb = rocket.GetComponent<Rigidbody2D>();
        rocketRb.velocity = direction * rocketSpeed; // Set rocket velocity directly

        RocketController rocketScript = rocket.GetComponent<RocketController>();
        rocketScript.destroyAfter = rocketDestroyAfter;
        rocketScript.damage = rocketDamage;
        rocketScript.SetSpawnerCollider(playerCollider);
        rocketScript.SetInitialRotation(transform.rotation); // Set initial rotation to player's rotation
    }
}