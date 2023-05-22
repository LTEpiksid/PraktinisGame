using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    private float timer;

    private void Start()
    {
        timer = lifetime;
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Decrease the timer
        timer -= Time.deltaTime;

        // Destroy the bullet after the lifetime has passed
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for collisions with enemies or other objects
        // and perform any desired actions or damage calculations.
        // You can access the collided object through the 'collision' parameter.

        // Example: Destroy the bullet when it hits an enemy
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}

