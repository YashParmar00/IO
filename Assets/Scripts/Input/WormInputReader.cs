using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace IO.Game.Input
{
    /// <summary>Reads touch/pointer/keyboard input or a supplied bot target; never moves the GameObject.</summary>
    [DisallowMultipleComponent]
    public sealed class WormInputReader : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private bool acceptPlayerInput = true;

        private Vector2? externalTarget;

        public bool AcceptPlayerInput => acceptPlayerInput;

        private void Awake()
        {
            if (worldCamera == null)
                worldCamera = Camera.main;
        }

        public void SetPlayerInputEnabled(bool enabled)
        {
            acceptPlayerInput = enabled;
            if (enabled)
                externalTarget = null;
        }

        public void SetSteeringTarget(Vector2 worldPosition)
        {
            externalTarget = worldPosition;
        }

        public void ClearSteeringTarget()
        {
            externalTarget = null;
        }

        public bool TryGetDesiredDirection(Vector2 origin, out Vector2 direction)
        {
            direction = Vector2.zero;

            if (externalTarget.HasValue)
            {
                direction = externalTarget.Value - origin;
                return direction.sqrMagnitude > 0.0001f;
            }

            if (!acceptPlayerInput)
                return false;

            if (TryReadKeyboard(out direction))
                return true;

            if (!TryReadPointer(out Vector2 pointerPosition))
                return false;

            if (worldCamera == null)
                return false;

            Vector3 world = worldCamera.ScreenToWorldPoint(pointerPosition);
            direction = (Vector2)world - origin;
            return direction.sqrMagnitude > 0.0001f;
        }

        private static bool TryReadKeyboard(out Vector2 direction)
        {
            direction = Vector2.zero;
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return false;

            float x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
            float y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);

            direction = new Vector2(x, y);
            if (direction.sqrMagnitude <= 0.0001f)
                return false;

            direction.Normalize();
            return true;
        }

        private static bool TryReadPointer(out Vector2 screenPosition)
        {
            screenPosition = Vector2.zero;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                var touch = Touchscreen.current.primaryTouch;
                int touchId = touch.touchId.ReadValue();
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId))
                    return false;

                screenPosition = touch.position.ReadValue();
                return true;
            }

            if (Mouse.current == null)
                return false;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return false;

            screenPosition = Mouse.current.position.ReadValue();
            return true;
        }
    }
}
