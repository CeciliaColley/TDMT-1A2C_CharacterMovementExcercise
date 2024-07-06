using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class CharacterEyes : MonoBehaviour
    {
        [SerializeField] private Camera characterCamera;
        [SerializeField] private Transform cameraContainer;

        public float sensitivity;
        public Transform orientation;
        public float xRotation;
        public float yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            characterCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            cameraContainer.rotation = Quaternion.Euler(0, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        public void Look(Vector2 lookInput)
        {
            float mouseX = lookInput.x * Time.fixedDeltaTime * sensitivity;
            float mouseY = lookInput.y * Time.fixedDeltaTime * sensitivity;
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, 0f, 30f);
        }
    }
}
