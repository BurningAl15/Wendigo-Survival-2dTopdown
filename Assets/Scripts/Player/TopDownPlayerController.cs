using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerRotation))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerTargetLock))]
[RequireComponent(typeof(LivingEntity))]
public class TopDownPlayerController : LivingEntity
{
    private PlayerMovement movement;
    private PlayerRotation rotation;
    private PlayerCombat combat;
    private PlayerTargetLock targetLock;
    private LivingEntity livingEntity;
    private HungerSystem hungerSystem;
    
    private GameInputSystem input;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rotation = GetComponent<PlayerRotation>();
        combat = GetComponent<PlayerCombat>();
        targetLock = GetComponent<PlayerTargetLock>();
        livingEntity = GetComponent<LivingEntity>();
        hungerSystem = GetComponent<HungerSystem>();
        
        input = new GameInputSystem();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;

        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLook;

        input.Player.Attack.performed += OnAttack;
        input.Player.Attack.canceled += OnAttackCanceled;

        input.Player.Sprint.performed += ctx => movement.SetSprint(true);
        input.Player.Sprint.canceled += ctx => movement.SetSprint(false);

        input.Player.Target.performed += ctx => targetLock.SetTargeting(true);
        input.Player.Target.canceled += ctx => targetLock.SetTargeting(false);

        input.Player.Next.performed += ctx => combat.NextWeapon();
        input.Player.Previous.performed += ctx => combat.PreviousWeapon();
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;

        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLook;

        input.Player.Attack.performed -= OnAttack;
        input.Player.Attack.canceled -= OnAttackCanceled;

        input.Disable();
    }

    private void Update()
    {
        bool wantsSprint = movement.IsSprinting;
        livingEntity.UpdateStamina(wantsSprint, Time.deltaTime);

        rotation.SetTargetLock(targetLock.IsTargeting ? targetLock.CurrentTarget : null);
        
        DetectInputDevice();
    }

    private void DetectInputDevice()
    {
        var mouse = Mouse.current;
        var gamepad = Gamepad.current;
    
        if (mouse != null && (mouse.delta.ReadValue().sqrMagnitude > 0.1f || mouse.leftButton.wasPressedThisFrame))
        {
            rotation.SetLookInput(Vector2.zero, mouse);
        }
        else if (gamepad != null && gamepad.rightStick.ReadValue().sqrMagnitude > 0.1f)
        {
            rotation.SetLookInput(gamepad.rightStick.ReadValue(), gamepad);
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        movement.SetMoveInput(ctx.ReadValue<Vector2>());
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        rotation.SetLookInput(ctx.ReadValue<Vector2>(), ctx.control.device);
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        combat.SetAttackHeld(true);
    }

    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        combat.SetAttackHeld(false);
    }
}
