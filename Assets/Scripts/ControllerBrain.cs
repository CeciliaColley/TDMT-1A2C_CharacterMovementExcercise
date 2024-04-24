using Input;
using Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBrain : MonoBehaviour
{
    [SerializeField] private CharacterBody body;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float acceleration = 4.0f;

    private Vector3 desiredDirection;

    private void OnEnable()
    {
        if (body == null)
        {
            Debug.LogError($"{name}: {nameof(body)} is null, so the object is being disabled.");
            enabled = false;
            return;
        }
        
        inputReader.onMovementInput += HandleMovementInput;
    }
    private void OnDisable()
    {
        inputReader.onMovementInput -= HandleMovementInput;
    }

    private void HandleMovementInput(Vector2 input)
    {
        desiredDirection = new Vector3(input.x, 0, input.y);
    }

    private void Update()
    {
        transform.position += desiredDirection * (Time.deltaTime * speed);
        body.SetMovement(new MovementRequest(desiredDirection, speed, acceleration));
    }
}
