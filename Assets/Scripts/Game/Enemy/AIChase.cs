using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIChase : MonoBehaviour
{
    private PlayerControl player;
    public float moveSpeed;
    public float distanceBetween;
    public float collisionOffset = 0.05f;
    public float damageCooldown = 1f; // Cooldown time between each damage instance
    public float damageRadius = 1.5f; // Radius within which damage is dealt
    public float knockbackForce = 5f; // Force to apply for knockback
    private float damageTimer = 0f; // Timer for tracking the cooldown
    public ContactFilter2D movementFilter;
    private Rigidbody2D rb; // Rigidbody2D component for applying knockback
    private Vector2 movementInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Health playerHealth; // Reference to the player's Health component
    private bool isDamaging; // Flag indicating if damage coroutine is running

    private void OnDestroy()
    {
        if (player != null)
        {
            player.EnemyDeleted();
        }
    }

    private void Start()
    {
    	
        player = PlayerControl.Instance;
        if (player == null)
        {
            Debug.LogError("Player reference is missing. Make sure the PlayerControl script is attached to the player object.");
        }

        rb = GetComponent<Rigidbody2D>(); // Assign the Rigidbody2D component

        // Get the player's Health component
        playerHealth = player.GetComponent<Health>();

        // Lock the rotation of the enemy's Rigidbody2D
        rb.freezeRotation = true;

        // Get the Animator component
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        // Check if the enemy is currently colliding with the player
        bool isCollidingWithPlayer = false;

        if (player != null)
        {
            Collider2D[] colliders = new Collider2D[5];
            int contactCount = rb.GetContacts(colliders);

            for (int i = 0; i < contactCount; i++)
            {
                if (colliders[i].gameObject == player.gameObject)
                {
                    isCollidingWithPlayer = true;
                    break;
                }
            }
        }

        // Stop the enemy from moving if colliding with the player
        if (isCollidingWithPlayer)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsWalk", false); // Set IsWalk parameter to false to play idle animation
            return; // Exit the FixedUpdate method to prevent further movement
        }

        Vector2 targetVelocity = movementInput * moveSpeed;

        // Check if the target velocity collides with any obstacles
        RaycastHit2D[] hits = new RaycastHit2D[5];
        int hitCount = rb.Cast(targetVelocity.normalized, movementFilter, hits, targetVelocity.magnitude * Time.fixedDeltaTime + collisionOffset);
        if (hitCount > 0)
        {
            // Adjust the target velocity based on collision response
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D collider = hits[i].collider;
                if (!collider.isTrigger) // Skip triggers
                {
                    Vector2 collisionOffset = hits[i].point - rb.position;
                    targetVelocity -= Vector2.Dot(targetVelocity, collisionOffset.normalized) * collisionOffset.normalized;
                }
            }
        }

        rb.velocity = targetVelocity;

        // Chase the player if within the specified distance
        if (player != null && damageTimer <= 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= distanceBetween)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                targetVelocity = direction * moveSpeed;
                distanceBetween = 1000000;

                // Check if the target velocity collides with any obstacles
                hitCount = rb.Cast(targetVelocity.normalized, movementFilter, hits, targetVelocity.magnitude * Time.fixedDeltaTime + collisionOffset);
                if (hitCount > 0)
                {
                    // Adjust the target velocity based on collision response
                    for (int i = 0; i < hitCount; i++)
                    {
                        Collider2D collider = hits[i].collider;
                        if (!collider.isTrigger) // Skip triggers
                        {
                            Vector2 collisionOffset = hits[i].point - rb.position;
                            targetVelocity -= Vector2.Dot(targetVelocity, collisionOffset.normalized) * collisionOffset.normalized;
                        }
                    }
                }

                rb.velocity = targetVelocity;

                // Start or continue damaging the player
                if (!isDamaging)
                {
                    StartCoroutine(DamagePlayerOverTime());
                }

                animator.SetBool("IsWalk", true); // Set IsWalk parameter to true to play walk animation
            }
            else
            {
                // Stop damaging the player if outside the damage radius
                if (isDamaging)
                {
                    StopCoroutine(DamagePlayerOverTime());
                    isDamaging = false;
                }

                animator.SetBool("IsWalk", false); // Set IsWalk parameter to false to play idle animation
            }

            if (targetVelocity.x < 0)
            {
                spriteRenderer.flipX = true; // Flip sprite to face left
            }
            else if (targetVelocity.x > 0)
            {
                spriteRenderer.flipX = false; // Do not flip sprite
            }
        }
        else
        {
            animator.SetBool("IsWalk", false); // Set IsWalk parameter to false to play idle animation
        }

        // Update the damage cooldown timer
        if (damageTimer > 0)
        {
            damageTimer -= Time.fixedDeltaTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
{
    // Check if the collided object is the player and the damage cooldown is over
    if (collision.gameObject == player.gameObject && damageTimer <= 0)
    {
        // Disable movement input
        movementInput = Vector2.zero;

        // Calculate knockback direction
        Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;

        // Apply knockback force to the enemy
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // Deal damage to the player
        playerHealth.GetHit(1, gameObject);

        // Reset the damage cooldown timer
        damageTimer = damageCooldown;
    }
}


    private IEnumerator DamagePlayerOverTime()
    {
        isDamaging = true;
        bool canDealDamage = true; // Flag to track if damage can be dealt

        // Continuously deal damage to the player within the damage radius
        while (isDamaging)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= damageRadius)
            {
                // Check if damage can be dealt based on the cooldown timer
                if (canDealDamage)
                {
                    playerHealth.GetHit(1, gameObject);

                    // Set the cooldown timer
                    canDealDamage = false;
                    yield return new WaitForSeconds(damageCooldown);
                    canDealDamage = true;
                }
            }

            yield return null;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
{
    // Check if the collided object is the player's weapon
    if (collision.gameObject.CompareTag("PlayerWeapon"))
    {
        // Apply knockback to the enemy
        Vector2 knockbackDirection = ((Vector2)rb.position - (Vector2)collision.transform.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }
}

    public void SetPlayer(PlayerControl player)
    {
        this.player = player;
    }

    public void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
}

