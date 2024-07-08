using UnityEngine;

namespace Movement
{
    /// <summary>
    /// This class interfaces with the camera and the character to determine their orientation based off of mouse input (or the equivalent control for non PC platforms.).
    /// </summary>
    public class CharacterEyes : MonoBehaviour
    {
        [Tooltip("A reference to the scenes camera")]
        [SerializeField] private Camera characterCamera;
        [Tooltip("A reference to the game object that contains the camera's target. The camera container is intended to be a cilinder with it's mesh deactivated. Since the camera target is it's child, when it rotates on spot the camera target rotates too, maintining a constant distance to the camera containers center.")]
        [SerializeField] private Transform cameraContainer;
        [Tooltip("How sensitive the mouse is.")]
        [SerializeField] private float sensitivity = 5.0f;
        [Tooltip("Percentage of the actual movement to complete per frame. Higher values mean the camera and character's rotation will more accurately (less smoothly) follow the mouse's input.")]
        [Range(0.001f, 1.0f)]
        [SerializeField] private float rotationSmoothTime = 0.1f;

        // Separate orientation references for the camera and the player are used to avoid conflicts between the camera's LateUpdate logic and the player's FixedUpdate movement.
        [Tooltip("A reference to the game object that represents and controls the camera's rotation values. This game object's position should not be updated by anyone else.")]
        [SerializeField] private Transform cameraOrientation;
        [Tooltip("A reference to the game object that represents and controls the player's rotation values. This game object's position should not be updated by anyone else.")]
        [SerializeField] private Transform playerOrientation;
        
        private float xRotation;
        private float yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            Quaternion targetRotation = Quaternion.Euler(xRotation, yRotation, 0);
            characterCamera.transform.rotation = Quaternion.Slerp(characterCamera.transform.rotation, targetRotation, rotationSmoothTime);
            cameraContainer.rotation = Quaternion.Slerp(cameraContainer.rotation, Quaternion.Euler(0, yRotation, 0), rotationSmoothTime);

            // The camera's orientation is updated in LateUpdate, while the player's orientation is updated in FixedUpdate to ensure smooth and consistent behavior.
            cameraOrientation.rotation = Quaternion.Slerp(cameraOrientation.rotation, Quaternion.Euler(0, yRotation, 0), rotationSmoothTime);
        }

        private void FixedUpdate()
        {
            // The camera's orientation is updated in LateUpdate, while the player's orientation is updated in FixedUpdate to ensure smooth and consistent behavior.
            playerOrientation.rotation = Quaternion.Slerp(playerOrientation.rotation, Quaternion.Euler(0, yRotation, 0), rotationSmoothTime);
        }

        public void Look(Vector2 lookInput)
        {
            float mouseX = lookInput.x * sensitivity * Time.deltaTime;
            float mouseY = lookInput.y * sensitivity * Time.deltaTime;
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        }
    }
}
