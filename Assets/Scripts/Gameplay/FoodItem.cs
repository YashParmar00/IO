using UnityEngine;

namespace IO.Game.Gameplay
{
    /// <summary>Reusable food collectible. The food prefab needs a trigger Collider2D.</summary>
    [DisallowMultipleComponent]
    public sealed class FoodItem : MonoBehaviour
    {
        [SerializeField, Min(1)] private int growthAmount = 1;
        [SerializeField, Min(0.1f)] private float baseVisualScale = 0.5f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private FoodSpawner owner;
        private bool consumed;

        public int ScoreValue { get; private set; } = 1;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Configure(FoodSpawner spawner, int scoreValue)
        {
            owner = spawner;
            ScoreValue = Mathf.Max(1, scoreValue);
            consumed = false;

            float value01 = Mathf.Clamp01((ScoreValue - 1) / 3f);
            transform.localScale = Vector3.one * baseVisualScale * (1f + value01 * 0.22f);
            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(new Color(0.45f, 1f, 0.35f), new Color(1f, 0.25f, 0.2f), value01);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed)
                return;

            WormController worm = other.GetComponentInParent<WormController>();
            if (worm == null)
                return;

            consumed = true;
            worm.EatFood(ScoreValue, growthAmount);

            if (owner != null)
                owner.Release(this);
            else
                gameObject.SetActive(false);
        }
    }
}
