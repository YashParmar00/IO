using UnityEngine;

namespace IO.Game.Player
{
    /// <summary>Initial player movement placeholder. Gameplay input and movement will be implemented next.</summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 5f;

        public float MoveSpeed => moveSpeed;
    }
}
