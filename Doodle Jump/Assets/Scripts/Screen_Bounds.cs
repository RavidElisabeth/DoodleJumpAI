using UnityEngine;
using System.Collections;

public class Screen_Bounds : MonoBehaviour
{
    public float leftConstraint = 0.0f;
    public float rightConstraint = 0.0f;
    public Transform target;
    public float buffer = 1.0f; // set this so the spaceship disappears offscreen before re-appearing on other side
    public float distanceZ = 10.0f;

    void Awake()
    {
        // using a specific distance to determine screen bounds
        leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
    }

    void Update()
    {
        // Get current position of the target every frame
        float targetX = target.position.x;

        // If the target is beyond the left constraint, move it to the right
        if (targetX < leftConstraint - buffer)
        {
            targetX = rightConstraint + buffer;
        }

        // If the target is beyond the right constraint, move it to the left
        if (targetX > rightConstraint + buffer)
        {
            targetX = leftConstraint - buffer;
        }

        // Update the target's position
        target.position = new Vector3(targetX, target.position.y, target.position.z);
    }
}
