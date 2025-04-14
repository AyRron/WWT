using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 40f;               // Dégâts de base
    public GameObject explosionEffect;       // Effet visuel d'explosion
    public float selfDestructDelay = 3f;     // Auto-destruction

    private void Start()
    {
        Destroy(gameObject, selfDestructDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("Enemy") || hitObject.CompareTag("Player"))
        {
            // Effet d'explosion
            if (explosionEffect != null)
            {
                var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }

            // Essayer de trouver le script de santé
            var tankHealth = hitObject.GetComponent<TankHealth>()
                ?? hitObject.GetComponentInParent<TankHealth>()
                ?? hitObject.GetComponentInChildren<TankHealth>();

            if (tankHealth != null)
            {
                float randomDamage = Random.Range(5f, 15f);
                tankHealth.TakeDamage(randomDamage);
            }
            else
            {
                Debug.LogWarning($"Aucun TankHealth trouvé sur {hitObject.name}", hitObject);
            }

            Destroy(gameObject);
        }
    }
}
