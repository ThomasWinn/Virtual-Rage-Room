using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualLocomotion : MonoBehaviour
{
    public Transform moveDirection;
    public float maxVelocity = 3.0f;
    public float deadZone = 0.25f;
    public bool flyMode = false;
    public InputActionProperty moveAction;

    // Start is called before the first frame update
    void Start()
    {
        moveAction.action.performed += Move;
    }

    private void OnDestroy()
    {
        moveAction.action.performed -= Move;
    }

    private void Move(InputAction.CallbackContext obj)
    {
        Vector2 inputAxes = obj.action.ReadValue<Vector2>();
        if (inputAxes.y >= deadZone || inputAxes.y <= -deadZone)
        {
            Vector3 moveVector = moveDirection.forward;
            if (!flyMode)
            {
                moveVector.y = 0;
                moveVector.Normalize();
            }
            float velocity = maxVelocity * inputAxes.y * Time.deltaTime; // scaled speed based on how far the user flicked up
            this.transform.Translate(moveVector * velocity); // please explain once more
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
