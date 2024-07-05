using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movement
{
    /// <summary>
    /// This class interfaces with rigidBody to control a character's movement through forces
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterBody : MonoBehaviour
    {
        public float breakForce;
        public float acceleration;
        public float rotationSpeed;
        public GameObject character;
        public GameObject characterContainer;
        public Rigidbody rb;
        public Transform orientation;

        private float movementForce;
        private Vector3 inputDirection;
        private Vector3 movementDirection;
        private float _drag;
        private Coroutine rotationLerp;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            _drag = rb.drag;
            movementForce = rb.mass * acceleration;
        }

        private void FixedUpdate()
        {
            movementDirection = inputDirection.x * orientation.right + inputDirection.z * orientation.forward;
            rb.AddForce(movementDirection * movementForce, ForceMode.Acceleration);
            characterContainer.transform.rotation = orientation.rotation;
        }

        public void Accelerate(Vector3 direction)
        {
            if (rb.drag != _drag)
            {
                rb.drag = _drag;
            }
            inputDirection = direction;
            Rotate(direction);
        }

        public void Decelerate(Vector3 direction)
        {
            rb.drag = breakForce;
            inputDirection = direction;
        }

        private void Rotate(Vector3 direction)
        {
            Vector3 orientedDirection = orientation.TransformDirection(direction);
            float rotationAmount = Mathf.Atan2(orientedDirection.x, orientedDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, rotationAmount, 0);
            if (rotationLerp != null)
            {
                StopCoroutine(rotationLerp);
            }
            rotationLerp = StartCoroutine(RotationLerp(targetRotation));
        }


        private IEnumerator RotationLerp(Quaternion targetRotation)
        {
            float time = 0;
            Quaternion initialRotation = character.transform.rotation;

            while (time < rotationSpeed)
            {
                float t = time / rotationSpeed;
                character.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            character.transform.rotation = targetRotation;
        }

    }

}