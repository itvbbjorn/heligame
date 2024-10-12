using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;

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
    public static bool CanSeePlayer(Transform transform, Transform playerTransform)
    {
        if (playerTransform == null) return false;

        // Calculate the direction to the center of the player
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        LayerMask layerMask = LayerMask.GetMask("Player", "Terrain", "Default");

        // Perform the raycast towards the center of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, layerMask);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

}
