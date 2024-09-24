using System.Collections;
using System.Collections.Generic;
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

    void Update()
    {
        if(animator.GetBool("IsAlive") == true){
            if (Input.GetKey(KeyCode.W)){
            animator.SetBool("isWalking", true);
            animator.SetInteger("MovementDirection", 1); // Corrected this line
            spriteRenderer.flipX = false;
            }
            else if (Input.GetKey(KeyCode.D)){
            animator.SetBool("isWalking", true);
            animator.SetInteger("MovementDirection", 2); // Corrected this line
            spriteRenderer.flipX = false;
            }
            else if (Input.GetKey(KeyCode.S)){
            animator.SetBool("isWalking", true);
            animator.SetInteger("MovementDirection", 3); // Corrected this line
            spriteRenderer.flipX = false;
            }
            else if (Input.GetKey(KeyCode.A)){
            animator.SetBool("isWalking", true);
            animator.SetInteger("MovementDirection", 4); // Corrected this line
            spriteRenderer.flipX = true;
            }
            else{
            animator.SetBool("isWalking", false);
            animator.SetInteger("MovementDirection", 0);
            }
        }
        else{
            animator.SetBool("IsAlive", false);
        }        
    }
}
