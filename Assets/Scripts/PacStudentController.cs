using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioClip pelletAudioClip;
    public AudioClip movementAudioClip;
    public ParticleSystem dustEffect;

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

            if (lastInput == Vector3.up && IsWalkable(Vector3.up))
                direction = Vector3.up;
            else if (lastInput == Vector3.down && IsWalkable(Vector3.down))
                direction = Vector3.down;
            else if (lastInput == Vector3.left && IsWalkable(Vector3.left))
                direction = Vector3.left;
            else if (lastInput == Vector3.right && IsWalkable(Vector3.right))
                direction = Vector3.right;

            if (direction != Vector3.zero)
            {
                StartLerping(direction);
                SetDustEffectRotation(direction); // Set rotation based on direction
            }
            else if (dustEffect.isPlaying)
            {
                dustEffect.Stop();
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
    // Loop through each tilemap to check for pellets
    foreach (Tilemap tilemap in tilemaps)
    {
        // Convert the target position to a cell position for the current tilemap
        Vector3Int cellPosition = tilemap.WorldToCell(targetPosition);
        
        // Get the tile at this cell position
        TileBase pelletTile = tilemap.GetTile(cellPosition);
        
        if (pelletTile != null && pelletTile.name == "sprite_0_0") // Adjust based on your pellet tile name
        {
            return true; // Pellet found, exit early
        }
    }

    // Check for nearby PowerPallet objects (assumes PowerPallet has a unique tag)
    Collider2D powerPelletCollider = Physics2D.OverlapCircle(targetPosition, 0.5f, LayerMask.GetMask("PowerPallets"));
    if (powerPelletCollider != null && powerPelletCollider.CompareTag("PowerPallet"))
    {
        Debug.Log("PowerPallet found!");
        return true;
    }

    return false;
}


    private void StartLerping(Vector3 direction)
    {
        targetPosition = transform.position + direction;
        isLerping = true;
        PlayMovementAudio();
        if (!dustEffect.isPlaying)
        {
            dustEffect.Play();
        }
    }

    private void SetDustEffectRotation(Vector3 direction)
    {
        if (direction == Vector3.up)
        {
            dustEffect.transform.rotation = Quaternion.Euler(90, 90, 0);
        }
        else if (direction == Vector3.down)
        {
            dustEffect.transform.rotation = Quaternion.Euler(-90, 90, 0);
        }
        else if (direction == Vector3.left)
        {
            dustEffect.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (direction == Vector3.right)
        {
            dustEffect.transform.rotation = Quaternion.Euler(0, 270, -90);
        }
    }

    private void LerpToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isLerping = false;
            StopMovementAudio();
        }
    }

    private void PlayMovementAudio()
    {
        if (CameraScript.isIntroComplete && !audioSource.isPlaying)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = IsAboutToEatPellet() ? pelletAudioClip : movementAudioClip;
                audioSource.Play();
            }
        }
        
    }

    private void StopMovementAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
