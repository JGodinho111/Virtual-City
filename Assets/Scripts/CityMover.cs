using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that detects player input (left or right mouse button)
/// and is attached to the city gameobject (this way we can have as many cities as we want)
/// If input is touching the gameobject it moves it either around the XZ plane (left click)
/// or tilts it around the Z axis (right click)
/// - Updated to now use physics-based movement
/// 
/// NOTE: If it were an XR Scene, simply replace the two Mouse.current references
/// with the desired ActionInputReference (in this case pertaining to the mouse left button and another to the right mouse button)
/// and add to its action properties the desired MetaQuest button binding
/// </summary>
[RequireComponent(typeof(Rigidbody))]
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

    private Rigidbody cityRigidbody;

    private void Start()
    {
        cityRigidbody = this.GetComponent<Rigidbody>();

        // Not needed since already set up in the inspector, just to explain it
        cityRigidbody.useGravity = false; // Not using it so the city doesn't just fall
        cityRigidbody.isKinematic = true; // So rigidbody is controlled via the script
    }

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
    // Gets the angle between mouse positions on the Z axis
    // and moves the rigidbody by multiplying the current rotation with the new one
    private IEnumerator RotateZAxis()
    {
        while (Mouse.current.rightButton.isPressed && currentMode == MovementMode.RotateZ)
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            float delta = currentMousePosition.x - lastMousePosition.x;

            //this.transform.Rotate(Vector3.forward, -delta * 0.2f, Space.World);
            Quaternion rotation = Quaternion.AngleAxis(-delta * 0.5f, Vector3.forward);
            cityRigidbody.MoveRotation(cityRigidbody.rotation * rotation);

            lastMousePosition = currentMousePosition;

            yield return null;
        }
    }

    // Moves gameobject around XZ plane with its point being the mouse click point and not the pivot point
    // Raycasts constanty to get the exact position to change to,
    // modifes it with the xzMovementPointOffset so it is applied directly on where the player pressed
    // and then sets the rigid body to move to a new position by interpolating it within 0.3 seconds (to appear to be gliding)
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
                //transform.position = grabHitPoint - xzMovementPointOffset;
                Vector3 newPosition = Vector3.Lerp(cityRigidbody.position, grabHitPoint - xzMovementPointOffset, 0.3f);
                cityRigidbody.MovePosition(newPosition);
            }
            yield return null;
        }
    }
}
