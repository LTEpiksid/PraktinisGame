using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    private Vector2 movementInput;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private List<WeaponParent> weaponParents = new List<WeaponParent>();
    private InputAction pointerPosition;
    [SerializeField]
    private InputActionReference fire;
    private Vector2 pointerInput;

    private int enemiesKilled = 0; // Track the number of enemies killed

public Rigidbody2D GetPlayerRigidbody()
{
    return rb;
}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        fire.action.performed += PerformAttack;
    }

    private void OnDisable()
    {
        fire.action.performed -= PerformAttack;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        foreach (var weaponParent in weaponParents)
        {
            weaponParent.Attack();
        }
    }

    public void EnemyDeleted()
    {
        enemiesKilled++;

        if (enemiesKilled == 10 || enemiesKilled == 30)
        {
            UpdateWeaponProperties();
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get all WeaponParent components attached to weapons
        weaponParents.AddRange(GetComponentsInChildren<WeaponParent>());

        foreach (var weaponParent in weaponParents)
        {
            weaponParent.characterRenderer = spriteRenderer;
        }

        pointerPosition = new InputAction("pointerPosition", binding: "<Mouse>/position");
        pointerPosition.Enable();
    }

    private void FixedUpdate()
    {
        pointerInput = GetPointerInput();

        foreach (var weaponParent in weaponParents)
        {
            weaponParent.PointerPosition = pointerInput;
        }

        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);
            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }
            animator.SetBool("IsMoving", success);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            int count = rb.Cast(direction, hits, moveSpeed * Time.fixedDeltaTime + collisionOffset);
            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    private Vector2 GetPointerInput()
    {
        Vector2 mousePos = pointerPosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos;
    }

    private void UpdateWeaponProperties()
    {
        foreach (var weaponParent in weaponParents)
        {
            var weaponTransform = weaponParent.transform.Find("Weapon"); // Assuming the child GameObject is named "Weapon"
            if (weaponTransform != null)
            {
                Vector3 weaponScale = weaponTransform.localScale;

                if (enemiesKilled >= 10)
                {
                    Vector3 scaleFactor = new Vector3(0.3f, 0.3f, 0.3f);
                    weaponScale += scaleFactor;
                    weaponParent.radius += 0.1f; // Increase radius by 0.05f
                }

                if (enemiesKilled >= 30)
                {
                    Vector3 scaleFactor = new Vector3(0.4f, 0.4f, 0.4f);
                    weaponScale += scaleFactor;
                    weaponParent.radius += 0.15f; // Increase radius by 0.1f
                }

                weaponTransform.localScale = weaponScale;
            }

            var weaponRenderer = weaponParent.GetComponentInChildren<SpriteRenderer>();
            if (weaponRenderer != null)
            {
                if (enemiesKilled >= 10 && enemiesKilled < 30)
                {
                    weaponRenderer.color = Color.red;
                }
                else if (enemiesKilled >= 30)
                {
                    weaponRenderer.color = Color.black;
                }
            }
        }

        Debug.Log("Switched weapons after killing 10 and 30 enemies!");
    }
}

