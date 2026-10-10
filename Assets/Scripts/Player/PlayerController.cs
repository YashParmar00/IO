using UnityEngine;
using IO.Game.Config;
using IO.Game.Core;
using IO.Game.Input;

namespace IO.Game.Player
{
    /// <summary>Owns worm-head steering and movement. Input collection lives in WormInputReader.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(WormInputReader))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private WormInputReader inputReader;
        [SerializeField, Min(0.1f)] private float moveSpeed = 4.5f;
        [SerializeField, Min(1f)] private float turnSpeedDegrees = 240f;

        private Rigidbody2D body;
        private Vector2 heading = Vector2.up;

        public float MoveSpeed => moveSpeed;
        public Vector2 Heading => heading;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            if (inputReader == null)
                inputReader = GetComponent<WormInputReader>();

            if (gameConfig != null)
            {
                moveSpeed = gameConfig.PlayerMoveSpeed;
                turnSpeedDegrees = gameConfig.TurnSpeedDegrees;
            }

            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Kinematic;
                body.gravityScale = 0f;
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            Vector2 initialHeading = transform.up;
            if (initialHeading.sqrMagnitude > 0.001f)
                heading = initialHeading.normalized;
        }

        private void Update()
        {
            if (!CanSimulate || inputReader == null)
                return;

            if (!inputReader.TryGetDesiredDirection(transform.position, out Vector2 desired))
                return;

            if (desired.sqrMagnitude <= 0.001f)
                return;

            float currentAngle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
            float desiredAngle = Mathf.Atan2(desired.y, desired.x) * Mathf.Rad2Deg;
            float nextAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, turnSpeedDegrees * Time.deltaTime);
            float radians = nextAngle * Mathf.Deg2Rad;
            heading = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }

        private void FixedUpdate()
        {
            if (!CanSimulate || body == null)
                return;

            Vector2 nextPosition = body.position + heading * moveSpeed * Time.fixedDeltaTime;
            float rotationZ = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg - 90f;
            body.MoveRotation(rotationZ);
            body.MovePosition(nextPosition);
        }

        public void SetSteeringTarget(Vector2 worldPosition)
        {
            if (inputReader != null)
                inputReader.SetSteeringTarget(worldPosition);
        }

        public void ClearSteeringTarget()
        {
            if (inputReader != null)
                inputReader.ClearSteeringTarget();
        }

        public void SetPlayerInputEnabled(bool enabled)
        {
            if (inputReader != null)
                inputReader.SetPlayerInputEnabled(enabled);
        }

        private static bool CanSimulate =>
            GameManager.Instance == null || GameManager.Instance.CanMove;
    }
}
