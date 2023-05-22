using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private void Update()
    {
        // Check for 'Esc' key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Check if the game is currently running
            if (SceneManager.GetActiveScene().name == "Game")
            {
                // Load the MainMenu scene
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}

