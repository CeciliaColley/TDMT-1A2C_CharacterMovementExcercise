
using UnityEngine;

namespace Movement
{
    ///// <summary>
    ///// This class interfaces with rigidBody to control a character's movement through forces
    ///// </summary>
    //[RequireComponent(typeof(Rigidbody))]
    //public class CharacterBody : MonoBehaviour
    //{
    //    public float maxSpeed;
    //    public float breakTime;
    //    public float accelerationDuration;
    //    public float rotationSpeed;
    //    public GameObject character;
    //    public GameObject characterContainer;
    //    public Rigidbody rb;
    //    public Transform orientation;

    //    private Vector3 inputDirection;
    //    private Vector3 movementDirection;
    //    private Coroutine rotationLerp;
    //    private Coroutine acelerationCoroutine;
    //    private Coroutine decelerationCoroutine;
    //    private bool maxSpeedReached = false;

    //    private void Start()
    //    {
    //        rb = GetComponent<Rigidbody>();
    //        rb.freezeRotation = true;

    //        // Ensure drag values are set correctly
    //        rb.drag = 0f;
    //        rb.angularDrag = 0.05f;
    //    }

    //    private void FixedUpdate()
    //    {
    //        movementDirection = inputDirection.x * orientation.right + inputDirection.z * orientation.forward;

    //        // Only apply velocity if the character is on the ground or accelerating
    //        if (maxSpeedReached && IsGrounded())
    //        {
    //            rb.velocity = movementDirection * maxSpeed;
    //        }
    //        //else
    //        //{
    //        //    // Allow gravity to take effect
    //        //    rb.velocity += Physics.gravity * Time.fixedDeltaTime;
    //        //    Debug.Log("Not grounded.");
    //        //}

    //        characterContainer.transform.rotation = orientation.rotation;
    //    }

    //    private bool IsGrounded()
    //    {
    //        // Adjust this method to check if the character is on the ground
    //        // Example: Use a raycast to check for ground collision
    //        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    //    }

    //    public void Move(Vector3 direction)
    //    {
    //        inputDirection = direction;
    //        Rotate(direction);
    //        if (acelerationCoroutine == null && !maxSpeedReached)
    //        {
    //            if (decelerationCoroutine != null)
    //            {
    //                StopCoroutine(decelerationCoroutine);
    //            }
    //            acelerationCoroutine = StartCoroutine(Accelerate());
    //        }
    //    }

    //    private IEnumerator Accelerate()
    //    {
    //        float time = 0.0f;
    //        float initialSpeed = rb.velocity.magnitude;
    //        float speed = initialSpeed;
    //        float secondsToSpeedUp = ((maxSpeed - initialSpeed) * accelerationDuration) / maxSpeed;

    //        while (time <= secondsToSpeedUp)
    //        {
    //            Debug.Log("A");
    //            float t = time / secondsToSpeedUp;
    //            speed = Mathf.Lerp(initialSpeed, maxSpeed, t);
    //            rb.velocity = movementDirection * speed;
    //            time += Time.fixedDeltaTime;
    //            yield return new WaitForFixedUpdate();
    //        }

    //        rb.velocity = movementDirection * maxSpeed;
    //        maxSpeedReached = true;
    //        acelerationCoroutine = null;
    //    }

    //    public void Break(Vector3 direction)
    //    {
    //        if (decelerationCoroutine != null)
    //        {
    //            StopCoroutine(decelerationCoroutine);
    //        }

    //        if (direction == Vector3.zero)
    //        {
    //            if (acelerationCoroutine != null)
    //            {
    //                StopCoroutine(acelerationCoroutine);
    //                acelerationCoroutine = null;
    //            }
    //            decelerationCoroutine = StartCoroutine(Decelerate());
    //        }
    //        inputDirection = direction;
    //    }

    //    private IEnumerator Decelerate()
    //    {
    //        maxSpeedReached = false;
    //        float time = 0.0f;
    //        while (rb.velocity.magnitude > 0.1f)
    //        {
    //            float t = time / breakTime;
    //            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, t);
    //            time += Time.fixedDeltaTime;
    //            yield return new WaitForFixedUpdate();
    //        }
    //        rb.velocity = Vector3.zero;
    //        decelerationCoroutine = null;
    //    }

    //    private void Rotate(Vector3 direction)
    //    {
    //        Vector3 orientedDirection = orientation.TransformDirection(direction);
    //        float rotationAmount = Mathf.Atan2(orientedDirection.x, orientedDirection.z) * Mathf.Rad2Deg;
    //        Quaternion targetRotation = Quaternion.Euler(0, rotationAmount, 0);
    //        if (rotationLerp != null)
    //        {
    //            StopCoroutine(rotationLerp);
    //        }
    //        rotationLerp = StartCoroutine(RotationLerp(targetRotation));
    //    }

    //    private IEnumerator RotationLerp(Quaternion targetRotation)
    //    {
    //        float time = 0;
    //        Quaternion initialRotation = character.transform.rotation;

    //        while (time < rotationSpeed)
    //        {
    //            float t = time / rotationSpeed;
    //            character.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
    //            time += Time.fixedDeltaTime;
    //            yield return new WaitForFixedUpdate();
    //        }
    //        character.transform.rotation = targetRotation;
    //    }
    //}

    [RequireComponent(typeof(Rigidbody))]
    public class CharacterBody : MonoBehaviour
    {
        [Header("References for characters displacement")]
        [SerializeField] private float maxSprintSpeed = 7.0f;
        [SerializeField] private float maxSpeed = 5.0f;
        [Tooltip("How many seconds it takes to reach Max Speed")]
        [SerializeField] private float acceleration = 5.0f;
        [Tooltip("How many seconds it takes to go from Max Speed to 0")]
        [SerializeField] private float deceleration = 5.0f;

        [Header("References for character rotation and orientation")]
        [SerializeField] private GameObject characterContainer;
        [SerializeField] private GameObject character;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform orientation;

        private Rigidbody rb;
        private Displacement displacement;
        private Rotation rotation;
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
            displacement = new Displacement(rb, acceleration, deceleration, this);
            rotation = new Rotation(rotationSpeed, character, orientation, this);
        }

        private void FixedUpdate()
        {
            movementDirection = inputDirection.x * orientation.right + inputDirection.z * orientation.forward;
            rb.velocity = movementDirection * Mathf.Lerp(0, maxSpeed, displacement.SpeedLerpValue);
            characterContainer.transform.rotation = orientation.rotation;
        }

        private void OnValidate()
        {
            if (displacement != null)
            {
                displacement.SetAcceleration(acceleration);
                displacement.SetDeceleration(deceleration);
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
    }
}
