using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public GameObject projectilePrefab;  // Préfabriqué du projectile
    public Transform firePoint;          // Position du tir (canon)
    public float shootForce = 40f;       // Puissance du tir
    public float fireRate = 1f;          // Temps entre deux tirs

    private float _nextFireTime = 5f;

    public void Shoot()
    {
        if (Time.time < _nextFireTime || !projectilePrefab || !firePoint) return;

        var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        var rb = projectile.GetComponent<Rigidbody>();
        if (rb) rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
            
        _nextFireTime = Time.time + 1f / fireRate;
    }
}