using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that handles player camera controls (WASD Movement in 3D space and Zoom In & Out with mouse wheel
/// Attached to main camera of a scene
/// Note: Input Actions created and handled only for keyboard/mouse controller scheme
/// </summary>
public class CameraControlls : MonoBehaviour
{
    // --- Speeds of camera movement and zoom

    private float cameraMoveSpeed = 20f;
    private float cameraZoomSpeed = 20f;

    // --- The reference fields to the camera input actions with the new Unity Input System ---

    [SerializeField]
    private InputActionReference cameraMoveAction;

    [SerializeField]
    private InputActionReference cameraZoomAction;

    // --- Enabling and disabling actions, not subscribing 

    private void OnEnable()
    {
        cameraMoveAction.action.Enable();
        cameraZoomAction.action.Enable();
        // Subscribing to method to perform zoom changes, rather than doing it within Update()
        cameraZoomAction.action.performed += OnCameraZoom;

    }

    private void OnDisable()
    {
        // Unsusbscribing to method to perform zoom changes if input actions are disabled
        cameraZoomAction.action.performed += OnCameraZoom;
        cameraMoveAction.action.Disable();
        cameraZoomAction.action.Disable();
    }

    // Only tracked when zooming in & out - less frequent than movement
    private void OnCameraZoom(InputAction.CallbackContext obj)
    {
        float newZoom = cameraZoomAction.action.ReadValue<float>();
        this.transform.position += transform.forward * newZoom * cameraZoomSpeed * Time.deltaTime;
    }

    // Movement changes handled here since if handled similarly to OnCameraZoom it would be stuttered (only moving one unit per input) and since movement is likely more frequent than zoom
    private void Update()
    {
        Vector2 newMovePosition = cameraMoveAction.action.ReadValue<Vector2>();
        this.transform.Translate(new Vector3(newMovePosition.x, 0, newMovePosition.y) * cameraMoveSpeed * Time.deltaTime, Space.World);
    }
}
