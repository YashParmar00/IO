using UnityEngine;
using IO.Game.Config;

namespace IO.Game.Gameplay
{
    /// <summary>Single source of truth for the playable world's rectangular bounds.</summary>
    [DisallowMultipleComponent]
    public sealed class ArenaBounds : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField, Min(4f)] private Vector2 size = new Vector2(32f, 20f);

        public Vector2 Size => size;
        public Vector2 Center => transform.position;

        private void Awake()
        {
            if (gameConfig != null)
                size = gameConfig.ArenaSize;

            size = new Vector2(Mathf.Max(4f, size.x), Mathf.Max(4f, size.y));
        }

        public bool Contains(Vector3 worldPosition, float margin = 0f)
        {
            Vector2 halfSize = size * 0.5f - Vector2.one * Mathf.Max(0f, margin);
            halfSize = new Vector2(Mathf.Max(0f, halfSize.x), Mathf.Max(0f, halfSize.y));
            Vector2 offset = (Vector2)worldPosition - Center;
            return Mathf.Abs(offset.x) <= halfSize.x && Mathf.Abs(offset.y) <= halfSize.y;
        }

        public Vector2 RandomPointInside(float margin = 0.5f)
        {
            Vector2 halfSize = size * 0.5f;
            float maxX = Mathf.Max(0f, halfSize.x - margin);
            float maxY = Mathf.Max(0f, halfSize.y - margin);
            return Center + new Vector2(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 0.1f));
        }

        private void OnValidate()
        {
            size = new Vector2(Mathf.Max(4f, size.x), Mathf.Max(4f, size.y));
        }
    }
}
