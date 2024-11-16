using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public int gridSize = 7;  // Tamaño de la cuadrícula (7x7)
    public float cellSize = 1f;  // Tamaño de cada celda
    public Color gridColor = Color.green;  // Color de la cuadrícula

    // Dibuja la cuadrícula usando Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // Dibujamos las líneas horizontales
        for (int x = 0; x <= gridSize; x++)
        {
            Vector3 startPos = new Vector3(x * cellSize, 0, 0);
            Vector3 endPos = new Vector3(x * cellSize, gridSize * cellSize, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        // Dibujamos las líneas verticales
        for (int y = 0; y <= gridSize; y++)
        {
            Vector3 startPos = new Vector3(0, y * cellSize, 0);
            Vector3 endPos = new Vector3(gridSize * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
