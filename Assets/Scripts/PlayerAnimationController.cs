using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateAnimationWithVelocity(Vector3 currentVelocity)
    {
        if (animator.GetBool("IsAlive"))
        {
            // Check if there is movement
            if (currentVelocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);

                // Set animation based on velocity direction
                if (Mathf.Abs(currentVelocity.x) > Mathf.Abs(currentVelocity.y))
                {
                    // Moving horizontally
                    if (currentVelocity.x > 0)
                    {
                        animator.SetInteger("MovementDirection", 2); // Right
                        spriteRenderer.flipX = false; // Face right
                    }
                    else
                    {
                        animator.SetInteger("MovementDirection", 4); // Left
                        spriteRenderer.flipX = true; // Face left
                    }
                }
                else
                {
                    // Moving vertically
                    if (currentVelocity.y > 0)
                    {
                        animator.SetInteger("MovementDirection", 1); // Up
                        spriteRenderer.flipX = false; // Always face up
                    }
                    else
                    {
                        animator.SetInteger("MovementDirection", 3); // Down
                        spriteRenderer.flipX = false; // Always face down
                    }
                }
            }
            else
            {
                // Not moving
                animator.SetBool("isWalking", false);
                animator.SetInteger("MovementDirection", 0); // Idle
            }
        }
        else
        {
            animator.SetBool("IsAlive", false); // If needed for "death" state
        }
    }
}
