using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        GenerateMap(); // Generate a new map when the game starts
    }

    private void Update()
    {
        // Check for 'Esc' key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the MainMenu scene
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
        GenerateMap(); // Generate a new map before loading the game scene
    }

    public void GoToSettingsMenu()
    {
        SceneManager.LoadScene("INFO");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void GenerateMap()
    {
        Debug.Log("Generating a new map...");
        CorridorFirstMapGenerator mapGenerator = FindObjectOfType<CorridorFirstMapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.GenerateMap();
            Debug.Log("New map generated successfully.");
        }
        else
        {
            Debug.LogWarning("CorridorFirstMapGenerator not found in the scene.");
        }
    }
}

