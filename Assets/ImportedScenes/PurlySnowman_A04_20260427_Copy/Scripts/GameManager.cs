using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class GameManager : MonoBehaviour
    {
        // Store the active imported-scene game manager instance.
        public static GameManager Instance { get; private set; }

        // Store the score label shown during gameplay.
        [SerializeField] private TextMeshProUGUI scoreText;

        // Store the pause overlay panel.
        [SerializeField] private GameObject pausePanel;

        // Store the game-over overlay panel.
        [SerializeField] private GameObject gameOverPanel;

        // Store the final-score text shown on game over.
        [SerializeField] private TextMeshProUGUI finalScoreText;

        // Store the high-score text shown on game over.
        [SerializeField] private TextMeshProUGUI highScoreText;

        // Store the current score.
        public int Score { get; private set; }

        // Store whether the run has already ended.
        public bool IsGameOver { get; private set; }

        // Store whether the game is currently paused.
        private bool isPaused;

        private void Awake()
        {
            // Destroy duplicate game manager instances.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Register this object as the active imported-scene manager.
            Instance = this;
        }

        private void Start()
        {
            // Reset the gameplay score.
            Score = 0;

            // Reset the game-over state.
            IsGameOver = false;

            // Ensure the scene starts unpaused.
            isPaused = false;
            Time.timeScale = 1f;

            // Refresh the HUD score text.
            UpdateScoreText();

            // Hide the pause panel on startup.
            SetActive(pausePanel, false);

            // Hide the game-over panel on startup.
            SetActive(gameOverPanel, false);
        }

        private void Update()
        {
            // Toggle pause with Escape only while the game is still active.
            if (!IsGameOver && Input.GetKeyDown(KeyCode.Escape))
            {
                OnPausePressed();
            }
        }

        public void AddScore(int amount)
        {
            // Ignore score changes after the run ends.
            if (IsGameOver)
            {
                return;
            }

            // Add the requested amount to the score.
            Score += amount;

            // Refresh the score label.
            UpdateScoreText();
        }

        public void OnPlayerDied()
        {
            // Ignore duplicate death handling.
            if (IsGameOver)
            {
                return;
            }

            // Mark the run as over.
            IsGameOver = true;

            // Save a new high score when this run beats the old one.
            int highScore = Mathf.Max(Score, PlayerPrefs.GetInt("HighScore", 0));
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            // Show the game-over panel.
            SetActive(gameOverPanel, true);

            // Write the final score if the field exists.
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {Score:00}";
            }

            // Write the saved high score if the field exists.
            if (highScoreText != null)
            {
                highScoreText.text = $"High: {highScore:00}";
            }

            // Freeze gameplay after death.
            Time.timeScale = 0f;
        }

        public void OnPausePressed()
        {
            // Ignore pause toggles after the run ends.
            if (IsGameOver)
            {
                return;
            }

            // Flip the paused state.
            isPaused = !isPaused;

            // Apply the matching time scale.
            Time.timeScale = isPaused ? 0f : 1f;

            // Show or hide the pause panel to match the paused state.
            SetActive(pausePanel, isPaused);
        }

        public void OnResumePressed()
        {
            // Clear the paused state.
            isPaused = false;

            // Resume gameplay time.
            Time.timeScale = 1f;

            // Hide the pause panel.
            SetActive(pausePanel, false);
        }

        public void OnRestartPressed()
        {
            // Ensure the reloaded scene starts unpaused.
            Time.timeScale = 1f;

            // Reload the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnExitToMenuPressed()
        {
            // Ensure the menu scene starts unpaused.
            Time.timeScale = 1f;

            // Load the build-settings scene named MainMenu.
            SceneManager.LoadScene("MainMenu");
        }

        private void UpdateScoreText()
        {
            // Stop if the score label was not assigned.
            if (scoreText == null)
            {
                return;
            }

            // Write the current score into the score label.
            scoreText.text = $"Score: {Score:00}";
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            // Stop if the target object was not assigned.
            if (target == null)
            {
                return;
            }

            // Apply the requested active state.
            target.SetActive(isActive);
        }
    }
}
