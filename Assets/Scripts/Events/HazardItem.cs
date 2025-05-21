using UnityEngine;

/// <summary>
/// Interface that Handles hazard behaviour and destruction when too far away from city
/// Methods implemented in the classes that extend the class
/// </summary>
public abstract class HazardItem : MonoBehaviour
{
    protected GameObject city;

    protected void Start()
    {
        var allCityObjects = GameObject.FindGameObjectsWithTag("City");

        foreach (var cityPart in allCityObjects)
        {
            if (cityPart.layer == LayerMask.NameToLayer("City"))
            {
                city = cityPart;
                return;
            }
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {}
}
