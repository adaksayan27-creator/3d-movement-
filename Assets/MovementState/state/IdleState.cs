using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.movementspeed = 0f;
        if (movement.anim != null)
        {
            movement.anim.SetBool("Walking", false);
            movement.anim.SetBool("Running", false);
            movement.anim.SetBool("Crouching", false);
        }
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // When C is pressed to crouch
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            movement.SwitchState(movement.crouch);
            return;
        }

        // When movement keys are pressed
        if (movement.hasInput)
        {
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            {
                movement.SwitchState(movement.run);
            }
            else
            {
                movement.SwitchState(movement.walk);
            }
        }
    }
}