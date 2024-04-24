using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> onMovementInput = delegate { };
        public void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            onMovementInput.Invoke(ctx.ReadValue<Vector2>());
        }
    }
}

