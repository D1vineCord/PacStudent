using System.Collections;
using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 5f;        // Movement speed
    public Vector3[] movementPoints;    // The four corner points for the movement
    private int currentTargetIndex = 0; // Index for the current target point
    private Vector3 lastPosition;       // To calculate velocity
    private Animator animator;          // Reference to the Animator
    private AudioSource audioSource;    // Reference to the AudioSource

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (movementPoints.Length == 0)
        {
            Debug.LogError("No movement points set!");
            return;
        }

        currentTargetIndex = 0;
        lastPosition = transform.position; // Initialize lastPosition
    }

    void Update()
    {
        MoveTowardsTarget();

        // Play the audio while moving
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        // Calculate the velocity and send it to the animation controller
        Vector3 currentVelocity = GetCurrentVelocity();

        // Assuming you're using PlayerAnimationController on the same GameObject
        PlayerAnimationController playerAnimController = GetComponent<PlayerAnimationController>();
        if (playerAnimController != null)
        {
            playerAnimController.UpdateAnimationWithVelocity(currentVelocity); // Pass the velocity to the animation controller
        }

        lastPosition = transform.position; // Update last position after moving
    }

    private void MoveTowardsTarget()
    {
        Vector3 targetPosition = movementPoints[currentTargetIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % movementPoints.Length;
        }
    }

    public Vector3 GetCurrentVelocity()
    {
        return (transform.position - lastPosition) / Time.deltaTime;
    }
}
