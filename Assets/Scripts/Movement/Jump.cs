using System;
using UnityEngine;
namespace Movement
{
    public class Jump
    {
        private Rigidbody rb;
        private float jumpForce;

        private static bool jumped = false;

        public Jump(Rigidbody rb, float jumpForce) 
        {
            this.rb = rb;
            this.jumpForce = jumpForce;
        }
        public void PerformJump()
        {
            Jumped = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        public void SetJumpForce(float newJumpForce)
        {
            jumpForce = newJumpForce;
        }

        public static Action<bool> onJumped;
        
        public bool Jumped
        {
            get { return jumped; }
            set 
            { 
                if (value != jumped)
                {
                    onJumped?.Invoke(value);
                }
                jumped = value; 
            }
        }
    }
}
