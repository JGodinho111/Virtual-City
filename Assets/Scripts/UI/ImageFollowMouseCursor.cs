using UnityEngine;

/// <summary>
/// Makes instantiated image follow cursor
/// 
/// NOTE: For XR, edit the Input Action bindings for the actions for XR and regenerate class
/// </summary>
public class ImageFollowMouseCursor : MonoBehaviour
{
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

    void Update()
    {
        Vector2 mousePosition = inputActions.Interactions.MousePosition.ReadValue<Vector2>(); // mouse screen position - instead of getting directly from Mouse.current.position.ReadValue()
        this.transform.position = mousePosition;
    }
}
