using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollide : Monster
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player")) // Use tag comparison
        {
            Player player = collision.transform.GetComponent<Player>();

            // If the player is using an item, destroy the monster
            if (player.isUsingItem)
            {
                Destroy(gameObject); // Destroy the monster
                return; // Exit early
            }

            // Check the direction of the collision
            Vector2 hit = collision.contacts[0].normal;
            float angle = Vector2.Angle(hit, Vector2.up);

            if (angle >= 135 && angle <= 225) // Player hit the monster from above
            {
                Debug.Log("monters die");
                PlayerJumped(collision.transform.GetComponent<Rigidbody2D>());
                Destroy(gameObject); // Destroy the monster GameObject
            }
            else // Player hits the monster from any other direction
            {
                // Only mark the player as dead if they are alive
                if (player.isAlive)
                {
                    // Disable all the player's colliders instead of deactivating the player
                    Collider2D[] colliders = player.GetComponents<Collider2D>(); // Get all colliders attached to the player
                    foreach (Collider2D collider in colliders)
                    {
                        collider.enabled = false; // Disable each collider
                    }

                    // Call the method to restart the game
                    GameManager gameManager = FindObjectOfType<GameManager>(); // Find the GameManager
                    if (gameManager != null)
                    {
                        gameManager.RestartGame(); // Call the RestartGame method
                    }
                }
            }
        }
    }
}
