using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Checks UI Raycasts based on mouse position
/// 
/// NOTE: For XR, edit the Input Action bindings for the actions for XR and regenerate class
/// </summary>
public class UIBlockDetector : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem; // Event System auto created by Unity when created a canvas

    [SerializeField]
    GraphicRaycaster canvasGraphicRaycaster; // Part of Unity's canvas

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

    public bool IsOnUI()
    {
        if (canvasGraphicRaycaster == null)
            return false;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = inputActions.Interactions.MousePosition.ReadValue<Vector2>(); //Mouse.current.position.ReadValue();

        List<RaycastResult> results = new ();
        canvasGraphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
}
