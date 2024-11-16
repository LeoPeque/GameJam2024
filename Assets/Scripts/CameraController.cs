using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's Transform
    public Vector3 offset = new Vector3(0, 0, -10); // Offset to keep the camera at the correct depth

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Update the camera's position to follow the player
            transform.position = playerTransform.position + offset;
        }
    }
}
