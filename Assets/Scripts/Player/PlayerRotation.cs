using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float gamepadDeadZone = 0.4f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 gamepadLookInput;
    
    private Transform targetLockTransform;
    private bool isUsingMouse;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        HandleRotation();
    }

    public void SetLookInput(Vector2 input, InputDevice device)
    {
        isUsingMouse = device is Mouse;
        
        if (!isUsingMouse)
        {
            gamepadLookInput = input;
        }
    }

    public void SetTargetLock(Transform target)
    {
        targetLockTransform = target;
    }

    private void HandleRotation()
    {
        if (targetLockTransform != null)
        {
            RotateTowardsTarget(targetLockTransform.position);
            return;
        }

        if (isUsingMouse)
        {
            HandleMouseRotation();
        }
        else
        {
            HandleGamepadRotation();
        }
    }

    private void HandleMouseRotation()
    {
        if (mainCamera == null) return;

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseScreenPos = mouse.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - transform.position);

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }
    }

    private void HandleGamepadRotation()
    {
        if (gamepadLookInput.sqrMagnitude >= gamepadDeadZone * gamepadDeadZone)
        {
            float angle = Mathf.Atan2(gamepadLookInput.y, gamepadLookInput.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position);
        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }
    }
}
