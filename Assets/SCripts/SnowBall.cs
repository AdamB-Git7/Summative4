using UnityEngine;

public class Snowball : MonoBehaviour
{
    public float speed = 4f;
    private Transform target;

    void Start()
    {
        // Find Purly using the Player tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }

        // Destroy snowball after 10 seconds
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        // Follow Purly
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If it hits Purly → destroy both
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject); // destroy Purly
            Destroy(gameObject); // destroy snowball
        }
    }
}