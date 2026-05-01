using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class CameraFollow : MonoBehaviour
    {
        // Store the transform the camera should follow.
        [SerializeField] private Transform target;

        // Store how quickly the camera blends toward the target position.
        [SerializeField] private float smoothSpeed = 5f;

        // Store the camera offset relative to the followed target.
        [SerializeField] private Vector3 offset = new(2f, 1f, -10f);

        // Store whether the camera should keep a fixed vertical position.
        [SerializeField] private bool lockY;

        // Store the vertical position used when Y locking is enabled.
        [SerializeField] private float fixedY;

        private void LateUpdate()
        {
            // Stop if there is no target to follow.
            if (target == null)
            {
                return;
            }

            // Start from the target position plus the desired offset.
            Vector3 desiredPosition = target.position + offset;

            // Force a fixed Y value when requested.
            if (lockY)
            {
                desiredPosition.y = fixedY;
            }

            // Smoothly move the camera toward the desired position.
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
        }
    }
}
