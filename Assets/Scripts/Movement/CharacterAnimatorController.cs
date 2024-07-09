using System;
using Input;
using UnityEngine;

namespace Movement
{
    public class CharacterAnimatorView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private CharacterBody body;

        [Header("Animator Parameters")]
        [SerializeField] private string jumpTriggerParameter = "Jumping";
        [SerializeField] private string isFallingParameter = "is_falling";
        [SerializeField] private string horSpeedParameter = "hor_speed";

        private void Start()
        {
            if (animator)
            {
                animator.SetBool(isFallingParameter, false);
            }
        }

        private void OnEnable()
        {
            Jump.onJumped += HandleJump;
        }

        private void OnDisable()
        {
            Jump.onJumped -= HandleJump;
        }

        private void Update()
        {
            if (!rigidBody || !animator || !body) { return; }

            // Get the horizontal speed for the blend tree
            Vector3 velocity = rigidBody.velocity;
            velocity.y = 0;
            float speed = velocity.magnitude;

            // Update the animator with the horizontal speed
            animator.SetFloat(horSpeedParameter, speed);
        }

        private void HandleJump(bool jumpedValue)
        {
            if (animator)
            {
                animator.SetBool(isFallingParameter, jumpedValue);
            }
            if (animator && jumpedValue == true)
            {
                animator.SetTrigger(jumpTriggerParameter);
            }
        }
    }
}