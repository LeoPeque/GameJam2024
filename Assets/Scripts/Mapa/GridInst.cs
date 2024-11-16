using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInst : MonoBehaviour
{
    [SerializeField] GameObject gridPrefab;  // Prefab de las celdas
    [SerializeField] int number = 7;         // Tamaño de la cuadrícula (7x7)
    [SerializeField] float cellSize = 1f;    // Tamaño de cada celda

    // Start is called before the first frame update
    void Start()
    {
        // La esquina inferior izquierda empieza en (0.5, 0.5)
        float offsetX = 0.5f;
        float offsetY = 0.5f;

        for (int i = 0; i < number; i++) // Recorre filas
        {
            for (int y = 0; y < number; y++) // Recorre columnas
            {
                // Instanciar la celda
                GameObject grid = Instantiate(gridPrefab) as GameObject;

                // Ajustar la posición de la celda considerando el desplazamiento
                grid.transform.position = new Vector3(
                    i * cellSize + offsetX,
                    y * cellSize + offsetY,
                    0f);

                // Hacer que la celda sea un hijo del GameObject que contiene este script
                grid.transform.parent = this.transform;
            }
        }
    }
}
