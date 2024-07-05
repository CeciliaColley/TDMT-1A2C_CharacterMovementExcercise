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

        private Vector3 movementForce;
        private float _drag;
        private Coroutine rotationLerp;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            rb.AddForce(movementForce.x*orientation.right + movementForce.z*orientation.forward, ForceMode.Acceleration);
            characterContainer.transform.rotation = orientation.rotation;
        }

        public void Accelerate(Vector3 direction)
        {
            Rotate(direction);
            if (rb.drag != _drag)
            {
                rb.drag = _drag;
            }
            movementForce = direction.normalized * rb.mass * acceleration;
        }

        public void Decelerate(Vector3 direction)
        {
            rb.drag = breakForce;
            movementForce = direction.normalized * rb.mass * acceleration;
            Rotate(direction);
        }

        private void Rotate(Vector3 direction)
        {
            float rotationAmount = Mathf.Asin(direction.x) * Mathf.Rad2Deg; ;
            if (direction.z < 0)
            {
                rotationAmount = (rotationAmount + 180) * -1;
            }
            Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);
            if (rotationLerp != null)
            {
                StopCoroutine(rotationLerp);
            }
            rotationLerp = StartCoroutine(RotationLerp(rotation));
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