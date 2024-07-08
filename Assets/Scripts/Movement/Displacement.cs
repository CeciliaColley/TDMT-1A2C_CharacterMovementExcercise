using System.Collections;
using UnityEngine;

namespace Movement
{
    public class Displacement
    {
        /* 
         * Index:
         *   - Variables
         *   - Constructors
         *   - Functions
         *   - Getters and setters
         *   - Coroutines
         */

        /*************************************************************************** VARIABLES ***************************************************************/
        private float acceleration;
        private float deceleration;
        private float speedLerpValue = 0.0f;
        private Vector3 currentMovement = Vector3.zero;
        private bool isAccelerating = false;
        private bool isDecelerating = false;
        private Coroutine lerpSpeedDownCoroutine;
        private Coroutine lerpSpeedUpCoroutine;
        private MonoBehaviour monoBehaviour;


        /*************************************************************************** CONSTRUCTORS ***************************************************************/
        public Displacement(float acceleration, float deceleration, MonoBehaviour monoBehaviour)
        {
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.monoBehaviour = monoBehaviour;
        }

        /*************************************************************************** FUNCTIONS ***************************************************************/
        public void Move()
        {
            Accelerate();
        }

        public void Accelerate()
        {
            AdjustVelocity(ref isAccelerating, ref isDecelerating, ref lerpSpeedUpCoroutine, ref lerpSpeedDownCoroutine, LerpSpeedUp());
        }

        public void Decelerate()
        {
            AdjustVelocity(ref isDecelerating, ref isAccelerating, ref lerpSpeedDownCoroutine, ref lerpSpeedUpCoroutine, LerpSpeedDown());
        }


        public void AdjustVelocity(ref bool doingWantedAcceleration, ref bool oppositeAcceleration, ref Coroutine wantedAccelerationCoroutine, ref Coroutine oppositeAccelerationCoroutine, IEnumerator wantedLerpMethod)
        {
            // Check if the character is already doing the aceleration you want it to, to prevent activating it twice
            if (!doingWantedAcceleration)
            {
                // Stop the opposite type of aceleration if it's happenning
                if (oppositeAccelerationCoroutine != null)
                {
                    monoBehaviour.StopCoroutine(oppositeAccelerationCoroutine);
                    oppositeAccelerationCoroutine = null;
                    oppositeAcceleration = false;
                }
                // Start the wanted aceleration and keep track of the coroutine to stop it later.
                wantedAccelerationCoroutine = monoBehaviour.StartCoroutine(wantedLerpMethod);
            }
        }

        /*************************************************************************** GETTERS AND SETTERS ***************************************************************/

        //Getters
        public Vector3 CurrentMovement
        {
            get { return currentMovement; }
        }

        public float SpeedLerpValue
        {
            get { return speedLerpValue; }
        }

        // Setters
        public void SetAcceleration(float newAcceleration)
        {
            acceleration = newAcceleration;
        }

        public void SetDeceleration(float newDeceleration)
        {
            deceleration = newDeceleration;
        }

        /*************************************************************************** COROUTINES ***************************************************************/

        // WARNING:
        // The logic in LerpSpeedUp and LerpSpeedDown follows the same pattern, but the values used are almost completely different. It may look like repeated code, but it's not.
        // Do not try to use them interchangeably, it won't work.
        // Leaving them as seperate coroutines, rather than refactoring them into one, improves readability and keeps the door open for accelerate-decelerate specific modifications in the future.

        private IEnumerator LerpSpeedUp()
        {
            // Innitialize the values for the lerp
            // By setting isDecelerating to false, any ongoing deceleration coroutine will be terminated, since the deceleration's coroutine loop depends on isDecelerating being true.
            isAccelerating = true;
            isDecelerating = false;
            float time = 0.0f;

            // Check if the character is accelerating from 0, or is already starting with an initial velocity
            float initialSpeedLerpValue = speedLerpValue;
            // Check how many seconds it would take for the character to reach maximum velocity, taking into account their initial velocity.
            float secondsToSpeedUp = speedLerpValue != 0 ? acceleration - (speedLerpValue * acceleration) : acceleration;

            // Lerp the character's velocity up towards max velocity, for as long as they are accelerating.
            // When the user cancels the movement (stops pressing the button) the decelerate method is called which sets isAccelerating false and stops this coroutine.
            // If deceleration doesn't set "isAccelerating" to false, the acceleration will continue at the same time as deceleration, leading to unexpected behaviour.
            // There's a fail safe implemented in the "Accelerate" function that will only start another coroutine if isAccelerating is false, just in case someone accidentally removes this condition.
            while (time <= secondsToSpeedUp && isAccelerating)
            {
                speedLerpValue = initialSpeedLerpValue + ((time / secondsToSpeedUp) * (1 - initialSpeedLerpValue));
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // If the lerp completed set to the max (1)
            // I'm just checking if the while was exited because of the first condition (Was the lerp completed?)
            if (!(time <= secondsToSpeedUp))
            {
                speedLerpValue = 1;
            }
        }

        private IEnumerator LerpSpeedDown()
        {
            // Innitialize the values for the lerp
            // By setting isAccelerating to false, any ongoing acceleration coroutine will be terminated, since the acceleration's coroutine loop depends on isAccelerating being true.
            isDecelerating = true;
            isAccelerating = false;
            float time = 0.0f;

            // Check if the character is decelerating from a max lerp value of 1, or is decelerating from a non max velocity
            float initialSpeedLerpValue = speedLerpValue;
            // Check how many seconds it would take for the character to slow down, taking into account their initial velocity.
            float secondsToSlowDown = (speedLerpValue != 0) ? deceleration * initialSpeedLerpValue : deceleration;

            // Lerp the character's velocity down towards 0, for as long as they are decelerating.
            // When the user tries to move again (presses a button) the acelerate method is called which sets isDecelerating false and stops this coroutine.
            // If Acceleration doesn't set "isDecelerating" to false, the deceleration will continue at the same time as aceleration, leading to unexpected behaviour.
            // There's a fail safe implemented in the "Decelerate" function that will only start another coroutine if isDecelerating is false, just in case someone accidentally removes this condition.
            while (time <= secondsToSlowDown && isDecelerating)
            {
                speedLerpValue = initialSpeedLerpValue - ((time / secondsToSlowDown) * initialSpeedLerpValue);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // If the lerp completed set to the min (0)
            // I'm just checking if the while was exited because of the first condition (Was the lerp completed?)
            if (!(time <= secondsToSlowDown))
            {
                speedLerpValue = 0;
            }
        }
    }
}
