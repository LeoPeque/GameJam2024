using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public float xPosition = 0.5f; // Coordenada X inicial con valor decimal
    public float yPosition = 0.5f; // Coordenada Y inicial con valor decimal
    public int energy = 10; // Energía inicial

    private float gridSize = 7f; // Tamaño de la matriz (7x7)
    private Vector2 shipPosition; // Posición de la nave en el mundo

    // Start se llama antes del primer frame
    void Start()
    {
        shipPosition = new Vector2(xPosition, yPosition);
        UpdatePosition();
    }

    // Update se llama una vez por frame
    void Update()
    {
        if (energy > 0)
        {
            if (Input.GetKeyDown(KeyCode.W)) // Mover hacia arriba
            {
                Move(0f, 1f); // Mover en dirección Y positiva
            }
            else if (Input.GetKeyDown(KeyCode.S)) // Mover hacia abajo
            {
                Move(0f, -1f); // Mover en dirección Y negativa
            }
            else if (Input.GetKeyDown(KeyCode.A)) // Mover hacia la izquierda
            {
                Move(-1f, 0f); // Mover en dirección X negativa
            }
            else if (Input.GetKeyDown(KeyCode.D)) // Mover hacia la derecha
            {
                Move(1f, 0f); // Mover en dirección X positiva
            }
        }
    }

    // Función para mover la nave y actualizar energía
    void Move(float deltaX, float deltaY)
    {
        // Calculamos la nueva posición (aceptando valores decimales)
        float newX = xPosition + deltaX;
        float newY = yPosition + deltaY;

        // Comprobar si la nave sigue dentro del rango de la matriz (aún puedes permitirle que se mueva fuera de los límites si lo deseas)
        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
        {
            xPosition = newX;
            yPosition = newY;

            // Reducir energía
            energy -= 1;
            Debug.Log("Energía restante: " + energy);

            // Actualizar la posición en el mundo (si tienes un sprite de la nave)
            shipPosition = new Vector2(xPosition, yPosition);
            UpdatePosition();
        }
        else
        {
            Debug.Log("Movimiento fuera de los límites del mapa.");
        }
    }

    // Función para actualizar la posición de la nave en el mundo (en caso de tener un sprite)
    void UpdatePosition()
    {
        transform.position = shipPosition; // Mueve la nave en la pantalla
    }
}
