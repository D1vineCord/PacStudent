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
    float rayLength = 0.5f; // Adjust based on character size and tile size

    // Define ray origins with slight offsets to cover character bounds
    Vector3 rayOriginCenter = transform.position + direction;
    Vector3 rayOriginLeft = rayOriginCenter + new Vector3(0, 0, 0); // Offset left
    Vector3 rayOriginRight = rayOriginCenter + new Vector3(0, 0, 0); // Offset right

    // Cast three rays and only consider collisions with the "Walls" layer
    int wallLayer = LayerMask.NameToLayer("Walls");
    bool hitCenter = Physics2D.Raycast(rayOriginCenter, direction, rayLength, 1 << wallLayer);
    bool hitLeft = Physics2D.Raycast(rayOriginLeft, direction, rayLength, 1 << wallLayer);
    bool hitRight = Physics2D.Raycast(rayOriginRight, direction, rayLength, 1 << wallLayer);

    // Draw debug rays to visualize in the Scene view
    Debug.DrawRay(rayOriginCenter, direction * rayLength, Color.red, 0.1f);
    Debug.DrawRay(rayOriginLeft, direction * rayLength, Color.red, 0.1f);
    Debug.DrawRay(rayOriginRight, direction * rayLength, Color.red, 0.1f);

    // Log if any ray hits something in the Walls layer
    if (hitCenter || hitLeft || hitRight)
    {
        Debug.Log("Hit wall in the Walls layer, blocking movement.");
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
