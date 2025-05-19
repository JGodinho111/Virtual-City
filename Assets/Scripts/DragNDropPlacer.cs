using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///  Given a UI button press, place a desired game object into the city
///  TODO:
///  - Verifications to not allow placement on top of other city children
///  - indexes for images to decide what gameobject to spawn, currently it is just one set type
/// </summary>
public class DragNDropPlacer : MonoBehaviour
{

    [SerializeField]
    private InputActionReference objectPlacementAction; // Left Mouse Button

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask cityMask;

    [SerializeField]
    private GameObject testPrismImage; // TODO: Change for list of preview images (list and not HashSet because I'm using indexes)
    [SerializeField]
    private GameObject testPrismGameObject; // TODO: Change for list of gameobjects (list and not HashSet because I'm using indexes)

    private bool buttonToPlacePressed = false; // flag to only check left mouse click when

    private GameObject existingSceneImage; // to delete the preview image

    [SerializeField]
    private Canvas canvas; // to attach as the parent transform of the image preview

    private void OnEnable()
    {
        objectPlacementAction.action.Enable();
    }

    private void OnDisable()
    {
        objectPlacementAction.action.Disable();
    }

    void Update()
    {
        if(buttonToPlacePressed && objectPlacementAction.action.WasPressedThisFrame())
        {
            EndGameObjectPlacement(); // hide image and future calls to place object
            PlaceObject(); // attempt to place the wanted gameobject
        }
    }

    // Called directly from UI Image Button
    public void StartGameObjectPlacement()
    {
        buttonToPlacePressed = true;
        existingSceneImage = Instantiate(testPrismImage, canvas.transform);
    }

    private void EndGameObjectPlacement()
    {
        buttonToPlacePressed = false;
        Destroy(existingSceneImage);
    }

    private void PlaceObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, cityMask))
        {
            // TODO: Logic to make sure it doesn't collide with other gameobjects such as roads & other buildings
            if (hit.collider.CompareTag("City")) // technically not needed since I'm already looking at the layerMask City
            {
                GameObject placedObject = Instantiate(testPrismGameObject, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);

                placedObject.transform.SetParent(hit.transform);
            }
        }
    }

    
}
