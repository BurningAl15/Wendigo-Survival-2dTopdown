using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    private Vector2 direction;
    private float speed;
    private float damage;

    public void Launch(Vector2 dir, float spd, float dmg)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si golpea una entidad viva, le hace daño
        LivingEntity entity = other.GetComponent<LivingEntity>();
        if (entity != null)
        {
            entity.TakeDamage(damage);
        }

        // TODO: ignorar colisión con el Player si hace falta usando layers
        Destroy(gameObject);
    }
}