using UnityEngine;
using UnityEngine.UI;

public class GridLayoutWithScrollbar : MonoBehaviour
{
    public GameObject prefab; // The prefab to instantiate in the grid
    public int numberOfItems = 10; // Total number of items to display
    public Transform content; // Reference to the Content object in the ScrollView

    void Start()
    {
        // Populate the grid with prefabs
        PopulateGrid();
    }

    void PopulateGrid()
    {
        // Destroy any existing items in the content area
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new items based on the numberOfItems
        for (int i = 0; i < numberOfItems; i++)
        {
            Instantiate(prefab, content);
        }
    }
}
