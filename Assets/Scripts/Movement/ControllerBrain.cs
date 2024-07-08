using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class ControllerBrain : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CharacterBody body;
        [SerializeField] private CharacterEyes eyes;

        private void Start()
        {
            if (!inputReader)
            {
                Debug.LogError($"{name}: {nameof(inputReader)} is null!");
                return;
            }
            
        }

        private void OnEnable()
        {
            inputReader.onMovementInput += HandleMovement;
            inputReader.onLook += HandleLook;
        }

        private void OnDisable()
        {
            inputReader.onMovementInput -= HandleMovement;
            inputReader.onLook -= HandleLook;
        }

        private void HandleMovement(Vector2 movementInput, InputActionPhase phase)
        {
            Vector3 inputDirection = new Vector3(movementInput.x, 0, movementInput.y);
            if (phase == InputActionPhase.Performed)
            {
                body.Move(inputDirection);
            }
            if (phase == InputActionPhase.Canceled && inputDirection == Vector3.zero)
            {
                body.Decelerate();
            }
        }

        private void HandleLook(Vector2 lookInput)
        {
            eyes.Look(lookInput);
        }
    }
}