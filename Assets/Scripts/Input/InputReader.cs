using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2, InputActionPhase> onMovement = delegate { };

        public event Action onJump = delegate { };

        public event Action<Vector2> onLook = delegate { };

        public event Action<InputActionPhase> onSprint = delegate { };
        public void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            onMovement.Invoke(ctx.ReadValue<Vector2>(), ctx.phase);
        }
        public void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                onJump.Invoke();
            }
        }
        public void HandleLookInput(InputAction.CallbackContext ctx)
        {
            onLook.Invoke(ctx.ReadValue<Vector2>());
        }
        public void HandleSprintInput(InputAction.CallbackContext ctx)
        {
            onSprint.Invoke(ctx.phase);
        }
    }
}

