using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PassiveAnimal : LivingEntity
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;

    [Header("Behaviour")]
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private float safeDistance = 10f;
    [SerializeField] private float lookDeadZone = 0.1f;

    private Rigidbody2D rb;
    private Transform player;

    private bool isFleeing;

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

        if (!isFleeing)
        {
            // Empieza a huir si el jugador entra en el radio de detección
            if (dist <= detectionRadius)
                isFleeing = true;
        }
        else
        {
            // Deja de huir cuando está suficientemente lejos
            if (dist >= safeDistance)
                isFleeing = false;
        }

        // Actualizar estamina: solo gasta cuando está huyendo "a toda velocidad"
        bool wantsSprint = isFleeing && CurrentStamina > 0.1f;
        UpdateStamina(wantsSprint, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isFleeing)
            HandleFleeing();
        else
            rb.linearVelocity = Vector2.zero; // por ahora se queda quieto cuando no huye
    }

    private void HandleFleeing()
    {
        Vector2 toPlayer = (player.position - transform.position);
        Vector2 fleeDir = -toPlayer.normalized;

        bool canSprint = CurrentStamina > 0.1f;
        float speed = canSprint ? runSpeed : walkSpeed;

        rb.linearVelocity = fleeDir * speed;

        // Rotar el animal en la dirección en la que huye (opcional)
        if (fleeDir.sqrMagnitude > lookDeadZone * lookDeadZone)
        {
            float angle = Mathf.Atan2(fleeDir.y, fleeDir.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }
    }
}