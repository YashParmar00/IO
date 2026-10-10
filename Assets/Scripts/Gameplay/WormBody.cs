using System.Collections.Generic;
using UnityEngine;
using IO.Game.Config;
using IO.Game.Core;

namespace IO.Game.Gameplay
{
    /// <summary>Owns the body's trail history, segment creation, and segment positions.</summary>
    [DisallowMultipleComponent]
    public sealed class WormBody : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Transform segmentPrefab;
        [SerializeField, Min(0)] private int startingSegments = 5;
        [SerializeField, Min(0.05f)] private float segmentSpacing = 0.38f;
        [SerializeField, Min(0.01f)] private float pathResolution = 0.06f;
        [SerializeField, Min(1)] private int maximumSegments = 500;

        private readonly List<Transform> segments = new List<Transform>();
        private readonly List<Vector3> path = new List<Vector3>();

        public int SegmentCount => segments.Count;

        private void Awake()
        {
            if (segmentPrefab == null)
                Debug.LogError("WormBody requires a BodySegment prefab reference.", this);

            if (gameConfig != null)
            {
                segmentSpacing = gameConfig.SegmentSpacing;
                pathResolution = gameConfig.PathResolution;
                maximumSegments = gameConfig.MaximumSegments;
            }

            path.Add(transform.position);
        }

        private void Start()
        {
            SeedInitialPath();
            Grow(startingSegments);
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanMove)
                return;

            RecordHeadPath();
            UpdateBodySegments();
        }

        public void Grow(int amount)
        {
            if (segmentPrefab == null || amount <= 0)
                return;

            int countToAdd = Mathf.Min(amount, Mathf.Max(0, maximumSegments - segments.Count));
            WormController owner = GetComponent<WormController>();

            for (int i = 0; i < countToAdd; i++)
            {
                Vector3 spawnPosition = GetPositionAlongPath((segments.Count + 1) * segmentSpacing);
                Transform segment = Instantiate(segmentPrefab, spawnPosition, Quaternion.identity);
                WormSegment marker = segment.GetComponent<WormSegment>();
                if (marker != null)
                    marker.SetOwner(owner);

                segments.Add(segment);
            }
        }

        public void HideSegments()
        {
            foreach (Transform segment in segments)
            {
                if (segment != null)
                    segment.gameObject.SetActive(false);
            }
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
            if (segments.Count == 0 || path.Count < 2)
                return;

            int segmentIndex = 0;
            float distanceBeforeEdge = 0f;

            // Traverse trail once per frame: amortized cost follows path length plus body length.
            for (int pathIndex = 1; pathIndex < path.Count && segmentIndex < segments.Count; pathIndex++)
            {
                Vector3 newer = path[pathIndex - 1];
                Vector3 older = path[pathIndex];
                float span = Vector3.Distance(newer, older);
                if (span <= 0.0001f)
                    continue;

                float distanceAfterEdge = distanceBeforeEdge + span;
                Vector3 tailDirection = older - newer;

                while (segmentIndex < segments.Count)
                {
                    float targetDistance = (segmentIndex + 1) * segmentSpacing;
                    if (targetDistance > distanceAfterEdge)
                        break;

                    Transform segment = segments[segmentIndex];
                    if (segment != null)
                    {
                        float t = Mathf.Clamp01((targetDistance - distanceBeforeEdge) / span);
                        segment.position = Vector3.Lerp(newer, older, t);
                        if (tailDirection.sqrMagnitude > 0.0001f)
                            segment.up = tailDirection.normalized;
                    }

                    segmentIndex++;
                }

                distanceBeforeEdge = distanceAfterEdge;
            }

            Vector3 oldestPoint = path[path.Count - 1];
            for (; segmentIndex < segments.Count; segmentIndex++)
            {
                if (segments[segmentIndex] != null)
                    segments[segmentIndex].position = oldestPoint;
            }
        }

        private Vector3 GetPositionAlongPath(float requestedDistance)
        {
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

            return path.Count > 0 ? path[path.Count - 1] : transform.position;
        }
    }
}
