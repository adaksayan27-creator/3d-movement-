using UnityEngine;
using UnityEngine.InputSystem;

public class Walkstate : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.movementspeed = movement.walkSpeed;
        if (movement.anim != null)
        {
            movement.anim.SetBool("Walking", true);
            movement.anim.SetBool("Running", false);
            movement.anim.SetBool("Crouching", false);
        }
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            movement.SwitchState(movement.crouch);
            return;
        }

        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed && movement.hasInput)
        {
            movement.SwitchState(movement.run);
            return;
        }

        if (!movement.hasInput && movement.currentInput.magnitude < 0.05f)
        {
            movement.SwitchState(movement.idle);
            return;
        }

        movement.movementspeed = movement.walkSpeed;
    }
}