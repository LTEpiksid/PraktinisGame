using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    public SpriteRenderer characterRenderer;
    public SpriteRenderer weaponRenderer;
    private PlayerControl playerControl; // Reference to the PlayerControl script
	private Rigidbody2D playerRb; // Reference to the player's Rigidbody2D
	
	
    public Vector2 PointerPosition { get; set; }

    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;

    public bool IsAttacking { get; private set; }

    public Transform circleOrigin;
    public float radius;

    private int killCount;
    public float knockbackForce = 5f; // Adjust the knockback force as desired

	private void Start()
	{

    playerRb = transform.root.GetComponent<Rigidbody2D>(); // Get the player's Rigidbody2D
	}

    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    private void Awake()
    {
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();
        killCount = 0;
    }

    private void Update()
    {
        if (IsAttacking)
            return;

        if (characterRenderer == null)
        {
            Debug.LogWarning("Character renderer is not assigned to WeaponParent.");
            return;
        }

        if (weaponRenderer == null)
        {
            Debug.LogWarning("Weapon renderer is not assigned to WeaponParent.");
            return;
        }

        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
        Vector2 scale = transform.localScale;

        // Flip the Y scale based on the X direction
        if (direction.x < 0)
        {
            scale.y = Mathf.Abs(scale.y) * -1f;
        }
        else if (direction.x > 0)
        {
            scale.y = Mathf.Abs(scale.y);
        }

        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }

    public void Attack()
    {
        if (attackBlocked)
            return;

        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;

        DetectColliders();

        killCount++;

        if (killCount == 10)
        {
            DealExtraDamage();
        }
        else if (killCount == 30)
        {
            DealExtraDamage();
        }
    }

    private void DealExtraDamage()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            Health health;
            if (health = collider.GetComponent<Health>())
            {
                health.GetHit(1, transform.parent.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    public void DetectColliders()
{
    foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
    {
        Health health;
        if (health = collider.GetComponent<Health>())
        {
            health.GetHit(1, transform.parent.gameObject);

            // Apply knockback to the enemy
            Rigidbody2D enemyRb = collider.GetComponent<Rigidbody2D>();
            if (enemyRb != null && playerRb != null)
            {
                Vector2 knockbackDirection = (enemyRb.position - playerRb.position).normalized;
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}

}

