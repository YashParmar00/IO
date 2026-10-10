using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IO.Game.Core;

namespace IO.Game.UI
{
    /// <summary>Wires pause, resume, and restart UI to the current scene's GameManager.</summary>
    public sealed class GameFlowUI : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject statusPanel;
        [SerializeField] private Text statusText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            if (gameManager == null)
                gameManager = FindFirstObjectByType<GameManager>();
        }

        private void OnEnable()
        {
            if (gameManager != null)
                gameManager.StateChanged += ApplyState;

            if (pauseButton != null)
                pauseButton.onClick.AddListener(Pause);
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);
            if (restartButton != null)
                restartButton.onClick.AddListener(Restart);
        }

        private void Start()
        {
            if (gameManager != null)
                ApplyState(gameManager.State);
        }

        private void OnDisable()
        {
            if (gameManager != null)
                gameManager.StateChanged -= ApplyState;

            if (pauseButton != null)
                pauseButton.onClick.RemoveListener(Pause);
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(Resume);
            if (restartButton != null)
                restartButton.onClick.RemoveListener(Restart);
        }

        public void Pause()
        {
            gameManager?.Pause();
        }

        public void Resume()
        {
            gameManager?.Resume();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex >= 0)
                SceneManager.LoadScene(activeScene.buildIndex);
            else
                SceneManager.LoadScene(activeScene.name);
        }

        private void ApplyState(GameState state)
        {
            bool paused = state == GameState.Paused;
            bool gameOver = state == GameState.GameOver;

            if (statusPanel != null)
                statusPanel.SetActive(paused || gameOver);
            if (statusText != null)
                statusText.text = gameOver ? "Game Over" : "Paused";
            if (pauseButton != null)
                pauseButton.gameObject.SetActive(state == GameState.Playing);
            if (resumeButton != null)
                resumeButton.gameObject.SetActive(paused);
            if (restartButton != null)
                restartButton.gameObject.SetActive(paused || gameOver);
        }
    }
}
