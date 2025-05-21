using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles logic for spawning events (these include physics-based interactions, although none are handled here)
/// </summary>
public class ElementalEffectsPlacer : MonoBehaviour
{
    private float timer;
    private float timerTime = 2f;

    private enum ongoingEvent {None, Trash, Storm}
    private ongoingEvent currentEvent = ongoingEvent.None;

    [SerializeField]
    private GameObject trashPrefab;

    [SerializeField]
    private GameObject snowPrefab;

    [SerializeField]
    private GameObject trashEffectPrefab;

    [SerializeField]
    private GameObject snowEffectPrefab;

    private SoundManager soundManager;

    void Start()
    {
        soundManager = SoundManager.Instance;

        timer = timerTime;
    }

    // Now only updates event end
    // Used to Update timer and if an event is ongoing, it updates when it ends
    void Update()
    {
        /*
        if(timer > 0 && currentEvent == ongoingEvent.None)
        {
            timer -= Time.deltaTime;
        }
        if(timer <= 0 && currentEvent == ongoingEvent.None)
        {
            timer = timerTime;
            ActivateHazard();
        }
        */
        if(currentEvent != ongoingEvent.None)
        {
            if(currentEvent == ongoingEvent.Trash)
            {
                // Check if any trash still exists
                var trash = GameObject.FindWithTag("Trash");
                if(trash == null)
                {
                    Debug.Log("All trash disposed of.");
                    currentEvent = ongoingEvent.None;
                }

            }
            if (currentEvent == ongoingEvent.Storm)
            {
                // Check if any Flood still exists
                var storm = GameObject.FindWithTag("Storm");
                if (storm == null)
                {
                    Debug.Log("Storm Cleaned Up.");
                    currentEvent = ongoingEvent.None;
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
            currentEvent = ongoingEvent.Trash;
            // TODO: Ui event badge stating trash event started
            StartGarbadgeEvent();
        }
        else
        {
            currentEvent = ongoingEvent.Storm;
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
            // TODO "Wuhhhooop" sound effect - assume it's alien objects instead of trash
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
                    // So that they interact with the city
                    instantiatedTrash.transform.SetParent(cityPart.GetComponentInParent<Transform>().transform);
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
            // TODO "Snow Thunder" sound effect
            soundManager.CheckPlaySound("SnowThunder");

            Vector3 cityPosition = city[0].transform.position;
            Vector3 citySize = city[0].GetComponent<Renderer>().bounds.size;

            // Particle Effect
            if (snowEffectPrefab != null)
                Instantiate(snowEffectPrefab, (cityPosition + citySize) / 2, Quaternion.identity);

            for (int i = 0; i < 200; i++)
            {
                // Since the pivot is at 0, 0, 0 the random is between that and the size (which is currently 30)
                // - if pivot were in the center, I'd get half of the negative position coordinates and half of the positive ones
                Instantiate(snowPrefab,new Vector3(Random.Range(cityPosition.x, citySize.x), 5f, Random.Range(cityPosition.z, citySize.z)), Quaternion.identity);
                // City is currently between 0 and 30, so I could just do Random.Range(0, 30), but I'm calling the city specifically so in case I move it everything is where it should be
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
