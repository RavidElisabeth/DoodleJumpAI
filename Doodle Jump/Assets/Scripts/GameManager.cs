using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    private Vector3 initialCameraPosition; // Holds the initial camera position

    // All platforms
    public GameObject regularPlatformPrefab;
    public GameObject movingPlatformPrefab;
    public GameObject disappearingPlatformPrefab;
    public GameObject brokenPlatformPrefab;

    public Transform player; // Reference player here
    private float spawnY = -2.0f; // Y position for new platform
    private float spawnX; // X position for new platform
    private float minSpacing = 0.5f; // Minimum spacing between platforms
    private float maxSpacing = 0.5f; // Maximum spacing between platforms
    private float score = 0; // Holds the current score
    private bool createdBrokenPlatform = false; // Ensure 2 broken platforms do not spawn one after another

    // Probabilities for spawning different types of platforms
    private float regularThreshold = 0.9f;
    private float brokenThreshold = 1f;
    private float movingThreshold = 0.99f;
    private float disappearingThreshold = 1f;

    private GameObject currentSpawnedPlatform;
    private List<GameObject> activePlatforms = new List<GameObject>(); // List to track spawned platforms


    public GameObject monsterPrefab; // Reference to the monster prefab
    private bool spawnedMonster = false;
    private int timesWithoutSpawning = 0;
    private float lastCheckedScore = 0; // Used to see how long it's been since monster spawn

    public CameraFollow cameraFollow; // Cam follow script
    public GameObject obstaclesContainer; // Container for all obstacles/platforms

    // UI stuff
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Store the initial camera position
        initialCameraPosition = Camera.main.transform.position;

        // Create initial platform at the start of the game
        for (int i = 0; i < 24; i++)
        {
            SpawnPlatform(true);
        }

        // Update initial score display
        scoreText.text = "Score: " + score;
    }

    void FixedUpdate()
    {
        // Check if player is present
        if (player != null)
        {
            // Update score
            UpdateScore();

            // Spawn platforms based on player's position
            if (player.position.y > spawnY - (12 * minSpacing))
            {
                SpawnPlatform(false);
            }

            // Spawn monsters based on score
            if (score > 2000 && score - lastCheckedScore >= 1000 && !spawnedMonster && score != 0)
            {
                float randomChance = Random.Range(0f, 1f); // Random chance for spawning a monster

                if (randomChance > 0.7f) // 30% chance to spawn a monster
                {
                    spawnedMonster = true;
                    SpawnMonster();
                }

                lastCheckedScore = score;
            }

            // Reset monster spawning after a delay or distance has passed
            if (spawnedMonster && score - lastCheckedScore >= 500) // Adjust the 500 value to control how frequently monsters spawn
            {
                spawnedMonster = false; // Allow another monster to spawn
            }
        }
    }

    void SpawnPlatform(bool isStart)
    {
        float randomValue = Random.value;
        createdBrokenPlatform = false;

        AdjustSpacing();
        CalculatePlatformThresholds();

        spawnX = Random.Range(-2.5f, 2.5f); // Get random x pos for platform
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        // Decide which kind of platform to spawn
        if (randomValue < regularThreshold)
        {
            currentSpawnedPlatform = Instantiate(regularPlatformPrefab, spawnPos, Quaternion.identity, obstaclesContainer.transform);
        }
        else if (randomValue < brokenThreshold && !isStart && score < 5000)
        {
            currentSpawnedPlatform = Instantiate(brokenPlatformPrefab, spawnPos, Quaternion.identity, obstaclesContainer.transform);
            createdBrokenPlatform = true;
        }
        else if (randomValue < movingThreshold && score > 3000)
        {
            currentSpawnedPlatform = Instantiate(movingPlatformPrefab, spawnPos, Quaternion.identity, obstaclesContainer.transform);
        }
        else if (randomValue < disappearingThreshold && score >= 7000)
        {
            currentSpawnedPlatform = Instantiate(disappearingPlatformPrefab, spawnPos, Quaternion.identity, obstaclesContainer.transform);
        }
        else
        {
            currentSpawnedPlatform = Instantiate(regularPlatformPrefab, spawnPos, Quaternion.identity, obstaclesContainer.transform);
        }

        // Add the spawned platform to the active platforms list
        activePlatforms.Add(currentSpawnedPlatform);

        // Increase y pos for next platform
        spawnY += Random.Range(minSpacing, maxSpacing);
    }


    void SpawnMonster()
    {
        // Define a spawn position for the monster
        float spawnX = Random.Range(-2.5f, 2.5f); // Random x position
        float spawnYMonster = Random.Range(spawnY, spawnY + 1.0f); // Slightly above the last platform
        Vector3 spawnPosition = new Vector3(spawnX, spawnYMonster, 0);

        // Instantiate the monster prefab at the calculated spawn position
        Instantiate(monsterPrefab, spawnPosition, Quaternion.identity, obstaclesContainer.transform);
    }

    void Update()
    {
        DespawnPlatformsOutsideCamera(); // Call despawn check
        CheckPlayerFall(); // Check if the player has fallen below the camera
    }

    private void DespawnPlatformsOutsideCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null) return;

        // Get camera bounds in world coordinates
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        // Define the boundaries
        float leftBound = cameraPosition.x - (cameraWidth / 2);
        float rightBound = cameraPosition.x + (cameraWidth / 2);
        float bottomBound = cameraPosition.y - (cameraHeight / 2);

        // Check each platform
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            GameObject platform = activePlatforms[i];
            // Check if the platform is below the bottom bound or outside the left/right bounds
            if (platform != null && (platform.transform.position.y < bottomBound || platform.transform.position.x < leftBound || platform.transform.position.x > rightBound))
            {
                Destroy(platform); // Destroy the platform
                activePlatforms.RemoveAt(i); // Remove from the list
            }
            else if (platform == null)
            {
                // If the platform is null, just remove it from the activePlatforms list
                activePlatforms.RemoveAt(i);
            }
        }
    }

    private void CheckPlayerFall()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null || player == null) return;

        // Get camera bounds in world coordinates
        float cameraHeight = mainCamera.orthographicSize * 2;
        Vector3 cameraPosition = mainCamera.transform.position;

        // Define the lower boundary
        float bottomBound = cameraPosition.y - (cameraHeight / 2);

        // Check if the player has fallen below the lower boundary
        if (player.position.y < bottomBound)
        {
            Debug.Log("Player has fallen below the camera! Restarting game.");
            RestartGame(); // Call the new restart method
        }
    }



    public void RestartGame()
    {
        // Clear existing platforms
        ClearPlatforms();

        // Reset the game state
        ResetGameState();

        // Reset player state
        if (player != null)
        {
            player.GetComponent<Player>().ResetPlayer(); // Reset player status
            player.gameObject.SetActive(true); // Ensure the player is active
        }

        // Reset the camera position
        Camera.main.transform.position = initialCameraPosition;

        // Optionally, spawn initial platforms
        SpawnInitialPlatforms();

        // Ensure the player's position is at the default starting position
        player.position = new Vector3(0, 0.11f, 0);

        player.gameObject.SetActive(true); // Ensure the player is active

        // Disable all the player's colliders instead of deactivating the player
        Collider2D[] colliders = player.GetComponents<Collider2D>(); // Get all colliders attached to the player
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true; // Disable each collider
        }
    }




    private void ClearPlatforms()
    {
        // Destroy all existing platforms in the obstaclesContainer
        foreach (Transform child in obstaclesContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ResetGameState()
    {
        score = 0;
        spawnY = -2.0f;
        minSpacing = 0.5f;
        maxSpacing = 0.5f;

        // Reset platform spawn probabilities
        regularThreshold = 0.9f;
        brokenThreshold = 1f;
        movingThreshold = 0.99f;
        disappearingThreshold = 1f;

        // Clear any other game state if needed
        // e.g., reset monster spawn variables if necessary
        spawnedMonster = false;
        timesWithoutSpawning = 0;
    }


    private void SpawnInitialPlatforms()
    {
        spawnY = -2.0f; // Reset spawnY to start position
        for (int i = 0; i < 24; i++)
        {
            SpawnPlatform(true);
        }
        // Make sure to set spawnY to the last spawned platform's Y position after spawning
        if (activePlatforms.Count > 0)
        {
            spawnY = activePlatforms[activePlatforms.Count - 1].transform.position.y;
        }
    }

    void UpdateScore()
    {
        if (player.position.y > score / 50) // Assuming score is based on player height
        {
            score = Mathf.FloorToInt(player.position.y * 50);
            scoreText.text = "Score: " + score;
        }
    }

    // Change platform spawn probabilities with higher scores
    void CalculatePlatformThresholds()
    {
        if (score < 3000)
        {
            regularThreshold = 0.9f;
            brokenThreshold = 1f;
        }
        else if (score < 7000)
        {
            regularThreshold = 0.75f;
            brokenThreshold = 0.85f;
            movingThreshold = 0.99f;
            disappearingThreshold = 1f;
        }
        else if (score > 10000)
        {
            regularThreshold = 0.7f;
            brokenThreshold = 0.8f;
            movingThreshold = 0.98f;
            disappearingThreshold = 1f;
        }
        else if (score > 15000)
        {
            regularThreshold = 0.65f;
            brokenThreshold = 0.75f;
            movingThreshold = 0.96f;
            disappearingThreshold = 1f;
        }
    }

    void AdjustSpacing()
    {
        // Adjust platform spacing based on the score
        if (score < 1000)
        {
            minSpacing = 0.5f;
            maxSpacing = 0.5f;
        }
        else if (score < 2000)
        {
            minSpacing = 0.6f;
            maxSpacing = 0.6f;
        }
        else if (score < 5000)
        {
            minSpacing = 0.7f;
            maxSpacing = 0.7f;
        }
        else
        {
            minSpacing = 0.8f;
            maxSpacing = 0.8f;
        }
    }
}
