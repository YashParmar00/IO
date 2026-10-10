using UnityEngine;

namespace IO.Game.Core
{
    /// <summary>Coordinates the overall game state. Add to a GameObject in the main scene.</summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void EndGame()
        {
            IsGameOver = true;
            Time.timeScale = 0f;
        }

        public void RestartTime()
        {
            Time.timeScale = 1f;
            IsGameOver = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
