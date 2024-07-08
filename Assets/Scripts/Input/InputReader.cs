using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2, InputActionPhase> onMovementInput = delegate { };

        public event Action onJumpInput = delegate { };

        public event Action<Vector2> onLook = delegate { };
        public void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            onMovementInput.Invoke(ctx.ReadValue<Vector2>(), ctx.phase);
        }
        public void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                onJumpInput.Invoke();
            }
        }
        public void HandleLookInput(InputAction.CallbackContext ctx)
        {
            onLook.Invoke(ctx.ReadValue<Vector2>());
        }
    }
}

