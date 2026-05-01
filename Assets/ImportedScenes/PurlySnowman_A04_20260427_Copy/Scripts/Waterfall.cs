using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class Waterfall : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Find Purly on the collider that entered the waterfall.
            PurlyController purly = other.GetComponent<PurlyController>();

            // Stop if the entering object is not Purly.
            if (purly == null)
            {
                return;
            }

            // Kill Purly when he touches the waterfall.
            purly.Die();
        }
    }
}
