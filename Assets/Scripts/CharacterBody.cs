using Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class CharacterBody : MonoBehaviour
{
    private Rigidbody rigidBody;
    private MovementRequest currentMovement;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnValidate()
    {
        rigidBody ??= GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!currentMovement.IsValid() || rigidBody.velocity.magnitude >= currentMovement.GoalSpeed) 
        {
            return;
        }
        rigidBody.AddForce(currentMovement.GetAccelerationVector(), ForceMode.Force);
    }

    public void SetMovement(MovementRequest movementRequest)
    {
        currentMovement = movementRequest;
    }
}
