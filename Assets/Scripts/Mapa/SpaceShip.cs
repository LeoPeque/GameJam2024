using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public float xPosition = 0.5f; // Coordenada X inicial con valor decimal
    public float yPosition = 0.5f; // Coordenada Y inicial con valor decimal
    public int energy = 10; // Energ�a inicial

    private float gridSize = 7f; // Tama�o de la matriz (7x7)
    private Vector2 shipPosition; // Posici�n de la nave en el mundo

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
                Move(0f, 1f); // Mover en direcci�n Y positiva
            }
            else if (Input.GetKeyDown(KeyCode.S)) // Mover hacia abajo
            {
                Move(0f, -1f); // Mover en direcci�n Y negativa
            }
            else if (Input.GetKeyDown(KeyCode.A)) // Mover hacia la izquierda
            {
                Move(-1f, 0f); // Mover en direcci�n X negativa
            }
            else if (Input.GetKeyDown(KeyCode.D)) // Mover hacia la derecha
            {
                Move(1f, 0f); // Mover en direcci�n X positiva
            }
        }
    }

    // Funci�n para mover la nave y actualizar energ�a
    void Move(float deltaX, float deltaY)
    {
        // Calculamos la nueva posici�n (aceptando valores decimales)
        float newX = xPosition + deltaX;
        float newY = yPosition + deltaY;

        // Comprobar si la nave sigue dentro del rango de la matriz (a�n puedes permitirle que se mueva fuera de los l�mites si lo deseas)
        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
        {
            xPosition = newX;
            yPosition = newY;

            // Reducir energ�a
            energy -= 1;
            Debug.Log("Energ�a restante: " + energy);

            // Actualizar la posici�n en el mundo (si tienes un sprite de la nave)
            shipPosition = new Vector2(xPosition, yPosition);
            UpdatePosition();
        }
        else
        {
            Debug.Log("Movimiento fuera de los l�mites del mapa.");
        }
    }

    // Funci�n para actualizar la posici�n de la nave en el mundo (en caso de tener un sprite)
    void UpdatePosition()
    {
        transform.position = shipPosition; // Mueve la nave en la pantalla
    }
}
