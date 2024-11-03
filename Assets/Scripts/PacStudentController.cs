using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioClip pelletAudioClip;
    public AudioClip movementAudioClip;
    public ParticleSystem dustEffect;
    
    public Tilemap[] tilemaps;
    
    private Vector3 targetPosition;
    private bool isLerping;
    private Vector3 lastInput;
    private AudioSource audioSource;
    private PlayerAnimationController animationController;

    private void Awake()
    {
        tilemaps = FindObjectsOfType<Tilemap>();
        Debug.Log("Found Tilemaps: " + tilemaps.Length);
        animationController = GetComponent<PlayerAnimationController>();
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

        if (isLerping)
        {
            LerpToTarget();
        }
        else
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
                SetDustEffectRotation(direction);
                animationController.UpdateAnimationWithVelocity(direction); // Update animation based on movement
            }
            else
            {
                animationController.UpdateAnimationWithVelocity(Vector3.zero); // Idle animation
                if (dustEffect.isPlaying)
                {
                    dustEffect.Stop();
                }
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
        float rayLength = 1f;
        Vector3 rayOrigin = transform.position + direction * 0.1f;

        int wallLayer = LayerMask.GetMask("Walls");
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayLength, wallLayer);
        Debug.DrawRay(rayOrigin, direction * rayLength, Color.red, 0.1f);

        return hit.collider == null;
    }

    private bool IsAboutToEatPellet()
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(targetPosition);
            TileBase pelletTile = tilemap.GetTile(cellPosition);
            if (pelletTile != null && pelletTile.name == "sprite_0_0")
            {
                return true;
            }
        }

        Collider2D powerPelletCollider = Physics2D.OverlapCircle(targetPosition, 0.5f, LayerMask.GetMask("PowerPallets"));
        return powerPelletCollider != null && powerPelletCollider.CompareTag("PowerPallet");
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
            dustEffect.transform.rotation = Quaternion.Euler(90, 90, 0);
        else if (direction == Vector3.down)
            dustEffect.transform.rotation = Quaternion.Euler(-90, 90, 0);
        else if (direction == Vector3.left)
            dustEffect.transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (direction == Vector3.right)
            dustEffect.transform.rotation = Quaternion.Euler(0, 270, -90);
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
            audioSource.clip = IsAboutToEatPellet() ? pelletAudioClip : movementAudioClip;
            audioSource.Play();
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
