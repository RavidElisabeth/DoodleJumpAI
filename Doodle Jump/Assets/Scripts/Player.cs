using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float movementSpeed = 10f;

    // New properties
    public bool isUsingItem = false;  // Indicates if the player is using an item
    public bool isAlive = true;        // Indicates if the player is alive
    public bool diedToMonster = false; // Indicates if the player died from a monster

    float movement = 0f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal") * movementSpeed;

        // Example check for using an item
        if (Input.GetKeyDown(KeyCode.Space)) // Assume space bar is used for items
        {
            isUsingItem = !isUsingItem; // Toggle item usage
        }
    }

    public void ResetPlayer()
    {
        bool isUsingItem = false;  // Indicates if the player is using an item
        bool isAlive = true;        // Indicates if the player is alive
        bool diedToMonster = false; // Indicates if the player died from a monster

        movement = 0f;
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = movement;
        rb.velocity = velocity;

        // Optionally handle player death logic
        if (!isAlive)
        {
            // Example logic for player death
            // You can trigger animations, reset the game, etc.
            HandleDeath();
        }
    }

    // Example method to handle player death
    void HandleDeath()
    {
        // Disable player movement
        rb.velocity = Vector2.zero;

        // You could add logic here to show a death animation or restart the game
        gameObject.SetActive(false); // Disable the player GameObject (or restart the game)
    }

    // You may also want to add methods for player respawn, collecting items, etc.
}
