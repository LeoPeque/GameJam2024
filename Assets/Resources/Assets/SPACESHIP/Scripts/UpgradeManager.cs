using UnityEngine;
using UnityEngine.UI;

public class BarUpgradeManager : MonoBehaviour
{
    [System.Serializable]
    public class BarButtonPair
    {
        public Button button;       // Button to upgrade the bar
        public Transform barContainer; // Container for this bar's upgrades
        public GameObject barPrefab;   // Prefab for the bars
        public int costPerUpgrade = 1; // Cost to upgrade this bar
    }

    public BarButtonPair[] barButtonPairs; // Array to hold all button-bar pairs
    public Text pointsText;               // Text UI to display points
    public int startingPoints = 5;        // Points the player starts with

    public GameObject energyBarContainer; // Container that holds the energy bars
    public GameObject energyBarPrefab;    // Prefab for individual energy bars
    public int startingEnergy = 6;        // Starting energy value
    public int maxEnergy = 6;             // Starting max energy

    public GameObject o2BarContainer;     // Container that holds the O2
    public GameObject o2BarPrefab;       // Prefab for individual O2 bars
    public int startingO2 = 6;            // Starting O2 value
    public int maxO2 = 6;                 // Starting max O2

    private int currentPoints;            // Tracks current points
    private int currentEnergy;            // Tracks current energy
    private int currentO2;                // Tracks current O2
    private int[] barLevels;              // Tracks the level of each bar

    void Start()
    {
        // Initialize points, energy, and bar levels
        currentPoints = startingPoints;
        currentEnergy = startingEnergy;
        currentO2 = startingO2;
        barLevels = new int[barButtonPairs.Length];

        // Update the points display
        UpdatePointsDisplay();
        UpdateEnergyBars();
        UpdateO2Bars();

        // Assign button click listeners
        for (int i = 0; i < barButtonPairs.Length; i++)
        {
            int index = i; // Capture index to avoid closure issues
            barButtonPairs[i].button.onClick.AddListener(() => UpgradeBar(index));
        }
    }

    public void UpgradeBar(int barIndex)
    {
        // Validate the bar index
        if (barIndex < 0 || barIndex >= barButtonPairs.Length)
        {
            Debug.LogError("Invalid bar index: " + barIndex);
            return;
        }

        BarButtonPair pair = barButtonPairs[barIndex];

        // If upgrading the first bar (energy upgrade)
        if (barIndex == 0)
        {
            // Check if there are enough points to upgrade energy
            if (currentPoints >= pair.costPerUpgrade && maxEnergy < 10) // Max energy cap (you can adjust the value)
            {
                // Deduct points and increase max energy
                maxEnergy++;

                // Update the energy and points display
                UpdateEnergyBars();

                Debug.Log($"Energy max upgraded to {maxEnergy}. Remaining points: {currentPoints}");
            }
            else
            {
                Debug.LogWarning("Not enough points to upgrade energy or max energy reached!");
            }
        }

        // If upgrading the second bar (O2 upgrade)
        if (barIndex == 1)
        {
            // Check if there are enough points to upgrade O2
            if (currentPoints >= pair.costPerUpgrade && maxO2 < 10) // Max O2 cap (you can adjust the value)
            {
                // Deduct points and increase max O2
                maxO2++;

                // Update the O2 and points display
                UpdateO2Bars();

                Debug.Log($"O2 max upgraded to {maxO2}. Remaining points: {currentPoints}");
            }
            else
            {
                Debug.LogWarning("Not enough points to upgrade O2 or max O2 reached!");
            }
        }

        // For other upgrades, follow the existing logic
        // Check if the player has enough points to upgrade
        if (currentPoints >= pair.costPerUpgrade)
        {
            // Deduct points and upgrade the bar
            currentPoints -= pair.costPerUpgrade;
            GameObject newBar = Instantiate(pair.barPrefab);
            newBar.transform.SetParent(pair.barContainer, false); // Maintain local positioning
            newBar.name = "Bar " + (barIndex + 1) + " Level " + (barLevels[barIndex] + 1);

            // Increment the bar's level
            barLevels[barIndex]++;

            // Update the points display
            UpdatePointsDisplay();

            Debug.Log($"Upgraded Bar {barIndex + 1} to Level {barLevels[barIndex]}. Remaining points: {currentPoints}");
        }
        else
        {
            Debug.LogWarning("Not enough points to upgrade!");
        }

    }

    private void UpdatePointsDisplay()
    {
        // Update the text on the UI
        pointsText.text = currentPoints.ToString();
    }

    private void UpdateEnergyBars()
    {
        // Clear any existing energy bars
        foreach (Transform child in energyBarContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Create energy bars based on currentEnergy
        for (int i = 0; i < currentEnergy; i++)
        {
            GameObject bar = Instantiate(energyBarPrefab);
            bar.transform.SetParent(energyBarContainer.transform, false);
            bar.name = "Energy Bar " + (i + 1);
        }
    }

    private void UpdateO2Bars()
    {
        // Clear any existing energy bars
        foreach (Transform child in o2BarContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Create energy bars based on currentEnergy
        for (int i = 0; i < currentO2; i++)
        {
            GameObject bar = Instantiate(o2BarPrefab);
            bar.transform.SetParent(o2BarContainer.transform, false);
            bar.name = "O2 Bar " + (i + 1);
        }
    }
    

    // Optional: Allow modifying points from Unity at runtime for testing
    public void AddPoints(int pointsToAdd)
    {
        currentPoints += pointsToAdd;
        UpdatePointsDisplay();
    }
}
