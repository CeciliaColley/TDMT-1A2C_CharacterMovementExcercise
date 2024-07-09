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

        private void OnEnable()
        {
            inputReader.onJump += HandleJump;
        }

        private void OnDisable()
        {
            inputReader.onJump -= HandleJump;
        }

        private void Update()
        {
            if (!rigidBody || !animator || !body) { return; }

            // Get the horizontal speed
            Vector3 velocity = rigidBody.velocity;
            velocity.y = 0;
            float speed = velocity.magnitude;

            // Update the animator with the horizontal speed
            animator.SetFloat(horSpeedParameter, speed);

            // Update the animator's falling state
            animator.SetBool(isFallingParameter, Jump.jumped);
        }

        private void HandleJump()
        {
            if (animator)
            {
                animator.SetTrigger(jumpTriggerParameter);
            }
        }
    }
}