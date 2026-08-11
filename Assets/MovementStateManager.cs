using UnityEngine;
using UnityEngine.InputSystem;

public class MovementStateManager : MonoBehaviour
{
    public float movementspeed = 3;
    public float inputSmoothTime = 0.1f;
    [HideInInspector] public Vector3 direction;
    float horizontalInput, verticalInput;
    Vector2 currentInput;
    Vector2 inputVelocity;
    CharacterController controller;
    [SerializeField]float groundYOffset;
    [SerializeField]LayerMask groundMask;
    Vector3 spherePosition;
    [SerializeField]float gravity = -9.81f;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionAndMove();
        Gravity();
    }

    void GetDirectionAndMove()
    {
        Vector2 targetInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) targetInput.y += 1;
            if (Keyboard.current.sKey.isPressed) targetInput.y -= 1;
            if (Keyboard.current.dKey.isPressed) targetInput.x += 1;
            if (Keyboard.current.aKey.isPressed) targetInput.x -= 1;
        }

        currentInput = Vector2.SmoothDamp(currentInput, targetInput, ref inputVelocity, inputSmoothTime);

        horizontalInput = currentInput.x;
        verticalInput = currentInput.y;
        direction = transform.forward * verticalInput + transform.right * horizontalInput;
        controller.Move(direction * movementspeed * Time.deltaTime);
    }

    bool IsGrounded()
    {
        spherePosition = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if(Physics.CheckSphere(spherePosition, 0.1f, groundMask))
        {
            return true;
        }
        return false;
    }
    void Gravity()
    {
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if(velocity.y < 0)
        {
            velocity.y = -2f;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spherePosition, 0.1f);
    }

    
}