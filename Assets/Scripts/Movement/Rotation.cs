using UnityEngine;
using System.Collections;

namespace Movement
{
    /// <summary>
    /// This class points the character in the direction of the input.
    /// When the player presses the forward key, the character will rotate to face forward, same with sideways, backwards, and diagonal.
    /// This class DOES NOT determine the actual movement vector. This class only determines the direction that the character faces, NOT the direction the character moves in.
    /// Displacement, which is, by definition, a vector that denotes the change in position of an object, determines the direction the character moves in.
    /// </summary>
    public class Rotation
    {
        /* 
         * Index:
         *   - Variables
         *   - Constructors
         *   - Getters and setters
         *   - Functions
         *   - Coroutines
         */

        /*************************************************************************** VARIABLES ***************************************************************/
        
        private Transform orientation; // A reference to the player orientation object. Remeber that this objects position and orientation should not be modified by any other object.
        private GameObject character; // A reference to the actual character's model. This is what is going to be rotated.
        private float rotationSpeed; // The speed in seconds at which the character will rotate. In the future, this could be modified to take into account the length of the rotation, but for now, the character will always rotate at this speed no matter if they are doing half a rotation or a whole rotation.
        private Coroutine rotationLerp;
        private MonoBehaviour monoBehaviour;

        /*************************************************************************** CONSTRUCTORS ***************************************************************/
        public Rotation(float rotationSpeed, GameObject character, Transform orientation,  MonoBehaviour monoBehaviour)
        {
            this.rotationSpeed = rotationSpeed;
            this.character = character;
            this.orientation = orientation;
            this.monoBehaviour = monoBehaviour;
        }

        /*************************************************************************** GETTERS AND SETTERS ***************************************************************/
        public void SetRotationSpeed(float newRotationSpeed)
        {
            rotationSpeed = newRotationSpeed;
        }

        /*************************************************************************** FUNCTIONS ***************************************************************/

        public void Rotate(Vector3 direction)
        {
            // Convert the input direction from local to world space, using the orientation transform as a reference for "forward".
            // This ensures the rotation is aligned with the orientation of the character or camera.
            Vector3 orientedDirection = orientation.TransformDirection(direction);
            
            // Find the angle θ where tan(θ) = orientedDirection.x / orientedDirection.z. Using Atan2 handles the sign of both orientedDirection.x and orientedDirection.z to place θ in the correct quadrant.
            // The calculation returns radians, so we have to convert to degrees.
            float rotationAmount = Mathf.Atan2(orientedDirection.x, orientedDirection.z) * Mathf.Rad2Deg;

            // Save the rotation amount in y axis of a qauternion to rotate our character. 
            Quaternion targetRotation = Quaternion.Euler(0, rotationAmount, 0);

            //Stop the previous lerp before starting a new one
            if (rotationLerp != null)
            {
                monoBehaviour.StopCoroutine(rotationLerp);
            }
            // Rotate the character
            rotationLerp = monoBehaviour.StartCoroutine(RotationLerp(targetRotation));
        }

        /*************************************************************************** COROUTINES ***************************************************************/


        // Smoothly rotate the character to the target rotation that we calculated, over a period of time.
        // As with all character movement, the rotation runs in fixed update.
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