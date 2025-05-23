using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles logic for spawning events (these include physics-based interactions, although none are handled here)
/// </summary>
public class ElementalEffectsPlacer : MonoBehaviour
{
    private enum Event {None, Trash, Storm}
    private Event currentEvent = Event.None;

    // ---------------------------------------------------
    // Could have created scriptable objects with the prefabs, effects and ongoingEvent name,
    // but left it like this since I had only planned one or two effects so no need

    [SerializeField]
    private GameObject trashPrefab;

    [SerializeField]
    private GameObject snowPrefab;

    [SerializeField]
    private GameObject trashEffectPrefab;

    [SerializeField]
    private GameObject snowEffectPrefab;

    // ---------------------------------------------------

    private GameObject eventsPooler;

    private SoundManager soundManager;

    void Start()
    {
        soundManager = SoundManager.Instance;

        eventsPooler = new GameObject();
        eventsPooler.name = "EventPooler";
    }

    // Now only updates event end
    // Used to Update timer and if an event is ongoing, it updates when it ends
    void Update()
    {
        if(currentEvent != Event.None)
        {
            if(currentEvent == Event.Trash)
            {
                // Check if any trash still exists
                var trash = GameObject.FindWithTag("Trash");
                if(trash == null)
                {
                    Debug.Log("All trash disposed of.");
                    currentEvent = Event.None;
                }

            }
            if (currentEvent == Event.Storm)
            {
                // Check if any Flood still exists
                var storm = GameObject.FindWithTag("Storm");
                if (storm == null)
                {
                    Debug.Log("Storm Cleaned Up.");
                    currentEvent = Event.None;
                }
            }
        }
    }

    // Now called directly by a button press to activate a random hazard
    public void ActivateHazard()
    {
        Debug.Log("Random Hazard Activated");
        if(Random.Range(0, 2) == 0)
        {
            currentEvent = Event.Trash;
            StartGarbadgeEvent();
        }
        else
        {
            currentEvent = Event.Storm;
            StartStormEvent();
        }
    }

    // Spawns one trash per city item in the scene
    private void StartGarbadgeEvent()
    {
        // Get all scene objects with tag city with layer CityItem
        var allCityObjects = FindGameObjectsInLayerWithTag("CityItem", "City"); //GameObject.FindGameObjectsWithTag("City");

        if (allCityObjects != null)
        {
            // Trash sound effect - assume it's alien objects instead of trash
            soundManager.CheckPlaySound("Trash");

            foreach (var cityPart in allCityObjects)
            {
                if (trashPrefab != null)
                {
                    // Particle Effect
                    if(trashEffectPrefab != null)
                        Instantiate(trashEffectPrefab, cityPart.GetComponentInParent<Transform>().transform.position + new Vector3(3f, 5f, 3f), Quaternion.identity);

                    // Above them Spawn Trash
                    GameObject instantiatedTrash = Instantiate(trashPrefab, cityPart.GetComponentInParent<Transform>().transform.position + new Vector3(3f, 5f, 3f), Quaternion.identity);

                    if (eventsPooler != null)
                        instantiatedTrash.transform.SetParent(eventsPooler.GetComponentInParent<Transform>().transform);
                }
            }
        }
        Debug.Log("Garbadge Hazard Started");
    }

    // Spawns snow drops
    // Currently only set up for one city, if multiple, then use a for each instead of accessing initial index
    private void StartStormEvent()
    {
        var city = FindGameObjectsInLayerWithTag("City", "City");

        if(city != null)
        {
            // "Snow Thunder" sound effect
            soundManager.CheckPlaySound("SnowThunder");

            GameObject actualCity = null;

            foreach (var cityObject in city)
            {
                if (cityObject.GetComponent<Rigidbody>() != null)
                {
                    actualCity = cityObject;
                }
            }

            if(actualCity != null)
            {
                Vector3 cityPosition = actualCity.GetComponent<Rigidbody>().position;
                Vector3 citySize = actualCity.GetComponent<Renderer>().bounds.size;

                // Particle Effect
                if (snowEffectPrefab != null)
                    Instantiate(snowEffectPrefab, cityPosition + citySize / 2, Quaternion.identity);

                for (int i = 0; i < 200; i++)
                {
                    // Since the pivot is at 0, 0, 0 the random is between those coordinates and those coordinates plus the size (which is currently 30)
                    // - if pivot were in the center, I'd get half of the negative position coordinates and half of the positive ones
                    GameObject instantiatedSnow = Instantiate(snowPrefab, new Vector3(Random.Range(cityPosition.x, cityPosition.x + citySize.x), 5f, Random.Range(cityPosition.z, cityPosition.z + citySize.z)), Quaternion.identity);

                    if (eventsPooler != null)
                        instantiatedSnow.transform.SetParent(eventsPooler.GetComponentInParent<Transform>().transform);
                }
            }
        }

        Debug.Log("Snow Storm Hazard Started");
    }

    // Method called to get all objects with a specific tag pertaining to a specific layer
    private List<GameObject> FindGameObjectsInLayerWithTag(string layerMaskName, string tagName)
    {
        var allObjectsWithTag = GameObject.FindGameObjectsWithTag(tagName);

        var neededObjects = new List<GameObject>();

        foreach (var objectWithTag in allObjectsWithTag)
        {
            if (objectWithTag.layer == LayerMask.NameToLayer(layerMaskName))
            {
                neededObjects.Add(objectWithTag);
            }
        }
        return neededObjects;
    }

    // If other events were to be added,
    // add a verification add them to the enum,
    // add them to ActivateHazard() add verification on Update(),
    // create the prefab with layer CityLayer and a new tag,
    // and create the method for their spawn

}
