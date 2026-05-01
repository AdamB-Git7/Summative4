using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class BalloonSpawner : MonoBehaviour
    {
        // Store the prefab that will be cloned for each balloon.
        [SerializeField] private GameObject balloonPrefab;

        // Store fallback spawn points for scenes without live markers.
        [SerializeField] private Transform[] spawnPoints;

        // Store the shortest allowed delay between spawn attempts.
        [SerializeField] private float minSpawnDelay = 0.6f;

        // Store the longest allowed delay between spawn attempts.
        [SerializeField] private float maxSpawnDelay = 1.6f;

        // Store the cap used to stop too many balloons from existing at once.
        [SerializeField] private int maxActiveBalloons = 12;

        // Track all balloons that are currently alive.
        private readonly List<GameObject> alive = new();

        private void Start()
        {
            // Stop immediately if the prefab was not assigned.
            if (balloonPrefab == null)
            {
                Debug.LogError("BalloonSpawner: balloonPrefab is not assigned.");
                return;
            }

            // Start the repeating spawn coroutine.
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            // Keep spawning until this spawner is destroyed.
            while (true)
            {
                // Wait a random time before trying the next spawn.
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

                // Remove destroyed balloons from the tracking list.
                alive.RemoveAll(balloon => balloon == null);

                // Skip this spawn if we are already at the balloon cap.
                if (alive.Count >= maxActiveBalloons)
                {
                    continue;
                }

                // Find one valid spawn point.
                Transform point = PickSpawnPoint();

                // Skip this spawn if no point is available.
                if (point == null)
                {
                    continue;
                }

                // Add a small random offset so balloons do not overlap perfectly.
                Vector3 offset = new(
                    Random.Range(-0.2f, 0.2f),
                    Random.Range(0f, 0.3f),
                    0f
                );

                // Create the balloon and remember it in the alive list.
                alive.Add(Instantiate(balloonPrefab, point.position + offset, Quaternion.identity));
            }
        }

        private Transform PickSpawnPoint()
        {
            // Prefer live markers that currently exist in the scene.
            BalloonSpawnPoint[] livePoints = Object.FindObjectsByType<BalloonSpawnPoint>(FindObjectsSortMode.None);

            // Return one random live marker if any were found.
            if (livePoints.Length > 0)
            {
                return livePoints[Random.Range(0, livePoints.Length)].transform;
            }

            // Return one random fallback point if any were configured.
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                return spawnPoints[Random.Range(0, spawnPoints.Length)];
            }

            // Return nothing if there are no valid spawn locations.
            return null;
        }

        private void OnDrawGizmos()
        {
            // Stop if there are no fallback points to preview.
            if (spawnPoints == null)
            {
                return;
            }

            // Draw fallback spawn points in yellow.
            Gizmos.color = Color.yellow;

            // Visit each fallback point one by one.
            foreach (Transform point in spawnPoints)
            {
                // Skip empty array entries safely.
                if (point == null)
                {
                    continue;
                }

                // Draw a small sphere where a balloon could appear.
                Gizmos.DrawWireSphere(point.position, 0.25f);
            }
        }
    }
}
