using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class CountdownTimer : MonoBehaviour
{
    public Text timerText; // Reference to the UI Text component
    private float timeRemaining = 60f; // Total time in seconds
    private bool timerIsRunning = false;

    void Start()
    {
        // Start the timer
        timerIsRunning = true;
        UpdateTimerText();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Decrease time
                UpdateTimerText(); // Update the UI text
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false; // Stop the timer
                // Optionally, you can trigger an event here when the timer ends
                Debug.Log("Timer has ended!");
            }
        }
    }

    void UpdateTimerText()
    {
        // Update the timer text to show the remaining time
        timerText.text = Mathf.Ceil(timeRemaining).ToString(); // Round up to the nearest whole number
    }
}