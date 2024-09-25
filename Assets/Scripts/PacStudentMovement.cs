using System.Collections;
using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 5f;        // Movement speed
    public Vector3[] movementPoints;    // The four corner points for the movement
    private int currentTargetIndex = 0; // Index for the current target point
    private Animator animator;          // Reference to the Animator
    private AudioSource audioSource;    // Reference to the AudioSource

    void Start()
    {
        // Initialize animator and audioSource
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Ensure movement points are set
        if (movementPoints.Length == 0)
        {
            Debug.LogError("No movement points set!");
            return;
        }

        // Set initial target to the first point
        currentTargetIndex = 0;
    }

    void Update()
    {
        // Move PacStudent toward the current target point
        MoveTowardsTarget();

        // Play the animation and audio while moving
        if (!audioSource.isPlaying)
        {
            audioSource.Play(); // Play movement sound
        }

        animator.SetBool("isWalking", true);
    }

    private void MoveTowardsTarget()
    {
        // Get the current target point
        Vector3 targetPosition = movementPoints[currentTargetIndex];

        // Move PacStudent towards the target at a consistent speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if we've reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Move to the next target point in the array (wrap around if needed)
            currentTargetIndex = (currentTargetIndex + 1) % movementPoints.Length;
        }
    }
}
