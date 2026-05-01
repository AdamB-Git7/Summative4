using System.Collections;
using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class SnowGun : MonoBehaviour
    {
        // Store the snowball prefab this cannon should spawn.
        [SerializeField] private GameObject snowballPrefab;

        // Store the muzzle transform used as the spawn point.
        [SerializeField] private Transform muzzle;

        // Store the minimum shot speed for random lower-half shots.
        [SerializeField] private float minLaunchSpeed = 6f;

        // Store the maximum shot speed for random lower-half shots.
        [SerializeField] private float maxLaunchSpeed = 10f;

        // Store whether the cannon should fire on its own.
        [SerializeField] private bool autoFire = true;

        // Store the delay between automatic shots.
        [SerializeField] private float fireInterval = 5f;

        private void Start()
        {
            // Start the automatic firing loop when auto-fire is enabled.
            if (autoFire)
            {
                StartCoroutine(FireLoop());
            }
        }

        public void Configure(GameObject prefab, Transform muzzleTransform, bool shouldAutoFire = true)
        {
            // Save the prefab assigned by runtime setup code.
            snowballPrefab = prefab;

            // Save the muzzle assigned by runtime setup code.
            muzzle = muzzleTransform;

            // Save the requested auto-fire mode.
            autoFire = shouldAutoFire;
        }

        private IEnumerator FireLoop()
        {
            // Keep firing forever until this object is destroyed.
            while (true)
            {
                // Wait the configured amount of time between shots.
                yield return new WaitForSeconds(fireInterval);

                // Fire one snowball.
                Fire();
            }
        }

        public void Fire()
        {
            // Stop if the prefab was not assigned.
            if (snowballPrefab == null)
            {
                Debug.LogError($"{name}: snowballPrefab is not assigned.");
                return;
            }

            // Stop if the muzzle was not assigned.
            if (muzzle == null)
            {
                Debug.LogError($"{name}: muzzle is not assigned.");
                return;
            }

            // Spawn one snowball at the muzzle position.
            GameObject snowball = Instantiate(snowballPrefab, muzzle.position, Quaternion.identity);

            // Try to get the spawned snowball's Rigidbody2D.
            Rigidbody2D body = snowball.GetComponent<Rigidbody2D>();

            // Stop if the spawned object has no rigidbody.
            if (body == null)
            {
                return;
            }

            // Pick a random angle in the lower 180 degrees only.
            float angle = Random.Range(180f, 360f) * Mathf.Deg2Rad;

            // Convert that angle into a unit direction vector.
            Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));

            // Pick a random speed inside the configured range.
            float speed = Random.Range(minLaunchSpeed, maxLaunchSpeed);

            // Launch the snowball with the chosen direction and speed.
            body.linearVelocity = direction.normalized * speed;
        }

        private void OnDrawGizmosSelected()
        {
            // Stop if there is no muzzle to preview from.
            if (muzzle == null)
            {
                return;
            }

            // Draw the allowed lower-half firing arc guide in cyan.
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(muzzle.position, muzzle.position + new Vector3(-2f, 0f, 0f));
            Gizmos.DrawLine(muzzle.position, muzzle.position + new Vector3(2f, 0f, 0f));
            Gizmos.DrawLine(muzzle.position, muzzle.position + Vector3.down * 2f);
        }
    }
}
