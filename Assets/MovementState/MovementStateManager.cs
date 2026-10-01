using UnityEngine;
using UnityEngine.InputSystem;

public class MovementStateManager : MonoBehaviour
{
    [Header("Movement Speeds")]
    [HideInInspector] public float movementspeed;
    public float walkSpeed = 3.5f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2f;
    public float walkBackSpeed = 2.5f;
    public float runBackSpeed = 4.5f;
    public float crouchBackSpeed = 1.5f;

    [Header("Input Smoothing")]
    public float inputSmoothTime = 0.08f;
    [HideInInspector] public float horizontalInput, verticalInput;
    [HideInInspector] public Vector2 currentInput;
    [HideInInspector] public Vector2 rawInput;
    [HideInInspector] public bool hasInput;
    private Vector2 inputVelocity;

    [Header("Ground & Movement Status")]
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector3 direction;

    [Header("References")]
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public AimStateManager aimState;

    private MovementBaseState currentState;
    public IdleState idle = new IdleState();
    public Walkstate walk = new Walkstate();
    public Runstate run = new Runstate();
    public Crouchstate crouch = new Crouchstate();

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        aimState = GetComponent<AimStateManager>();

        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        if (controller != null)
        {
            controller.stepOffset = 0.3f;
            controller.slopeLimit = 45f;
            controller.skinWidth = 0.05f;
            controller.minMoveDistance = 0.001f;
        }
    }

    void Start()
    {
        SwitchState(idle);
    }

    void Update()
    {
        ReadInput();

        if (currentState != null)
        {
            currentState.UpdateState(this);
        }

        ApplyMovement();

        UpdateAnimator();
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void ReadInput()
    {
        rawInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) rawInput.y += 1f;
            if (Keyboard.current.sKey.isPressed) rawInput.y -= 1f;
            if (Keyboard.current.dKey.isPressed) rawInput.x += 1f;
            if (Keyboard.current.aKey.isPressed) rawInput.x -= 1f;
        }

        hasInput = rawInput.sqrMagnitude > 0.01f;

        Vector2 targetInput = rawInput.sqrMagnitude > 1f ? rawInput.normalized : rawInput;

        currentInput = Vector2.SmoothDamp(currentInput, targetInput, ref inputVelocity, inputSmoothTime);

        if (!hasInput && currentInput.sqrMagnitude < 0.0001f)
        {
            currentInput = Vector2.zero;
        }

        horizontalInput = currentInput.x;
        verticalInput = currentInput.y;
    }

    void ApplyMovement()
    {
        if (controller == null || !controller.enabled) return;

        Vector3 moveDir = transform.forward * currentInput.y + transform.right * currentInput.x;
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }

        direction = moveDir;

        float speed = movementspeed * Mathf.Clamp01(currentInput.magnitude);
        Vector3 velocity = moveDir * speed;

        isGrounded = controller.SimpleMove(velocity);
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("horizontalInput", horizontalInput);
        anim.SetFloat("verticalInput", verticalInput);
    }
}