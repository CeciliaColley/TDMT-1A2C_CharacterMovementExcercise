using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CameraControl
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 2.0f, -5.0f);
        [SerializeField] private float speed = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;
        [SerializeField] private float minimumDistanceToRotate = 0.1f;

        private void Awake()
        {
            if (!target)
            {
                Debug.LogError($"{name}: Target is null!");
                enabled = false;
            }
        }
        
        // If you move a camera in update it's clippy and jumpy, so use LateUpdate or FixedUpdate instead., this is how unity's cinemachine works.
        private void FixedUpdate()
        {
            var offsetPosition = target.TransformPoint(offset);
            // In fixed update you need to use fixedDeltaTime
            transform.position = Vector3.Lerp(transform.position, offsetPosition, Time.fixedDeltaTime * speed);

            if (Vector3.Distance(transform.position, offsetPosition) < minimumDistanceToRotate)
                return;

            var desiredRotation = target.rotation * Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * rotationSpeed);
            // The difference between lerp and slerp is that slerp follows a circumference for the change, and lerp follows a line.
        }
    }
}

