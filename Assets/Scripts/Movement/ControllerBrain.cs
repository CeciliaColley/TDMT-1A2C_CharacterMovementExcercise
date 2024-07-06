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
            Vector3 moveVector = new Vector3(movementInput.x, 0, movementInput.y);
            if (phase == InputActionPhase.Performed)
            {
                body.Move(moveVector);
            }
            if (phase == InputActionPhase.Canceled && moveVector == Vector3.zero)
            {
                body.Break(moveVector);
            }
        }

        private void HandleLook(Vector2 lookInput)
        {
            eyes.Look(lookInput);
        }
    }
}