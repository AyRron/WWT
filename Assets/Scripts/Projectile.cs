using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float explosionRadius = 15f;    // Rayon de l'explosion
    public float explosionForce = 700f;   // Force de l'explosion
    public float damage = 40f;            // Dégâts infligés aux tanks
    public GameObject explosionEffect;    // Effet visuel d'explosion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }


    private void Explode()
    {
        if (explosionEffect != null)
        {
            var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        // Trouver tous les objets dans le rayon de l'explosion
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var nearbyObject in colliders)
        {
            // Appliquer une force d'explosion aux objets physiques
            var rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Calculer les dégâts en fonction de la distance
            var tankHealth = nearbyObject.GetComponent<TankHealth>();
            if (tankHealth == null) continue;

            var distance = Vector3.Distance(transform.position, nearbyObject.transform.position);
            var damageMultiplier = Mathf.Clamp01(1 - (distance / explosionRadius));
            var finalDamage = damage * damageMultiplier;

            tankHealth.TakeDamage(finalDamage);
        }

        Destroy(gameObject);
    }
}