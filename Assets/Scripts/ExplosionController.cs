using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float lifeTime = 0.1f;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Explosion"))
        {
            // Reduce health of the unit hit
            var healthComponent = collision.gameObject.GetComponent<Health>();
            if (healthComponent != null)
            {
                Debug.Log("Explosion collided with " + collision.gameObject.name);
                Debug.Log("Explosion damage: " + damage);
                healthComponent.TakeDamage(damage);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
