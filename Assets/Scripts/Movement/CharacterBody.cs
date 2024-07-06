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
        public float maxSpeed;
        public float breakTime;
        public float accelerationDuration;
        public float rotationSpeed;
        public GameObject character;
        public GameObject characterContainer;
        public Rigidbody rb;
        public Transform orientation;

        private Vector3 inputDirection;
        private Vector3 movementDirection;
        private Coroutine rotationLerp;
        private Coroutine acelerationCoroutine;
        private Coroutine decelerationCoroutine;
        private bool maxSpeedReached = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            movementDirection = inputDirection.x * orientation.right + inputDirection.z * orientation.forward;
            if (maxSpeedReached)
            {
                rb.velocity = movementDirection * maxSpeed;
            }  
            characterContainer.transform.rotation = orientation.rotation;
        }

        public void Move(Vector3 direction)
        {
            inputDirection = direction;
            Rotate(direction);
            if (acelerationCoroutine == null && !maxSpeedReached)
            {
                if (decelerationCoroutine != null)
                {
                    StopCoroutine(decelerationCoroutine);
                }
                acelerationCoroutine = StartCoroutine(Accelerate());
            }
        }

        private IEnumerator Accelerate()
        {
            float time = 0.0f;
            float initialSpeed = rb.velocity.magnitude;
            float speed = initialSpeed;
            float secondsToSpeedUp = ((maxSpeed - initialSpeed) * accelerationDuration) / maxSpeed;
            
            while (time <= secondsToSpeedUp)
            {
                float t = time / secondsToSpeedUp;
                speed = Mathf.Lerp(initialSpeed, maxSpeed, t);
                rb.velocity = movementDirection * speed;
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            if (!(time <= secondsToSpeedUp))
            {
                rb.velocity = movementDirection * maxSpeed;
                maxSpeedReached = true;
            }
            
            acelerationCoroutine = null;
        }

        public void Break(Vector3 direction)
        {
            if (decelerationCoroutine != null)
            {
                StopCoroutine(decelerationCoroutine);
            }

            if (direction == Vector3.zero)
            {
                if (acelerationCoroutine != null )
                {
                    StopCoroutine(acelerationCoroutine);
                    acelerationCoroutine = null;
                }
                decelerationCoroutine = StartCoroutine(Decelerate());
            }
            inputDirection = direction;
        }

        private IEnumerator Decelerate()
        {
            maxSpeedReached = false; 
            float time = 0.0f;
            while (rb.velocity.magnitude > 0.1f)
            {
                float t = time / breakTime;
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            rb.velocity = Vector3.zero;

            decelerationCoroutine = null;
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