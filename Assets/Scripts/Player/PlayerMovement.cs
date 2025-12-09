using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool sprintPressed;
    
    public bool IsSprinting { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetSprint(bool active)
    {
        sprintPressed = active;
    }

    private void HandleMovement()
    {
        float currentStamina = GetComponent<LivingEntity>()?.CurrentStamina ?? 100f;
        bool canSprint = sprintPressed && currentStamina > 0.1f;
        float targetSpeed = canSprint ? sprintSpeed : walkSpeed;

        IsSprinting = canSprint;

        Vector2 dir = moveInput.normalized;
        rb.linearVelocity = dir * targetSpeed;
    }
}