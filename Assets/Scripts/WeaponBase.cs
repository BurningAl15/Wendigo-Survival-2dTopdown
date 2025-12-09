using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] protected float damage = 10f;

    [Header("Cooldown")]
    [SerializeField] protected float fireCooldown = 0.5f;
    protected float nextFireTime = 0f;

    public bool CanFire => Time.time >= nextFireTime;

    public abstract void AttackFromPoint(Transform firePoint);

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }

    protected void ConsumeCooldown()
    {
        nextFireTime = Time.time + fireCooldown;
    }
}