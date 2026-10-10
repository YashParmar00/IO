using System;
using UnityEngine;
using IO.Game.Core;

namespace IO.Game.UI
{
    /// <summary>Tracks run score and a non-sensitive local high score.</summary>
    public sealed class ScoreManager : MonoBehaviour
    {
        private const string HighScoreKey = "io.game.highscore";

        public int Score { get; private set; }
        public int HighScore { get; private set; }

        public event Action<int, int> ScoreChanged;

        private GameManager gameManager;
        private bool highScoreDirty;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            ScoreChanged?.Invoke(Score, HighScore);
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            if (gameManager != null)
                gameManager.StateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            if (gameManager != null)
                gameManager.StateChanged -= HandleGameStateChanged;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveHighScoreIfNeeded();
        }

        private void OnApplicationQuit()
        {
            SaveHighScoreIfNeeded();
        }

        public void AddScore(int amount = 1)
        {
            if (amount <= 0)
                return;

            Score += amount;
            if (Score > HighScore)
            {
                HighScore = Score;
                highScoreDirty = true;
            }

            ScoreChanged?.Invoke(Score, HighScore);
        }

        public void ResetScore()
        {
            Score = 0;
            ScoreChanged?.Invoke(Score, HighScore);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
                SaveHighScoreIfNeeded();
        }

        private void SaveHighScoreIfNeeded()
        {
            if (!highScoreDirty)
                return;

            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            highScoreDirty = false;
        }
    }
}
