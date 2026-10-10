using UnityEngine;
using UnityEngine.UI;

namespace IO.Game.UI
{
    /// <summary>Optional uGUI binding for current and best score text labels.</summary>
    public sealed class ScoreHUD : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private Text currentScoreText;
        [SerializeField] private Text highScoreText;

        private void OnEnable()
        {
            if (scoreManager == null)
                scoreManager = FindFirstObjectByType<ScoreManager>();

            if (scoreManager != null)
                scoreManager.ScoreChanged += Refresh;

            Refresh(scoreManager != null ? scoreManager.Score : 0,
                    scoreManager != null ? scoreManager.HighScore : 0);
        }

        private void OnDisable()
        {
            if (scoreManager != null)
                scoreManager.ScoreChanged -= Refresh;
        }

        private void Refresh(int score, int highScore)
        {
            if (currentScoreText != null)
                currentScoreText.text = "Score: " + score;

            if (highScoreText != null)
                highScoreText.text = "Best: " + highScore;
        }
    }
}
