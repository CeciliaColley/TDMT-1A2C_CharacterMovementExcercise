using UnityEngine;
using System.Collections;

namespace Movement
{
    public class Rotation
    {
        private Transform orientation;
        private GameObject character;

        private Coroutine rotationLerp;
        private float rotationSpeed;
        private MonoBehaviour monoBehaviour;

        public Rotation(float rotationSpeed, GameObject character, Transform orientation,  MonoBehaviour monoBehaviour)
        {
            this.rotationSpeed = rotationSpeed;
            this.character = character;
            this.orientation = orientation;
            this.monoBehaviour = monoBehaviour;
        }

        public void SetRotationSpeed(float newRotationSpeed)
        {
            rotationSpeed = newRotationSpeed;
        }

        public void Rotate(Vector3 direction)
        {
            Vector3 orientedDirection = orientation.TransformDirection(direction);
            float rotationAmount = Mathf.Atan2(orientedDirection.x, orientedDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, rotationAmount, 0);
            if (rotationLerp != null)
            {
                monoBehaviour.StopCoroutine(rotationLerp);
            }
            rotationLerp = monoBehaviour.StartCoroutine(RotationLerp(targetRotation));
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