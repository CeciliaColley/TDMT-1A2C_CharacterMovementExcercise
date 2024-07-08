using UnityEngine;

namespace CameraControl
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float smoothSpeed = 0.125f; // Speed at which the camera follows the target
        [SerializeField] private Vector3 offset; // Offset to maintain relative to the target

        private void LateUpdate()
        {
            // Calculate the desired position with the offset
            Vector3 desiredPosition = cameraTarget.position + offset;
            // Smoothly interpolate towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}

