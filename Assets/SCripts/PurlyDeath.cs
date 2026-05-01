using UnityEngine;

public class PurlyDeath : MonoBehaviour
{
    private void OnEnable()
    {
        // Stop if the game manager does not exist yet.
        if (GameManager.Instance == null)
        {
            return;
        }

        // Subscribe to the game-over event while this object is active.
        GameManager.Instance.OnGameOver += HandleDeath;
    }

    private void OnDisable()
    {
        // Stop if the game manager does not exist anymore.
        if (GameManager.Instance == null)
        {
            return;
        }

        // Unsubscribe from the game-over event while this object is inactive.
        GameManager.Instance.OnGameOver -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Hide the player object instead of destroying it.
        gameObject.SetActive(false);
    }
}
