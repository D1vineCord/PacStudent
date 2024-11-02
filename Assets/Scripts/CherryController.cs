using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab; // Reference to the cherry prefab
    public float spawnInterval = 10f; // Time interval for spawning cherries
    public float moveSpeed = 5f; // Speed at which the cherry moves

    private Camera mainCamera; // Reference to the main camera
    private GameObject currentCherry; // Store the currently active cherry

    // Define the bounds for the center area (modify these values as needed)
    public float centerAreaWidth = 5f; // Width of the center area
    public float centerAreaHeight = 5f; // Height of the center area

    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera
        StartCoroutine(CherrySpawnRoutine()); // Start the cherry spawn coroutine
    }

    private System.Collections.IEnumerator CherrySpawnRoutine()
    {
        while (true)
        {
            // Check if a cherry is already active
            if (currentCherry == null)
            {
                // Determine a random position within the center area
                Vector3 spawnPosition = GetRandomSpawnPosition();
                
                // Instantiate the cherry
                currentCherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);
                
                // Start moving the cherry
                yield return MoveCherry(currentCherry);
            }

            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Get the camera's viewport corners
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calculate boundaries for the spawn area
        float minX = mainCamera.transform.position.x - (centerAreaWidth / 2);
        float maxX = mainCamera.transform.position.x + (centerAreaWidth / 2);
        float minY = mainCamera.transform.position.y - (centerAreaHeight / 2);
        float maxY = mainCamera.transform.position.y + (centerAreaHeight / 2);

        // Randomly choose a spawn position within the defined area
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY, 0);
    }

    private System.Collections.IEnumerator MoveCherry(GameObject cherry)
    {
        // Determine the target position (move through the center area)
        Vector3 targetPosition = GetTargetPosition(cherry.transform.position);
        
        float journeyLength = Vector3.Distance(cherry.transform.position, targetPosition);
        float startTime = Time.time;

        while (cherry != null && Vector3.Distance(cherry.transform.position, targetPosition) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            cherry.transform.position = Vector3.Lerp(cherry.transform.position, targetPosition, fractionOfJourney);

            yield return null; // Wait until the next frame
        }

        // Destroy the cherry if it has reached the target position
        if (cherry != null)
        {
            Destroy(cherry);
            currentCherry = null; // Reset the current cherry reference
        }
    }

    private Vector3 GetTargetPosition(Vector3 startPosition)
    {
        // Calculate the target position in the center area
        float minX = mainCamera.transform.position.x - (centerAreaWidth / 2);
        float maxX = mainCamera.transform.position.x + (centerAreaWidth / 2);
        float minY = mainCamera.transform.position.y - (centerAreaHeight / 2);
        float maxY = mainCamera.transform.position.y + (centerAreaHeight / 2);

        // Choose a random target position on the opposite side of the screen within the center area
        float targetX = Random.Range(minX, maxX);
        float targetY = startPosition.y; // Keep the same Y value for horizontal movement
        if (startPosition.x < mainCamera.transform.position.x)
        {
            // Start from the left side, move to the right
            targetX = maxX; 
        }
        else
        {
            // Start from the right side, move to the left
            targetX = minX;
        }

        return new Vector3(targetX, targetY, 0);
    }
}
