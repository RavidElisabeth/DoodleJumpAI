using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewVisualizer : MonoBehaviour
{
    // How far the player can see
    public float viewDistance = 5f;

    // Layer mask to define what is visible (e.g., walls or obstacles in 2D)
    public LayerMask viewMask;

    // Layer mask for detecting monsters in 2D
    public LayerMask monsterMask;

    // Gizmo flag to toggle debugging lines in the editor (optional)
    public bool drawDebugLines = true;

    // Update is called once per frame
    void Update()
    {
        // Visualize the rays in 45-degree increments (360 degrees / 45 = 8 rays)
        for (int i = 0; i < 8; i++)
        {
            // Calculate the direction of the ray based on the angle
            float angle = i * 45f;
            Vector2 direction = AngleToDirection(angle);

            // Perform raycast for obstacles (using Physics2D)
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, viewMask);
            bool hitObstacle = false;
            if (hit.collider != null)
            {
                // If the ray hits an obstacle, draw a line to the hit point
                if (drawDebugLines)
                    Debug.DrawLine(transform.position, hit.point, Color.red); // Red for blocked view
                hitObstacle = true; // Mark that this ray hit an obstacle
            }
            else
            {
                // If no obstacle is hit, draw a line to the max view distance
                if (drawDebugLines)
                    Debug.DrawLine(transform.position, (Vector2)transform.position + direction * viewDistance, Color.green); // Green for clear line of sight
            }

            // Perform raycast in the same direction for detecting monsters (using Physics2D)
            RaycastHit2D monsterHit = Physics2D.Raycast(transform.position, direction, viewDistance, monsterMask);
            if (monsterHit.collider != null)
            {
                // If a monster is detected within view distance and no obstacle is in the way
                if (!hitObstacle || (hit.distance > monsterHit.distance))
                {
                    if (drawDebugLines)
                    {
                        Debug.DrawLine(transform.position, monsterHit.point, Color.blue); // Blue for monster detection
                        Debug.DrawRay(monsterHit.point, Vector2.up * 0.5f, Color.blue); // Optional: Draw a small upward line at the monster's position
                    }
                }
            }
        }
    }

    // Convert an angle in degrees to a direction vector for 2D
    Vector2 AngleToDirection(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }
}
