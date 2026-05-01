using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Snowball : MonoBehaviour
    {
        // Store how long a missed snowball may live before auto-cleanup.
        [SerializeField] private float lifetime = 5f;

        private void Start()
        {
            // Schedule the snowball to destroy itself after its lifetime expires.
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            // Destroy the snowball if it falls far below the play area.
            if (transform.position.y < -12f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check whether the snowball directly hit Purly.
            PurlyController purly = other.GetComponent<PurlyController>();

            // Kill Purly and remove the snowball on direct hit.
            if (purly != null)
            {
                purly.Die();
                Destroy(gameObject);
                return;
            }

            // Remove the snowball when it touches the ground.
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
