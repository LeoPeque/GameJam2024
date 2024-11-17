using System.Collections.Generic;
using UnityEngine;

public class InvadersBehavior : MonoBehaviour
{
    private Vector2Int currentGridPosition; // Invader's current position on the grid
    private Vector2Int targetPosition; // Target grid position (player position)
    private Vector2Int playerPreviousGridPosition; // Player's previous grid position

    private float gridSize = 1f; // Size of each grid cell
    private int currentEnergy = 10; // Current player energy

    void Start()
    {
        // Initialize invader position on the grid and center it
        currentGridPosition = Vector2Int.FloorToInt(transform.position / gridSize);
        transform.position = new Vector3(
            currentGridPosition.x * gridSize + gridSize / 2,
            currentGridPosition.y * gridSize + gridSize / 2,
            0f
        );
    }

    void Update()
    {
        // Find the player spaceship in the scene
        SpaceShip playerSpaceship = FindObjectOfType<SpaceShip>();

        if (playerSpaceship != null)
        {
            // Get player's grid position
            Vector2Int playerGridPosition = Vector2Int.FloorToInt(playerSpaceship.transform.position / gridSize);

            // Check if the player has moved or energy has changed
            if (playerGridPosition != playerPreviousGridPosition || currentEnergy != playerSpaceship.energy)
            {
                // Update target position
                targetPosition = playerGridPosition;
                playerPreviousGridPosition = playerGridPosition;
                currentEnergy = playerSpaceship.energy;

                // Calculate the next greedy move
                MakeGreedyMove();
            }
        }
    }

    void MakeGreedyMove()
    {
        Vector2Int bestMove = currentGridPosition;
        int bestCost = CalculateCost(currentGridPosition);

        // Evaluate all possible moves
        foreach (Vector2Int direction in GetCardinalDirections())
        {
            Vector2Int newPosition = currentGridPosition + direction;

            if (IsPositionValid(newPosition))
            {
                int newCost = CalculateCost(newPosition);

                if (newCost < bestCost)
                {
                    bestCost = newCost;
                    bestMove = newPosition;
                }
            }
        }

        // Update invader's position
        currentGridPosition = bestMove;
        transform.position = new Vector3(
                currentGridPosition.x * gridSize + gridSize / 2,
                currentGridPosition.y * gridSize + gridSize / 2,
                0f
            );
    }

    int CalculateCost(Vector2Int position)
    {
        // Greedy cost function (Manhattan distance to the target)
        return Mathf.Abs(targetPosition.x - position.x) + Mathf.Abs(targetPosition.y - position.y);
    }

    bool IsPositionValid(Vector2Int position)
    {
        // Check if the position is within bounds or avoids obstacles (optional)
        return true; // Add additional logic if needed
    }

    List<Vector2Int> GetCardinalDirections()
    {
        return new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }
}
