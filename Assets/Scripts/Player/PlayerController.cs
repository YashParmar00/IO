using UnityEngine;
using UnityEngine.InputSystem;
using IO.Game.Core;

namespace IO.Game.Player
{
    /// <summary>
    /// Steers the worm head toward the pointer/touch position. Keyboard arrows or WASD also work in the Editor.
    /// Orient the head sprite so its forward direction points up (positive Y).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 4.5f;
        [SerializeField, Min(1f)] private float turnSpeedDegrees = 240f;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private bool acceptPlayerInput = true;

        private Vector2 heading = Vector2.up;
        private Vector2? externalTarget;
        private Rigidbody2D body;

        public float MoveSpeed => moveSpeed;
        public Vector2 Heading => heading;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();

            if (worldCamera == null)
                worldCamera = Camera.main;

            Vector2 initialHeading = transform.up;
            if (initialHeading.sqrMagnitude > 0.001f)
                heading = initialHeading.normalized;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanMove)
                return;

            if (TryGetDesiredDirection(out Vector2 desired) && desired.sqrMagnitude > 0.001f)
            {
                float currentAngle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
                float desiredAngle = Mathf.Atan2(desired.y, desired.x) * Mathf.Rad2Deg;
                float nextAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, turnSpeedDegrees * Time.deltaTime);
                float radians = nextAngle * Mathf.Deg2Rad;
                heading = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            }

            transform.up = heading;
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanMove)
                return;

            Vector2 nextPosition = (Vector2)transform.position + heading * moveSpeed * Time.fixedDeltaTime;
            if (body != null)
                body.MovePosition(nextPosition);
            else
                transform.position = nextPosition;
        }

        public void SetSteeringTarget(Vector2 worldPosition) => externalTarget = worldPosition;
        public void ClearSteeringTarget() => externalTarget = null;
        public void SetPlayerInputEnabled(bool enabled) => acceptPlayerInput = enabled;

        private bool TryGetDesiredDirection(out Vector2 direction)
        {
            direction = Vector2.zero;

            if (externalTarget.HasValue)
            {
                direction = externalTarget.Value - (Vector2)transform.position;
                return true;
            }

            if (!acceptPlayerInput)
                return false;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                float x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                        - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
                float y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                        - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);

                Vector2 keyboardDirection = new Vector2(x, y);
                if (keyboardDirection.sqrMagnitude > 0.001f)
                {
                    direction = keyboardDirection.normalized;
                    return true;
                }
            }

            Vector2? screenPosition = null;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            else if (Mouse.current != null)
                screenPosition = Mouse.current.position.ReadValue();

            if (!screenPosition.HasValue || worldCamera == null)
                return false;

            Vector3 world = worldCamera.ScreenToWorldPoint(screenPosition.Value);
            direction = (Vector2)world - (Vector2)transform.position;
            return true;
        }
    }
}
