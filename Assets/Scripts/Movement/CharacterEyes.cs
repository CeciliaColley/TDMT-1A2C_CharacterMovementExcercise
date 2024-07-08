using UnityEngine;

namespace Movement
{
    public class CharacterEyes : MonoBehaviour
    {
        [SerializeField] private Camera characterCamera;
        [SerializeField] private Transform cameraContainer;
        [SerializeField] private Transform orientation;
        [SerializeField] private float sensitivity = 5.0f;
        [SerializeField] private float rotationSmoothTime = 0.1f;

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
            orientation.rotation = Quaternion.Slerp(orientation.rotation, Quaternion.Euler(0, yRotation, 0), rotationSmoothTime);
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
