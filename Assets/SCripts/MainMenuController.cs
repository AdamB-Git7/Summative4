using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // Store the input used for the player's display name.
    public InputField playerNameInput;

    // Store the button that starts a fresh game.
    public Button newGameButton;

    // Store the button that resumes a paused game.
    public Button savedGameButton;

    // Store the button that opens the high-score overlay.
    public Button highScoresButton;

    // Store the button that exits the app.
    public Button exitButton;

    // Store the panel that shows the normal menu buttons.
    public GameObject buttonPanel;

    // Store the panel that shows the high-score overlay.
    public GameObject highScoresPanel;

    // Store the text element that displays the high-score list.
    public Text highScoresText;

    // Store the button that closes the high-score overlay.
    public Button closeHighScoresButton;

    // Store the build index of the gameplay scene.
    public int gameSceneBuildIndex = 1;

    private void Start()
    {
        // Hide the score overlay when the menu first opens.
        SetActive(highScoresPanel, false);

        // Show the regular button panel when the menu first opens.
        SetActive(buttonPanel, true);

        // Hook the New Game button to its handler.
        Bind(newGameButton, OnNewGame);

        // Hook the Saved Game button to its handler.
        Bind(savedGameButton, OnSavedGame);

        // Hook the High Scores button to its handler.
        Bind(highScoresButton, OnHighScores);

        // Hook the Exit button to its handler.
        Bind(exitButton, OnExit);

        // Hook the Close High Scores button to its handler.
        Bind(closeHighScoresButton, OnCloseHighScores);

        // Restore the last player name into the input field.
        if (playerNameInput != null)
        {
            playerNameInput.text = GameManager.GetPlayerName();
        }

        // Enable Saved Game only when a paused run exists.
        if (savedGameButton != null)
        {
            savedGameButton.interactable = GameManager.HasSavedGame();
        }
    }

    private void OnNewGame()
    {
        // Save the entered player name before loading the game.
        ApplyPlayerName();

        // Load the configured gameplay scene.
        SceneManager.LoadScene(gameSceneBuildIndex);
    }

    private void OnSavedGame()
    {
        // Stop if there is no paused game to resume.
        if (!GameManager.HasSavedGame())
        {
            return;
        }

        // Save the entered player name before loading the game.
        ApplyPlayerName();

        // Tell the gameplay scene to restore the paused score.
        GameManager.ResumingSavedGame = true;

        // Load the configured gameplay scene.
        SceneManager.LoadScene(gameSceneBuildIndex);
    }

    private void OnHighScores()
    {
        // Stop if the overlay panel was not assigned.
        if (highScoresPanel == null)
        {
            return;
        }

        // Load the latest saved scores.
        List<ScoreEntry> entries = ScoreUtility.LoadTopScores();

        // Write the score list into the overlay text if it exists.
        if (highScoresText != null)
        {
            highScoresText.text = ScoreUtility.BuildHighScoreDisplayText(entries);
        }

        // Hide the normal button panel.
        SetActive(buttonPanel, false);

        // Show the score overlay panel.
        highScoresPanel.SetActive(true);
    }

    private void OnCloseHighScores()
    {
        // Hide the score overlay.
        SetActive(highScoresPanel, false);

        // Show the normal button panel again.
        SetActive(buttonPanel, true);
    }

    private void OnExit()
    {
        // Quit the built application.
        Application.Quit();

#if UNITY_EDITOR
        // Stop Play Mode when running inside the Unity Editor.
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ApplyPlayerName()
    {
        // Stop if there is no player-name input field.
        if (playerNameInput == null)
        {
            return;
        }

        // Save the current input value into the shared game state.
        GameManager.SetPlayerName(playerNameInput.text);
    }

    private static void Bind(Button button, UnityEngine.Events.UnityAction action)
    {
        // Stop if the button reference is missing.
        if (button == null)
        {
            return;
        }

        // Add the click callback to the button.
        button.onClick.AddListener(action);
    }

    private static void SetActive(GameObject target, bool isActive)
    {
        // Stop if the object reference is missing.
        if (target == null)
        {
            return;
        }

        // Apply the requested active state.
        target.SetActive(isActive);
    }
}
