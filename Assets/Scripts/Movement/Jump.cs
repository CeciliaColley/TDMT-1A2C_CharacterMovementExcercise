using System;
using UnityEngine;
namespace Movement
{
    public class Jump
    {
        /*************************************************************************** VARIABLES ***************************************************************/
        private Rigidbody rb;
        private float jumpForce;

        /*************************************************************************** OBSERVED VARIABLES ***************************************************************/

        // This variable is observed to invoke jump specific events.
        private static bool jumped = false;
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

        /*************************************************************************** CONSTRUCTOR ***************************************************************/
        public Jump(Rigidbody rb, float jumpForce) 
        {
            this.rb = rb;
            this.jumpForce = jumpForce;
        }
        /*************************************************************************** FUNCTIONS ***************************************************************/
        public void PerformJump()
        {
            Jumped = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        /*************************************************************************** GETTERS AND SETTERS ***************************************************************/
        public void SetJumpForce(float newJumpForce)
        {
            jumpForce = newJumpForce;
        }

    }
}
