using UnityEngine;

namespace Movement
{
    public class RotateCharacterBasedOnVelocity : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private float rotationSpeed = 1;
        [SerializeField] private float minimumSpeedForRotation = 0.01f;

        private void Update()
        {
            var velocity = rigidBody.velocity;
            velocity.y = 0;
            if (velocity.magnitude < minimumSpeedForRotation)
                return;

            var rotationAngle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
            transform.Rotate(Vector3.up, rotationAngle * rotationSpeed * Time.deltaTime);
        }
    }
}