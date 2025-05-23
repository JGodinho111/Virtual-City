using System;
using UnityEngine;

/// <summary>
/// Just for fun
/// A simple game script
/// TODO: Add UI and UI screens
/// </summary>
public class GameManager : MonoBehaviour
{
    private float gameTimer = 100f;
    private float hazardTimer = 6f;
    private int totalBuildingsBuilt;
    private int startBuildings;

    private bool timerEnded = false;

    private ElementalEffectsPlacer elementalEffectsPlacer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startBuildings = GameObject.FindGameObjectsWithTag("City").Length;
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

            totalBuildingsBuilt = GameObject.FindGameObjectsWithTag("City").Length - startBuildings;

            if (totalBuildingsBuilt < 0)
                totalBuildingsBuilt = 0;

            Debug.Log("Total buildings built: " + totalBuildingsBuilt);
        }
    }

    private void HandleGameEnd()
    {
        throw new NotImplementedException();
    }
}
