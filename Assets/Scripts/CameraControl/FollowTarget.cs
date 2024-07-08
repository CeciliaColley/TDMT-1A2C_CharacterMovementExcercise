using UnityEngine;

namespace CameraControl
{
    public class FollowTarget : MonoBehaviour
    {
        
        // WARNING: ALL CAMERA LOGIC IS RUN IN LATE UPDATE.

        [Tooltip("The scenes camera, or the camera intended to follow the target")]
        [SerializeField] private Transform cameraTarget;
        [Tooltip("Speed at which the camera follows the target")]
        [SerializeField] private float smoothSpeed = 0.125f;
       
        // This variable can be used to move the camera realtive the it's target at runtime. 
        private Vector3 offset;

        private void LateUpdate()
        {
            Vector3 desiredPosition = cameraTarget.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}

