using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;         // Drag your player here
    public Vector3 offset = new Vector3(0f, 3f, -5f); // Distance behind/above player
    public float smoothSpeed = 10f;  // How smoothly the camera catches up

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate the target position based on the player's position + offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // Ensure the camera always looks at the player
        transform.LookAt(player.position + Vector3.up * 1f); // Added slight height offset so it looks at the chest/head area
    }
}