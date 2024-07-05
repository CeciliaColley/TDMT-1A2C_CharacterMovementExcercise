using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CameraControl
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] Transform cameraTarget;
        [SerializeField] Transform rotationTarget;

        private void FixedUpdate()
        {
            transform.position = cameraTarget.position;
            // camera rotation in Y = character's rotation in Y
        }

    }
}

