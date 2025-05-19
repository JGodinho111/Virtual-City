using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///  Given a UI button press, place a desired game object into the city
///  TODO:
///  - Verifications to not allow placement on top of other city children
///  - indexes for images to decide what gameobject to spawn, currently it is just one set type
/// -  could be a singleton (if so change the name to manager)
/// </summary>
public class DragNDropPlacer : MonoBehaviour
{

    [SerializeField]
    private InputActionReference objectPlacementAction; // Left Mouse Button

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask cityMask;

    private GameObject currentGameobjectImage;
    private GameObject currentGameObjectPrefab;

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
        if(buttonToPlacePressed && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndGameObjectPlacement(); // hide image and future calls to place object
            PlaceObject(); // attempt to place the wanted gameobject
        }
    }

    // Called directly from UI Image Button
    public void StartGameObjectPlacement(GameObject prefabToPlace, GameObject imageToShow)
    {
        currentGameObjectPrefab = prefabToPlace;
        currentGameobjectImage = imageToShow;

        buttonToPlacePressed = true;
        if(currentGameobjectImage != null)
        {
            existingSceneImage = Instantiate(currentGameobjectImage, canvas.transform);
        }
        else
        {
            Debug.LogError("No currentGameobjectImage exists to instantiate!");
        }
    }

    private void EndGameObjectPlacement()
    {
        buttonToPlacePressed = false;
        if (currentGameobjectImage != null)
        {
            Destroy(existingSceneImage);
        }
        else
        {
            Debug.LogError("No currentGameobjectImage exists to destroy!");
        }
    }

    private void PlaceObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, cityMask))
        {
            if (hit.collider.CompareTag("City")) // technically not needed since I'm already looking at the layerMask City
            {
                // TODO: Logic to make sure it doesn't collide with other gameobjects such as roads & other buildings

                if (currentGameObjectPrefab != null)
                {
                    GameObject placedObject = Instantiate(currentGameObjectPrefab, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);

                    placedObject.transform.SetParent(hit.transform);
                }
                else
                {
                    Debug.LogError("No currentGameObjectPrefab exists to instantiate!");
                }
            }
        }
    }

    
}
