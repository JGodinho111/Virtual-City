using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that detects player input (left or right mouse button)
/// and is attached to the city gameobject (TODO: Maybe change to be separate)
/// If input is touching the gameobject it moves it either around the XZ plane (left click)
/// or tilts it around the Z axis (right click)
/// </summary>
public class CityMover : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask cityMask;

    [SerializeField]
    private LayerMask cityItem;

    private Vector2 lastMousePosition;

    private enum MovementMode {None, MoveXZ, RotateZ}
    private MovementMode currentMode = MovementMode.None;

    private Vector3 xzMovementPointOffset; // this is used when moving the city on the XZ plane so it doesn't just snap to the pivot of the gameobject when moving

    void Update()
    {
        if (currentMode == MovementMode.None && Mouse.current.leftButton.wasPressedThisFrame)
        {
            currentMode = MovementMode.MoveXZ;
            Debug.Log("Checking to move city on XZ plane");
            CheckCityCollisionToMove(); // check if hit the city
        }

        if (currentMode == MovementMode.None && Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentMode = MovementMode.RotateZ;
            Debug.Log("Checking to tilt city on Z axis");
            CheckCityCollisionToMove(); // check if hit the city
        }

        if (currentMode != MovementMode.None && (Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame))
        {
            currentMode = MovementMode.None;
            //StopMovingCity();
            Debug.Log("Not checking City Movement");
        }
    }

    // Checks to see if it hit the city, if so, then goes into a coroutine to move it around the XZ plane or tilt around the Z axis
    // TODO - Fix raycast working when hitting UI
    private void CheckCityCollisionToMove()
    {
        LayerMask allCity = cityMask | cityItem; // | uiLayer;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, allCity))
        {
            if (hit.collider.CompareTag("City") && (hit.transform == this.transform || hit.transform.gameObject.GetComponentInParent<Transform>().transform == hit.transform))
            {
                lastMousePosition = Mouse.current.position.ReadValue();
                if(currentMode == MovementMode.RotateZ)
                {
                    StartCoroutine(RotateZAxis());
                }
                else if(currentMode == MovementMode.MoveXZ)
                {
                    // Update grabbing point to be the exact coordinates of the place it was grabbed and not the gameobject pivot
                    xzMovementPointOffset = transform.InverseTransformPoint(hit.point);
                    StartCoroutine(MoveXZPlane());
                }
                else
                {
                    Debug.LogWarning("Mode not set to any movement");
                    return;
                }
            }
            else
            {
                Debug.Log("Raycast collided outside all city objects");
                // Ignore
                return;
            }
        }
    }

    // Moves gameobject around Z axis with its pivot as the axis origin
    private IEnumerator RotateZAxis()
    {
        while (Mouse.current.rightButton.isPressed && currentMode == MovementMode.RotateZ)
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            float delta = currentMousePosition.x - lastMousePosition.x;

            this.transform.Rotate(Vector3.forward, -delta * 0.2f, Space.World);
            lastMousePosition = currentMousePosition;

            yield return null;
        }
    }

    // Moves gameobject around XZ plane with its point being the mouse click point and not the pivot point
    private IEnumerator MoveXZPlane()
    {
        Plane movePlane = new Plane(Vector3.up, transform.position); // Vector3.zero

        while (Mouse.current.leftButton.isPressed && currentMode == MovementMode.MoveXZ)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(movePlane.Raycast(ray, out float enter))
            {
                Vector3 grabHitPoint = ray.GetPoint(enter);
                // TODO - FIX (offset is pulling it down a bit)
                transform.position = grabHitPoint - xzMovementPointOffset;
            }
            yield return null;
        }

    }
}
