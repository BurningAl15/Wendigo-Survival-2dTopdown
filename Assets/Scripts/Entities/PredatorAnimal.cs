using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PredatorAnimal : LivingEntity
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Behaviour")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 15f;

    [Header("Drops")]
    [SerializeField] private GameObject meatDropPrefab;

    private Rigidbody2D rb;
    private Transform player;
    private float nextAttackTime;
    private bool isChasing;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        isChasing = dist <= detectionRadius;

        if (isChasing && dist <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isChasing)
            ChasePlayer();
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float speed = CurrentStamina > 0.1f ? runSpeed : walkSpeed;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90f;
    }

    private void AttackPlayer()
    {
        LivingEntity playerEntity = player.GetComponent<LivingEntity>();
        if (playerEntity != null)
        {
            playerEntity.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    protected override void Die()
    {
        if (meatDropPrefab != null)
        {
            Instantiate(meatDropPrefab, transform.position, Quaternion.identity);
        }
        base.Die();
    }
}
