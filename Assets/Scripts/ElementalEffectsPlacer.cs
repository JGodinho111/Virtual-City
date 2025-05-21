using UnityEngine;

/// <summary>
/// Class that handles logic for spawning events (these include physics-based interactions, although none are handled here)
/// </summary>
public class ElementalEffectsPlacer : MonoBehaviour
{
    private float timer;
    private float timerTime = 2f;

    private enum ongoingEvent {None, Trash, Flood}
    private ongoingEvent currentEvent = ongoingEvent.None;

    [SerializeField]
    private GameObject trashPrefab;

    void Start()
    {
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
            if (currentEvent == ongoingEvent.Flood)
            {
                // Check if any Flood still exists
                var flood = GameObject.FindWithTag("Flood");
                if (flood == null)
                {
                    Debug.Log("Flood Drained.");
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
            currentEvent = ongoingEvent.Flood;
            StartFloodEvent();
        }
    }

    // Spawns one trash per city item in the scene
    private void StartGarbadgeEvent()
    {
        // Get all scene objects with tag city with layer CityItem
        var allCityObjects = GameObject.FindGameObjectsWithTag("City");

        foreach(var cityPart in allCityObjects)
        {
            if(cityPart.layer == LayerMask.NameToLayer("CityItem"))
            {
                if(trashPrefab != null)
                {
                    // Above them Spawn Trash
                    GameObject instantiatedTrash = Instantiate(trashPrefab, cityPart.GetComponentInParent<Transform>().transform.position + new Vector3(3f, 5f, 3f), Quaternion.identity);
                    // So that they interact with the city
                    instantiatedTrash.transform.SetParent(cityPart.GetComponentInParent<Transform>().transform); 
                }
            }
        }
        Debug.Log("Garbadge Hazard Started");
    }

    // Creates a water flood
    private void StartFloodEvent()
    {
        Debug.Log("Flood Hazard Started");
    }

    
}
