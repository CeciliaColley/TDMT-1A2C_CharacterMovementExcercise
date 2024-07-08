
using System.Collections;
using UnityEngine;

namespace Movement
{
    /// <summary>
    /// This class interfaces with rigidBody to control a character's movement.
    /// </summary>

    [RequireComponent(typeof(Rigidbody))]
    public class CharacterBody : MonoBehaviour
    {
        [Header("References for characters displacement")]
        [SerializeField] private float maxSprintSpeed = 7.0f;
        [SerializeField] private float maxSpeed = 5.0f;
        [Tooltip("How many seconds it takes to reach Max Speed")]
        [SerializeField] private float accelerationTime = 5.0f;
        [Tooltip("How many seconds it takes to go from Max Speed to 0")]
        [SerializeField] private float decelerationTime = 5.0f;

        [Header("References for character rotation and orientation")]
        [SerializeField] private GameObject characterContainer;
        [SerializeField] private GameObject character;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform playerOrientation;

        [Header("References for jumping")]
        [SerializeField] private bool directionalJumping = true;
        [SerializeField] private float jumpForce = 7.0f;
        [Tooltip("The radius of the sphere that checks for a collision with the ground. It should be the same radius as the character collider.")]
        [SerializeField] private float groundedSphereRadius = 0.3f;
        [SerializeField] private float groundCheckDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float additionalGravity = 10.0f;
        
        private Rigidbody rb;
        private Displacement displacement;
        private Rotation rotation;
        private Jump jump;
        private Vector3 inputDirection;
        private Vector3 movementDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody component not found!");
                return;
            }
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            displacement = new Displacement(rb, accelerationTime, decelerationTime, this);
            rotation = new Rotation(rotationSpeed, character, playerOrientation, this);
            jump = new Jump(rb, jumpForce);
        }

        private void FixedUpdate()
        {
            movementDirection = inputDirection.x * playerOrientation.right + inputDirection.z * playerOrientation.forward;

            if (IsGrounded())
            {
                rb.velocity = movementDirection * Mathf.Lerp(0, maxSpeed, displacement.SpeedLerpValue) + new Vector3(0, rb.velocity.y, 0);
            }
            else
            {
                rb.AddForce(Vector3.down * additionalGravity, ForceMode.Acceleration);
            }
        }

        private void LateUpdate()
        {
            characterContainer.transform.rotation = playerOrientation.rotation;
        }

        private void OnValidate()
        {
            if (displacement != null)
            {
                displacement.SetAcceleration(accelerationTime);
                displacement.SetDeceleration(decelerationTime);
            }
            if (rotation != null)
            {
                rotation.SetRotationSpeed(rotationSpeed);
            }
            if (jump != null)
            {
                jump.SetJumpForce(jumpForce);
            }
        }

        public void Move(Vector3 direction)
        {
            inputDirection = direction;
            rotation.Rotate(direction);
            displacement.Move();
        }

        public void Accelerate()
        {
            displacement.Accelerate();
        }

        public void Decelerate()
        {
            displacement.Decelerate();
        }

        private bool IsGrounded()
        {
            Vector3 sphereCenter = rb.position + Vector3.up * groundedSphereRadius; // Adjusted center to prevent intersecting with the ground
            return Physics.SphereCast(sphereCenter, groundedSphereRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
        }

        public void PerformJump()
        {
            if (IsGrounded())
            {
                jump.PerformJump();
            }
        }
    }
}
