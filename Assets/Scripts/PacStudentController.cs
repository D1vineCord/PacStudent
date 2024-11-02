using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioClip pelletAudioClip;
    public AudioClip movementAudioClip;
    //public ParticleSystem dustEffect;

    public Tilemap[] tilemaps; // Array to store multiple tilemaps

    private Vector3 targetPosition;
    private bool isLerping;
    private Vector3 currentDirection;
    private Vector3 lastInput;
    private Vector3 currentInput;
    private AudioSource audioSource;

    private void Awake()
{
    tilemaps = FindObjectsOfType<Tilemap>();
    Debug.Log("Found Tilemaps: " + tilemaps.Length);
}

    private void Start()
    {
        targetPosition = transform.position;
        isLerping = false;
        audioSource = GetComponent<AudioSource>();
    }

void Update()
{
    HandleInput();

    if (isLerping) // Call LerpToTarget if currently lerping
    {
        LerpToTarget();
    }
    else // Only check for input if not currently lerping
    {
        Vector3 direction = Vector3.zero;

        // Check the last input and try to move in that direction
        if (lastInput == Vector3.up && IsWalkable(Vector3.up))
        {
            direction = Vector3.up;
        }
        else if (lastInput == Vector3.down && IsWalkable(Vector3.down))
        {
            direction = Vector3.down;
        }
        else if (lastInput == Vector3.left && IsWalkable(Vector3.left))
        {
            direction = Vector3.left;
        }
        else if (lastInput == Vector3.right && IsWalkable(Vector3.right))
        {
            direction = Vector3.right;
        }

        if (direction != Vector3.zero)
        {
            StartLerping(direction);
        }
    }
}



private void HandleInput()
{
    if (Input.GetKeyDown(KeyCode.W)) 
    {
        lastInput = Vector3.up;
        Debug.Log("Input: Up");
    }
    if (Input.GetKeyDown(KeyCode.S)) 
    {
        lastInput = Vector3.down;
        Debug.Log("Input: Down");
    }
    if (Input.GetKeyDown(KeyCode.A)) 
    {
        lastInput = Vector3.left;
        Debug.Log("Input: Left");
    }
    if (Input.GetKeyDown(KeyCode.D)) 
    {
        lastInput = Vector3.right;
        Debug.Log("Input: Right");
    }
}


private bool IsWalkable(Vector3 direction)
{
    
    Vector3Int checkPosition = tilemaps[0].WorldToCell(transform.position + direction);
    
    // Make sure the position is within bounds of each tilemap
    foreach (Tilemap tilemap in tilemaps)
    {
        Debug.Log("Checking tile at position: " + checkPosition + " in tilemap: " + tilemap.name);

        if (tilemap.cellBounds.Contains(checkPosition))
        {
            TileBase tile = tilemap.GetTile(checkPosition);
            // Check if the tile is a wall
            if (tile != null && tile.name != "sprite_0_0") // Adjust the name to match your wall tiles
            {
                return false; // Found a wall, cannot walk here
            }
        }
    }

    // If we found no walls in any tilemap, it is walkable
    return true; 
}


private bool IsAboutToEatPellet()
{
    Vector3Int pelletPosition = tilemaps[0].WorldToCell(targetPosition);
    
    foreach (Tilemap tilemap in tilemaps)
    {
        TileBase pelletTile = tilemap.GetTile(pelletPosition);
        if (pelletTile != null && pelletTile.name == "sprite_0_0") // Adjust based on your pellet naming
        {
            return true; // Found a pellet
        }
    }

    return false; // No pellet found
}


private void StartLerping(Vector3 direction)
{
    Debug.Log("Starting lerp to: " + targetPosition);
    targetPosition = transform.position + direction;
    isLerping = true;
    PlayMovementAudio();
    //dustEffect.Play();
}


    private void LerpToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isLerping = false;
            StopMovementAudio();
            //dustEffect.Stop();
        }
    }

    private void PlayMovementAudio()
    {
        audioSource.clip = IsAboutToEatPellet() ? pelletAudioClip : movementAudioClip;
        audioSource.Play();
    }

    private void StopMovementAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
