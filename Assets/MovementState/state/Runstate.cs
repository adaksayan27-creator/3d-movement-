using UnityEngine;
using UnityEngine.InputSystem;

public class Runstate : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.movementspeed = movement.runSpeed;
        if (movement.anim != null)
        {
            movement.anim.SetBool("Walking", true);
            movement.anim.SetBool("Running", true);
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

        if (Keyboard.current != null && !Keyboard.current.leftShiftKey.isPressed)
        {
            movement.SwitchState(movement.walk);
            return;
        }

        if (!movement.hasInput && movement.currentInput.magnitude < 0.05f)
        {
            movement.SwitchState(movement.idle);
            return;
        }

        movement.movementspeed = movement.runSpeed;
    }
}