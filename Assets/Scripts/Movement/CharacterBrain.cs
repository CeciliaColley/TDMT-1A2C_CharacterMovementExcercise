using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class ControllerBrain : MonoBehaviour
    {
        /*************************************************************************** VARIABLES ***************************************************************/

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

        /*************************************************************************** SUBSCRIBING TO EVENTS ***************************************************************/
        private void OnEnable()
        {
            inputReader.onMovement += HandleMovement;
            inputReader.onLook += HandleLook;
            inputReader.onJump += HandleJump;
            inputReader.onSprint += HandleSprint;
        }

        private void OnDisable()
        {
            inputReader.onMovement -= HandleMovement;
            inputReader.onLook -= HandleLook;
            inputReader.onJump -= HandleJump;
            inputReader.onSprint -= HandleSprint;
        }

        /*************************************************************************** HANDLING INPUT ***************************************************************/

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

        private void HandleJump()
        {
            body.PerformJump();
        }

        private void HandleSprint(InputActionPhase phase)
        {
            switch (phase)
            {
                case InputActionPhase.Performed:
                    body.Sprint(true);
                    break;
                case InputActionPhase.Canceled:
                    body.Sprint(false);
                    break;
                default: break;
            }

        }
    }
}