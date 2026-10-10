using System.Collections.Generic;
using UnityEngine;
using IO.Game.Core;
using IO.Game.UI;

namespace IO.Game.Gameplay
{
    /// <summary>Tracks the head path so body segments follow smoothly and growth adds tail length.</summary>
    [DisallowMultipleComponent]
    public sealed class WormController : MonoBehaviour
    {
        [SerializeField] private Transform segmentPrefab;
        [SerializeField, Min(0)] private int startingSegments = 5;
        [SerializeField, Min(0.05f)] private float segmentSpacing = 0.32f;
        [SerializeField, Min(0.01f)] private float pathResolution = 0.06f;
        [SerializeField, Min(1)] private int maximumSegments = 500;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private FoodSpawner arenaSpawner;

        private readonly List<Transform> segments = new List<Transform>();
        private readonly List<Vector3> path = new List<Vector3>();

        public int SegmentCount => segments.Count;
        public bool IsPlayerControlled { get; private set; } = true;

        private void Awake()
        {
            if (scoreManager == null)
                scoreManager = FindFirstObjectByType<ScoreManager>();
            if (arenaSpawner == null)
                arenaSpawner = FindFirstObjectByType<FoodSpawner>();

            path.Add(transform.position);
        }

        private void Start()
        {
            SeedInitialPath();
            Grow(startingSegments);
        }

        private void SeedInitialPath()
        {
            path.Clear();
            Vector3 head = transform.position;
            Vector3 tailDirection = -transform.up;
            path.Add(head);

            float resolution = Mathf.Max(0.01f, pathResolution);
            float requiredLength = (startingSegments + 2) * segmentSpacing + resolution;
            int sampleCount = Mathf.CeilToInt(requiredLength / resolution);

            for (int i = 1; i <= sampleCount; i++)
                path.Add(head + tailDirection * (i * resolution));
        }

        private void Update()
        {
            if (arenaSpawner != null && !arenaSpawner.IsInsideArena(transform.position))
            {
                HandleArenaExit();
                return;
            }

            RecordHeadPath();
            UpdateBodySegments();
        }

        public void SetPlayerControlled(bool isPlayer)
        {
            IsPlayerControlled = isPlayer;
        }

        public void EatFood(int points, int growth)
        {
            if (IsPlayerControlled)
                scoreManager?.AddScore(points);

            Grow(growth);
        }

        public void Grow(int amount = 1)
        {
            if (segmentPrefab == null || amount <= 0)
                return;

            int countToAdd = Mathf.Min(amount, maximumSegments - segments.Count);
            for (int i = 0; i < countToAdd; i++)
            {
                Vector3 spawnPosition = GetPositionAlongPath((segments.Count + 1) * segmentSpacing);
                Transform segment = Instantiate(segmentPrefab, spawnPosition, Quaternion.identity);
                WormSegment wormSegment = segment.GetComponent<WormSegment>();
                if (wormSegment != null)
                    wormSegment.SetOwner(this);

                segments.Add(segment);
            }
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
            foreach (Transform segment in segments)
            {
                if (segment != null)
                    segment.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

        private void RecordHeadPath()
        {
            Vector3 currentPosition = transform.position;
            if (path.Count == 0)
            {
                path.Add(currentPosition);
                return;
            }

            float resolution = Mathf.Max(0.01f, pathResolution);
            if ((currentPosition - path[0]).sqrMagnitude < resolution * resolution)
                return;

            path.Insert(0, currentPosition);

            float maxHistory = (segments.Count + 2) * segmentSpacing + resolution * 2f;
            float distance = 0f;
            for (int i = 1; i < path.Count; i++)
            {
                distance += Vector3.Distance(path[i - 1], path[i]);
                if (distance > maxHistory)
                {
                    path.RemoveRange(i + 1, path.Count - i - 1);
                    break;
                }
            }
        }

        private void UpdateBodySegments()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Transform segment = segments[i];
                if (segment == null)
                    continue;

                float followDistance = (i + 1) * segmentSpacing;
                Vector3 nextPosition = GetPositionAlongPath(followDistance);
                segment.position = nextPosition;

                Vector2 forward = (Vector2)(GetPositionAlongPath(Mathf.Max(0f, followDistance - pathResolution)) - nextPosition);
                if (forward.sqrMagnitude > 0.0001f)
                    segment.up = forward.normalized;
            }
        }

        private Vector3 GetPositionAlongPath(float requestedDistance)
        {
            if (path.Count == 0)
                return transform.position;

            float remaining = Mathf.Max(0f, requestedDistance);
            for (int i = 1; i < path.Count; i++)
            {
                Vector3 newer = path[i - 1];
                Vector3 older = path[i];
                float span = Vector3.Distance(newer, older);
                if (span >= remaining && span > 0.0001f)
                    return Vector3.Lerp(newer, older, remaining / span);

                remaining -= span;
            }

            return path[path.Count - 1];
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
