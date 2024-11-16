using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int gridSize = 7; // Tama�o del mapa 7x7
    public GameObject cellPrefab; // Prefab para representar cada celda
    public float cellSize = 1f; // Tama�o de cada celda

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                Instantiate(cellPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
