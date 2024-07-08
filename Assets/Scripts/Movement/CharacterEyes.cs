using UnityEngine;

namespace Movement
{
    public class CharacterEyes : MonoBehaviour
    {
        [SerializeField] private Camera characterCamera;
        [SerializeField] private Transform cameraContainer;
        [SerializeField] private float sensitivity = 5.0f;
        [SerializeField] private float rotationSmoothTime = 0.1f;

        // Separate orientation references for the camera and the player are used to avoid conflicts between the camera's LateUpdate logic and the player's FixedUpdate movement.
        [SerializeField] private Transform cameraOrientation;
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
