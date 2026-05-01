using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class Balloon : MonoBehaviour
    {
        // Store how many points this balloon gives when Purly pops it.
        [SerializeField] private int scoreValue = 1;

        // Store the optional visual effect prefab to spawn on pop.
        [SerializeField] private GameObject popEffectPrefab;

        // Store the optional sound effect clip to play on pop.
        [SerializeField] private AudioClip popSfx;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore anything that is not Purly.
            if (other.GetComponent<PurlyController>() == null)
            {
                return;
            }

            // Add this balloon's score to the active game if a manager exists.
            GameManager.Instance?.AddScore(scoreValue);

            // Spawn the pop effect if one was assigned in the Inspector.
            if (popEffectPrefab != null)
            {
                Instantiate(popEffectPrefab, transform.position, Quaternion.identity);
            }

            // Play the pop sound at the balloon's position if one was assigned.
            if (popSfx != null)
            {
                AudioSource.PlayClipAtPoint(popSfx, transform.position);
            }

            // Remove the balloon after it has been popped.
            Destroy(gameObject);
        }
    }
}
