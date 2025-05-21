using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Checks UI Raycasts based on mouse position
/// NOTE: For XR, change Mouse.current with InputActionReference of current controller position in 2D (need to double check)
/// </summary>
public class UIBlockDetector : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem; // Event System auto created by Unity when created a canvas

    [SerializeField]
    GraphicRaycaster canvasGraphicRaycaster; // Part of Unity's canvas

    public bool IsOnUI()
    {
        if (canvasGraphicRaycaster == null)
            return false;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new ();
        canvasGraphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
}
