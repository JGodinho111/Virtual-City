using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
///  Given a UI button press, place a desired game object into the city
///  NOTE: If it were an XR Scene, simply replace the two Mouse.current references
///  with the desired ActionInputReference (in this case pertaining to the mouse left button)
///  and add to its action properties the desired MetaQuest button binding
///  TODO:
///  - Verifications to not allow placement on top of other city children (not fully working)
/// </summary>
public class DragNDropPlacer : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask cityMask;

    [SerializeField]
    private LayerMask cityItem;

    private GameObject currentGameobjectImage;
    private GameObject currentGameObjectPrefab;

    private bool buttonToPlacePressed = false; // flag to only check left mouse click when

    private GameObject existingSceneImage; // to delete the preview image

    [SerializeField]
    private Canvas canvas; // to attach as the parent transform of the image preview

    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
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
            StartCoroutine(FadeAwayImageUI());
        }
        else
        {
            Debug.LogError("No currentGameobjectImage exists to destroy!");
        }
    }

    // Fades away the UI icon and then destroys it
    private IEnumerator FadeAwayImageUI()
    {
        Image imageToFade = existingSceneImage.GetComponent<Image>();
        if(imageToFade != null)
        {
            Color color = imageToFade.color;
            float durationToFade = 0.2f;
            float timeElapsed = 0f;

            while (timeElapsed < durationToFade)
            {
                float alphaValue = Mathf.Lerp(1f, 0f, timeElapsed / durationToFade);
                imageToFade.color = new Color(color.r, color.g, color.b, alphaValue);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            imageToFade.color = new Color(color.r, color.g, color.b, 0f);
            Destroy(existingSceneImage);
        }
    }

    private void PlaceObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, cityMask))
        {
            if (hit.collider.CompareTag("City")) // technically not needed since I'm already looking at the layerMask City
            {

                if (currentGameObjectPrefab != null)
                {
                    // Randomizing slightly the coordinates so it isn't exactly where it is placed as requested (XZ Plane)
                    Vector3 randomOffset = new Vector3(Random.Range(-0.01f, 0.01f), 0f, Random.Range(-0.01f, 0.01f));

                    // Change the position so it makes sure not to overlap with the city itself
                    GameObject placedObject = Instantiate(currentGameObjectPrefab, new Vector3(hit.point.x + randomOffset.x, hit.point.y + 0.5f, hit.point.z + randomOffset.z), hit.transform.rotation);
                    placedObject.transform.SetParent(hit.transform);

                    // TODO: Change to use simpler box colliders, will be more accurate overall
                    // Logic to make sure it doesn't collide with other gameobjects such as roads & other buildings
                    Collider[] placedObjectColliderHits = Physics.OverlapBox(placedObject.transform.position, placedObject.GetComponent<Collider>().bounds.size / 2, placedObject.transform.rotation, cityItem);

                    foreach(var colliderHit in placedObjectColliderHits)
                    {
                        if (colliderHit.gameObject != placedObject && colliderHit.gameObject.layer != cityMask)
                        {
                            Debug.LogWarning("Gameobject hit another cityItem, and so is removed!");
                            // TODO - Fail to deploy sound
                            soundManager.CheckPlaySound("SpawnFailure");
                            Destroy(placedObject);
                            return;
                        }
                    }

                    Debug.Log("Gameobject hit another, but it is the city so it remains in the scene.");
                    // TODO - Successful deploy sound
                    soundManager.CheckPlaySound("SpawnSuccess");
                }
                else
                {
                    Debug.LogError("No currentGameObjectPrefab exists to instantiate!");
                    // TODO - Fail to deploy sound
                    soundManager.CheckPlaySound("SpawnFailure");
                }
            }
            else
            {
                Debug.LogWarning("Raycast collided outside the buildable city");
                // TODO - Fail to deploy sound
                soundManager.CheckPlaySound("SpawnFailure");
            }
        }
    }
}
