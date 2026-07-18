using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTester : MonoBehaviour
{
    public void HandleMovement(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();

        transform.position += new Vector3(v.x, 0, v.y) * Time.deltaTime;
    }
}