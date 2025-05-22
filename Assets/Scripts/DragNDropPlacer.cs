using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
///  Given a UI button press, place a desired game object into the city
///  NOTE: If it were an XR Scene, simply replace the two Mouse.current references
///  with the desired ActionInputReference (in this case pertaining to the mouse left button)
///  and add to its action properties the desired MetaQuest button binding
/// </summary>
public class DragNDropPlacer : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask cityMask;

    [SerializeField]
    private LayerMask cityItem;

    [SerializeField]
    private LayerMask cityTree;

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
                    // NOTE: This was a problem with the way I setup the probuilder mesh, it now works perfectly (and doesn't use them - only in the city base)
                    //---- IGNORE ----
                    // Adding the normal so regardless of city tilt, it is always correct
                    //Vector3 position = hit.point + hit.normal * 1.25f; //1.25f checked directly in director
                    // Essentially what is happening without the 1.25 is it spawning within the city (1/4 of the way inside)
                    //---- IGNORE ----

                    // To make it so the rotation is randomized
                    int[] angles = { 0, 90, 180, 270};

                    // Since the external asset prefabs are facing the opposite way of what I want, I need to modify them rather than just using hit.transform.rotation
                    Quaternion rotationOfNewPrefabs = hit.transform.rotation * Quaternion.Euler(0f, angles[Random.Range(0, angles.Length)], 0f);

                    // I had a random offset to offset the position, but I decided against it because it didn+t feel right
                    GameObject placedObject = Instantiate(currentGameObjectPrefab, hit.point, rotationOfNewPrefabs);
                    placedObject.transform.SetParent(hit.transform);

                    // Logic to make sure it doesn't collide with other gameobjects such as roads & other buildings
                    Collider[] placedObjectColliderHits = Physics.OverlapBox(placedObject.transform.position, placedObject.GetComponent<Collider>().bounds.size / 2, placedObject.transform.rotation, cityItem);

                    foreach(var colliderHit in placedObjectColliderHits)
                    {
                        // Needs to check if didn't hit self 
                        if (colliderHit.gameObject != placedObject)
                        {
                            Debug.Log("Gameobject hit another cityItem, and so is removed!");
                            // Fail to deploy sound
                            soundManager.CheckPlaySound("SpawnFailure");
                            Destroy(placedObject);
                            return;
                        }
                        
                    }

                    // If it can, then proceed also check trees
                    Collider[] treesColliderHits = Physics.OverlapBox(placedObject.transform.position, placedObject.GetComponent<Collider>().bounds.size / 2, placedObject.transform.rotation, cityTree);

                    foreach (var colliderHit in treesColliderHits)
                    {
                        // Needs to check if didn't hit self 
                        if (colliderHit.gameObject != placedObject)
                        {
                            Debug.Log("Gameobject hit tree, and so tree is removed!");
                            // Delete Tree
                            Destroy(colliderHit.gameObject);
                        }
                    }

                    // Successful deploy sound
                    soundManager.CheckPlaySound("SpawnSuccess");

                    Debug.Log("Gameobject hit another, but it is the city so it remains in the scene.");
                }
                else
                {
                    // Fail to deploy sound
                    soundManager.CheckPlaySound("SpawnFailure");
                    Debug.LogError("No currentGameObjectPrefab exists to instantiate!");
                }
            }
        }
        else
        {
            // Fail to deploy sound
            soundManager.CheckPlaySound("SpawnFailure");
            Debug.Log("Raycast collided outside the buildable city");
        }
    }
}
