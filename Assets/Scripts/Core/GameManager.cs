using System;
using UnityEngine;

namespace IO.Game.Core
{
    public enum GameState
    {
        Ready,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>Owns the current run state. Place one instance in the game scene.</summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.Playing;
        public bool CanMove => State == GameState.Playing;
        public event Action<GameState> StateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Time.timeScale = 1f;
            Application.targetFrameRate = 60;
        }

        public void StartRun() => SetState(GameState.Playing);

        public void Pause()
        {
            if (State == GameState.Playing)
                SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (State == GameState.Paused)
                SetState(GameState.Playing);
        }

        public void EndRun()
        {
            if (State != GameState.GameOver)
                SetState(GameState.GameOver);
        }

        public void ResetRun() => SetState(GameState.Playing);

        private void SetState(GameState nextState)
        {
            if (State == nextState)
                return;

            State = nextState;
            Time.timeScale = nextState == GameState.Paused || nextState == GameState.GameOver ? 0f : 1f;
            StateChanged?.Invoke(State);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                Time.timeScale = 1f;
            }
        }
    }
}
