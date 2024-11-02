using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioClip pelletAudioClip;
    public AudioClip movementAudioClip;
    // public ParticleSystem dustEffect;

    public Tilemap[] tilemaps; // Array to store multiple tilemaps

    private Vector3 targetPosition;
    private bool isLerping;
    private Vector3 lastInput;
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
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector3.up;
        if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector3.down;
        if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector3.right;
    }

    private bool IsWalkable(Vector3 direction)
{
    // Define the ray's starting position and length
    Vector3 rayOrigin = transform.position;
    float rayLength = 0.6f; // Adjust based on tile size and character collider size

    // Cast a ray in the direction of movement
    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayLength, LayerMask.GetMask("Walls"));

    // Debug the ray to visualize it in the Scene view
    Debug.DrawRay(rayOrigin, direction * rayLength, Color.red, 0.1f);

    // If the ray hits something in the "Walls" layer, movement is blocked
    if (hit.collider != null)
    {
        Debug.Log("Raycast hit a wall: " + hit.collider.name);
        return false;
    }

    return true;
}


    private bool IsAboutToEatPellet()
    {
        Vector3Int pelletPosition = tilemaps[0].WorldToCell(targetPosition);

        foreach (Tilemap tilemap in tilemaps)
        {
            TileBase pelletTile = tilemap.GetTile(pelletPosition);
            if (pelletTile != null && pelletTile.name == "sprite_0_0") // Adjust based on pellet tile name
            {
                return true; // Found a pellet
            }
        }

        return false;
    }

    private void StartLerping(Vector3 direction)
    {
        targetPosition = transform.position + direction;
        isLerping = true;
        PlayMovementAudio();
        // dustEffect.Play();
    }

    private void LerpToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isLerping = false;
            StopMovementAudio();
            // dustEffect.Stop();
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
