using System.Collections;
using UnityEngine;

/// <summary>
/// Class that detects player input (left or right mouse button)
/// and is attached to the city gameobject (this way we can have as many cities as we want)
/// If input is touching the gameobject it moves it either around the XZ plane (left click)
/// or tilts it around the Z axis (right click)
/// - Updated to now use physics-based movement
/// 
/// NOTE: For XR, edit the Input Action bindings for the actions for XR and regenerate class
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

    [SerializeField]
    private UIBlockDetector blockDetector; // serves to detect if pointer is over UI to then prevent raycasts

    private Vector2 lastMousePosition;

    private enum Movement {None, MoveXZ, RotateZ}
    private Movement currentMode = Movement.None;

    private Vector3 xzMovementPointOffset; // this is used when moving the city on the XZ plane so it doesn't just snap to the pivot of the gameobject when moving

    private Rigidbody cityRigidbody;

    // Instead of accessing input action references, using the class I auto generated from the existing ones
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        cityRigidbody = this.GetComponent<Rigidbody>();

        // Not needed since already set up in the inspector, but just in case I change it accidentally
        cityRigidbody.useGravity = false; // Not using it so the city doesn't just fall
        cityRigidbody.isKinematic = true; // So rigidbody is controlled via the script
    }

    void Update()
    {
        if (currentMode == Movement.None && inputActions.Interactions.LeftMouseClick.WasPressedThisFrame()) //Mouse.current.leftButton.wasPressedThisFrame)
        {
            currentMode = Movement.MoveXZ;
            Debug.Log("Checking to move city on XZ plane");
            CheckCityCollisionToMove(); // check if hit the city
        }

        if (currentMode == Movement.None && inputActions.Interactions.RightMouseClick.WasPressedThisFrame())//Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentMode = Movement.RotateZ;
            Debug.Log("Checking to tilt city on Z axis");
            CheckCityCollisionToMove(); // check if hit the city
        }

        if (currentMode != Movement.None && (inputActions.Interactions.LeftMouseClick.WasReleasedThisFrame() || inputActions.Interactions.RightMouseClick.WasReleasedThisFrame()))//(Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame))
        {
            currentMode = Movement.None;
            //StopMovingCity();
            Debug.Log("Not checking City Movement");
        }
    }

    // Checks to see if it is not hitting UI, the check if it hit the city, if so, then goes into a coroutine to move it around the XZ plane or tilt around the Z axis
    private void CheckCityCollisionToMove()
    {
        if(!blockDetector.IsOnUI())
        {
            LayerMask allCity = cityMask | cityItem;

            Ray ray = mainCamera.ScreenPointToRay(inputActions.Interactions.MousePosition.ReadValue<Vector2>()); // Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, allCity))
            {
                if (hit.collider.CompareTag("City") && (hit.transform == this.transform || hit.transform.gameObject.GetComponentInParent<Transform>().transform == hit.transform))
                {
                    lastMousePosition = inputActions.Interactions.MousePosition.ReadValue<Vector2>(); //Mouse.current.position.ReadValue();
                    if (currentMode == Movement.RotateZ)
                    {
                        StartCoroutine(RotateZAxis());
                    }
                    else if (currentMode == Movement.MoveXZ)
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
                    return;
                }
            }
        }
    }

    // Moves gameobject around Z axis with its pivot as the axis origin
    // Gets the angle between mouse positions on the Z axis
    // and moves the rigidbody by multiplying the current rotation with the new one
    private IEnumerator RotateZAxis()
    {
        while (inputActions.Interactions.RightMouseClick.IsPressed() && currentMode == Movement.RotateZ) //Mouse.current.rightButton.isPressed
        {
            Vector2 currentMousePosition = inputActions.Interactions.MousePosition.ReadValue<Vector2>();//Mouse.current.position.ReadValue();
            float delta = currentMousePosition.x - lastMousePosition.x;

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
        Plane movePlane = new Plane(Vector3.up, cityRigidbody.transform.position);

        while (inputActions.Interactions.LeftMouseClick.IsPressed() && currentMode == Movement.MoveXZ) //Mouse.current.leftButton.isPressed
        {
            Ray ray = mainCamera.ScreenPointToRay(inputActions.Interactions.MousePosition.ReadValue<Vector2>()); // Mouse.current.position.ReadValue());

            if(movePlane.Raycast(ray, out float enter))
            {
                Vector3 grabHitPoint = ray.GetPoint(enter);

                // xzMovementPointOffset still has a problem, if I'm grabbing it from the sides or the bottom, disregarding for now
                Vector3 newPosition = Vector3.Lerp(cityRigidbody.position, grabHitPoint - xzMovementPointOffset, 0.3f);
                cityRigidbody.MovePosition(newPosition);
            }
            yield return null;
        }
    }
}
