using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    private int killCount;

    private void Start()
    {
        // Initialize the kill count to 0
        killCount = 0;
    }

    public void IncrementKillCount()
    {
        // Increment the kill count
        killCount++;

        // Update the counter text
        counterText.text = "Kills: " + killCount.ToString();
    }
}

