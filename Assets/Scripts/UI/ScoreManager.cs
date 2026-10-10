using System;
using UnityEngine;

namespace IO.Game.UI
{
    /// <summary>Tracks run score and a non-sensitive local high score.</summary>
    public sealed class ScoreManager : MonoBehaviour
    {
        private const string HighScoreKey = "io.game.highscore";

        public int Score { get; private set; }
        public int HighScore { get; private set; }

        public event Action<int, int> ScoreChanged;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            ScoreChanged?.Invoke(Score, HighScore);
        }

        public void AddScore(int amount = 1)
        {
            if (amount <= 0)
                return;

            Score += amount;
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
                PlayerPrefs.Save();
            }

            ScoreChanged?.Invoke(Score, HighScore);
        }

        public void ResetScore()
        {
            Score = 0;
            ScoreChanged?.Invoke(Score, HighScore);
        }
    }
}
