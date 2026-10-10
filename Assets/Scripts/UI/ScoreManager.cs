using UnityEngine;

namespace IO.Game.UI
{
    /// <summary>Tracks the score for the current run.</summary>
    public class ScoreManager : MonoBehaviour
    {
        public int Score { get; private set; }

        public void AddScore(int amount = 1)
        {
            if (amount > 0)
            {
                Score += amount;
            }
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}
