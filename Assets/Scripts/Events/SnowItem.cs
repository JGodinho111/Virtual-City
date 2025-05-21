using UnityEngine;

/// <summary>
/// Handles snow storm destruction when too far away from city
/// Placed on trash items with tag Storm and Layer CityItem (to be checked for collisions and prevent them in new spawned game items)
/// </summary>
public class SnowItem : HazardItem
{
    public override void Update()
    {
        if (city != null)
        {
            // If snow away from city, destroy - 37 because it is spawned from above,
            // it is 30x30 and it starts counting from the origin of the gameobject (pivot - on the origin corner)
            if (Vector3.Distance(city.transform.position, this.transform.position) > 37f)
            {
                Debug.Log("Snow removed");
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.Log("City is null");
        }

    }
}
