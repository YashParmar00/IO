using UnityEngine;
using IO.Game.Core;
using IO.Game.UI;

namespace IO.Game.Gameplay
{
    /// <summary>Coordinates worm rules: ownership, score, arena exit, and head/body collisions.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WormBody))]
    public sealed class WormController : MonoBehaviour
    {
        [SerializeField] private WormBody wormBody;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private ArenaBounds arenaBounds;

        public int SegmentCount => wormBody != null ? wormBody.SegmentCount : 0;
        public bool IsPlayerControlled { get; private set; } = true;

        private void Awake()
        {
            if (wormBody == null)
                wormBody = GetComponent<WormBody>();
            if (scoreManager == null)
                scoreManager = FindFirstObjectByType<ScoreManager>();
            if (arenaBounds == null)
                arenaBounds = FindFirstObjectByType<ArenaBounds>();
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanMove)
                return;

            if (arenaBounds != null && !arenaBounds.Contains(transform.position))
                HandleArenaExit();
        }

        public void SetPlayerControlled(bool isPlayer)
        {
            IsPlayerControlled = isPlayer;
        }

        public void EatFood(int points, int growth)
        {
            if (IsPlayerControlled)
                scoreManager?.AddScore(points);

            wormBody?.Grow(growth);
        }

        public void HandleCollisionWith(WormController other)
        {
            if (other == null || other == this)
                return;

            if (IsPlayerControlled)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.EndRun();
            }
            else
            {
                RemoveBotFromArena();
            }
        }

        private void HandleArenaExit()
        {
            if (IsPlayerControlled)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.EndRun();
            }
            else
            {
                RemoveBotFromArena();
            }
        }

        private void RemoveBotFromArena()
        {
            wormBody?.HideSegments();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            WormController otherHead = other.GetComponentInParent<WormController>();
            if (otherHead != null && otherHead != this)
            {
                HandleCollisionWith(otherHead);
                return;
            }

            WormSegment otherSegment = other.GetComponentInParent<WormSegment>();
            if (otherSegment != null && otherSegment.Owner != this)
                HandleCollisionWith(otherSegment.Owner);
        }
    }
}
