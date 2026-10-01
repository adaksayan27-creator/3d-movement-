using UnityEngine;
using UnityEngine.InputSystem;

public class Crouchstate : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.movementspeed = movement.crouchSpeed;
        if (movement.anim != null)
        {
            movement.anim.SetBool("Walking", false);
            movement.anim.SetBool("Running", false);
            movement.anim.SetBool("Crouching", true);
        }
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // Press C to toggle out of crouch
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (movement.hasInput)
            {
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    movement.SwitchState(movement.run);
                }
                else
                {
                    movement.SwitchState(movement.walk);
                }
            }
            else
            {
                movement.SwitchState(movement.idle);
            }
            return;
        }

        // Sprinting breaks out of crouch
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed && movement.hasInput)
        {
            movement.SwitchState(movement.run);
            return;
        }

        movement.movementspeed = movement.crouchSpeed;
    }
}