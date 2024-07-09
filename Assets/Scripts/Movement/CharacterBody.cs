
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
        [SerializeField] private float maxSprintSpeed = 15.0f;
        [SerializeField] private float maxSpeed = 10.0f;
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
        [SerializeField] private float jumpForce = 7.0f;
        [Tooltip("The radius of the sphere that checks for a collision with the ground. It should be the same radius as the character collider.")]
        [SerializeField] private float groundedSphereRadius = 0.3f;
        [SerializeField] private float groundCheckDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float additionalGravity = 10.0f;

        [Header("Variables for this bodies limits.")]
        [SerializeField] private float maxWalkableSlopeAngle = 60.0f;

        private Rigidbody rb;
        private Displacement displacement;
        private Rotation rotation;
        private Jump jump;
        private Vector3 inputDirection;
        private Vector3 movementDirection;
        private float _maxSpeed;

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
            displacement = new Displacement(accelerationTime, decelerationTime, this);
            rotation = new Rotation(rotationSpeed, character, playerOrientation, this);
            jump = new Jump(rb, jumpForce);
            _maxSpeed = maxSpeed;
        }

        private void FixedUpdate()
        {
            movementDirection = inputDirection.x * playerOrientation.right + inputDirection.z * playerOrientation.forward;

            float slopeMultiplier = CalculateSlopeMultiplier();
            rb.velocity = (movementDirection * Mathf.Lerp(0, _maxSpeed, displacement.SpeedLerpValue) + new Vector3(0, rb.velocity.y, 0)) * slopeMultiplier;

            if (!IsGrounded())
            {
                rb.AddForce(Vector3.down * additionalGravity, ForceMode.Acceleration);
            }
            else if (jump.Jumped)
            {
                jump.Jumped = false;
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
            // Adjusted center to prevent intersecting with the ground
            Vector3 sphereCenter = rb.position + Vector3.up * groundedSphereRadius;
            return Physics.SphereCast(sphereCenter, groundedSphereRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
        }

        public void PerformJump()
        {
            if (IsGrounded())
            {
                jump.PerformJump();
            }
        }

        //In the future, the following function can be moved into it's own class, just like displacement and jump, for more complex slope management.
        /// <summary>
        /// This function tries to cast a ray onto the floor.
        /// If succesfull, it calculates the angle between a vertical line, and the floors normal.
        /// Then, it lerps between 1 and 0 depending on the resulting angle.
        /// Angles close to maxWalkableSlopeAngle return 0, and angles close to 0 return 1
        /// If it was unable to find the floor, it defaults to 1
        /// The resulting number is intended to be multiplied with the characters rigidbody properties, like velocity, to reduce them depending on the slope.
        /// </summary>
        /// <returns></returns>
        private float CalculateSlopeMultiplier()
        {
            RaycastHit hit;
            if (Physics.Raycast(rb.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                // Get the angle between the hit normal and a vertical line
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle > maxWalkableSlopeAngle)
                {
                    return 0.0f;
                }
                // Lerp between 1 and 0, where angles close to maxWalkableSlopeAngle return 0, and angles close to 0 return 1
                return Mathf.InverseLerp(maxWalkableSlopeAngle, 0, angle);
            }
            // Default to 1 if no ground detected 
            return 1.0f; 
        }

        public void Sprint(bool performed)
        {
            if (performed)
            {
                _maxSpeed = maxSprintSpeed;
            }
            else
            {
                _maxSpeed = maxSpeed;
            }
        }

        // STUDENT NOTE:
        // I HAVE USED THIS METHOD ON THE BIG STAIRS (THE BRIGHT PINK ONES THAT COME OFF THE PLATFORM) TO SHOWCASE HOW IT CHANGES THE MECHANIC.
        // IT MAKES THE PLAYER STOP WHERE THEY LAND, MAKING THE GAME A BIT EASIER.
        // ITS UP TO THE GAME DESIGNER WHETHER THEY WANT TO USE THIS MODE, OR THE MORE DIFFICULT MODE.
        // IF THEY DON'T WANT TO USE IT, THEN JUST KEEP THE STAIRS IN THE FLOOR LAYER COMPLETELY.
        // IT'S A NICE OPTION TO HAVE.

        /// <summary>
        /// This function stops the game object when they collide with another object that isn't the floor. 
        /// This is useful because it can make the game more precise. 
        /// The player will stop exactly where they land, as long as the objec they're landing on has a collider that ISN'T on the floor.
        /// In other words, to get the player to stop where they land, just add a collider to the game object that isn't part of the floor layer.
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            if (!IsGrounded() || (groundLayer.value & (1 << collision.gameObject.layer)) == 0)
            {
                inputDirection = Vector3.zero;
            }
        }
    }
}
