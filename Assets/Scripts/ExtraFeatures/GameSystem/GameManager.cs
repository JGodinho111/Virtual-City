using System;
using UnityEngine;
using TMPro;

/// <summary>
/// Just for fun - Extra Feature
/// 
/// A simple game manager script that checks buildings built over a 100 seconds and then displays and end screen
/// And creates hazard events every 6 seconds for the player to have to deal with it
/// Also updates UI
/// </summary>
public class GameManager : MonoBehaviour
{
    private float gameTimer = 30f;
    private float hazardTimer = 6f;
    private GameObject[] totalBuildingsList;
    private int totalBuildings;
    private int startBuildings;
    private int totalBuildingsBuilt = 0;

    private bool timerEnded = false;

    private ElementalEffectsPlacer elementalEffectsPlacer;

    [SerializeField]
    private TMP_Text textUI;

    [SerializeField]
    private Transform endScreen;

    [SerializeField]
    private TMP_Text endText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endScreen.gameObject.SetActive(false);
        GameObject[] startBuildingsList = GameObject.FindGameObjectsWithTag("City");
        startBuildings = GetCityItemCount(startBuildingsList);

        elementalEffectsPlacer = GameObject.FindFirstObjectByType<ElementalEffectsPlacer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!timerEnded)
        {
            gameTimer -= Time.deltaTime;
            hazardTimer -= Time.deltaTime;

            Debug.Log("Game Timer: " + gameTimer);
            Debug.Log("Hazard Timer: " + hazardTimer);

            if (gameTimer <= 0)
            {
                timerEnded = true;
                Debug.Log("Game Over");
                HandleGameEnd();
            }
            
            if (hazardTimer <= 0)
            {
                hazardTimer = 6f;
                if (elementalEffectsPlacer != null)
                    elementalEffectsPlacer.ActivateHazard();
            }

            totalBuildingsList = GameObject.FindGameObjectsWithTag("City");

            totalBuildings = GetCityItemCount(totalBuildingsList);            

            if (totalBuildings > startBuildings)
            {
                totalBuildingsBuilt = totalBuildings - startBuildings;
            }

            Debug.Log("Start buildings: " + startBuildings);
            Debug.Log("Total buildings: " + totalBuildings);
            Debug.Log("Total buildings built: " + totalBuildingsBuilt);

            if (textUI != null)
                textUI.SetText("Timer: " + (int) gameTimer + "\nBuildings Placed: " + totalBuildingsBuilt);
        }
    }

    private int GetCityItemCount(GameObject[] totalBuildingsList)
    {
        int totalBuildings = 0;
        foreach (var building in totalBuildingsList)
        {
            if (building.gameObject.layer == LayerMask.NameToLayer("CityItem"))
            {
                totalBuildings += 1;
            }
        }
        return totalBuildings;
    }

    private void HandleGameEnd()
    {
        endScreen.gameObject.SetActive(true);
        if (endText != null)
            endText.SetText("You built " + totalBuildingsBuilt + " buildings in 30 seconds!");
    }
}
