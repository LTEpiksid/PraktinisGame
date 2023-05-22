using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent1 : MonoBehaviour
{
    public SpriteRenderer characterRenderer;
    public SpriteRenderer weaponRenderer1;

    public Vector2 PointerPosition { get; set; }

    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;

    public bool IsAttacking { get; private set; }

    public Transform circleOrigin;
    public float radius;

    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    private void Awake()
    {
        weaponRenderer1 = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (IsAttacking)
            return;

        if (characterRenderer == null)
        {
            Debug.LogWarning("Character renderer is not assigned to WeaponParent1.");
            return;
        }

        if (weaponRenderer1 == null)
        {
            Debug.LogWarning("Weapon renderer is not assigned to WeaponParent1.");
            return;
        }

        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -0.5f;
        }
        else if (direction.x > 0)
        {
            scale.y = 0.5f;
        }
        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer1.sortingOrder = characterRenderer.sortingOrder - 1;
        }
        else
        {
            weaponRenderer1.sortingOrder = characterRenderer.sortingOrder + 1;
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
            //Debug.Log(collider.name);
            Health health;
            if (health = collider.GetComponent<Health>())
            {
                health.GetHit(1, transform.parent.gameObject);
            }
        }
    }
}

