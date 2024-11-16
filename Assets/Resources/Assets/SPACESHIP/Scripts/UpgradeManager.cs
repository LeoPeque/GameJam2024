using UnityEngine;
using UnityEngine.UI;

public class BarUpgradeManager : MonoBehaviour
{
    [System.Serializable]
    public class BarButtonPair
    {
        public Button button;       
        public Transform barContainer; 
        public GameObject barPrefab;   
    }

    public BarButtonPair[] barButtonPairs; 
    private int[] barLevels; 

    void Start()
    {
        barLevels = new int[barButtonPairs.Length];

        for (int i = 0; i < barButtonPairs.Length; i++)
        {
            int index = i; 
            Debug.Log($"Assigning Button {barButtonPairs[i].button.name} to Bar {index}");
            barButtonPairs[i].button.onClick.AddListener(() => UpgradeBar(index));
        }
    }

    public void UpgradeBar(int barIndex)
    {
        if (barIndex < 0 || barIndex >= barButtonPairs.Length)
        {
            Debug.LogError("Invalid bar index: " + barIndex);
            return;
        }

        BarButtonPair pair = barButtonPairs[barIndex];

        if (pair.barPrefab == null || pair.barContainer == null)
        {
            Debug.LogError("Bar prefab or container is not assigned for bar index: " + barIndex);
            return;
        }

        GameObject newBar = Instantiate(pair.barPrefab);
        newBar.transform.SetParent(pair.barContainer, false); 

        barLevels[barIndex]++;
        Debug.Log($"Bar {barIndex + 1} upgraded to level {barLevels[barIndex]}");
    }
}
