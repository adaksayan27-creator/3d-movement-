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

    // Finite State Machine States
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

        // Disable root motion so animation clip translations don't interfere with CharacterController
        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        // Configure CharacterController to avoid snagging on terrain geometry
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
        // Default state
        SwitchState(idle);
    }

    void Update()
    {
        ReadInput();

        // 1. Update State first so movementspeed and animations match current frame input
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }

        // 2. Apply movement using CharacterController SimpleMove (natively handles gravity, slopes, stepOffset without seam freezing)
        ApplyMovement();

        // 3. Feed input into blend tree
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

        // Direction relative to player facing direction:
        // W = forward, S = backward, D = right, A = left
        Vector3 moveDir = transform.forward * currentInput.y + transform.right * currentInput.x;
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }

        direction = moveDir;

        float speed = movementspeed * Mathf.Clamp01(currentInput.magnitude);
        Vector3 velocity = moveDir * speed;

        // SimpleMove automatically applies gravity and handles slope sliding, ground contact, and stepOffset in PhysX
        isGrounded = controller.SimpleMove(velocity);
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        // Feed horizontal and vertical inputs into 2D strafe blend tree
        anim.SetFloat("horizontalInput", horizontalInput);
        anim.SetFloat("verticalInput", verticalInput);
    }
}