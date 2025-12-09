using UnityEngine;

public class ProjectileWeapon : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;

    public override void AttackFromPoint(Transform firePoint)
    {
        if (!CanFire) return;
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        SimpleProjectile p = proj.GetComponent<SimpleProjectile>();
        if (p != null)
        {
            p.Launch(firePoint.up, projectileSpeed, damage);
        }
        else
        {
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = firePoint.up * projectileSpeed;
        }

        ConsumeCooldown();
    }
}