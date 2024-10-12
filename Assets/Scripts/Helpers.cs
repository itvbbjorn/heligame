using UnityEngine;

public static class Helpers
{
    public static void CreateExplosion(GameObject explosionPrefab, Vector3 position, int splashDamage, float splashSize)
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Object.Instantiate(explosionPrefab, position, Quaternion.identity);
            ExplosionController explosionController = explosion.GetComponent<ExplosionController>();

            if (explosionController != null)
            {
                explosionController.damage = splashDamage;
                explosion.transform.localScale = new Vector3(splashSize, splashSize, 1);
            }

            // Destroy the explosion after 0.1 seconds
            Object.Destroy(explosion, 0.1f);
        }
    }
}
