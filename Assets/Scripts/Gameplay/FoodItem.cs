using UnityEngine;

namespace IO.Game.Gameplay
{
    /// <summary>Reusable food collectible. The food prefab needs a trigger Collider2D.</summary>
    [DisallowMultipleComponent]
    public sealed class FoodItem : MonoBehaviour
    {
        [SerializeField, Min(1)] private int growthAmount = 1;

        private FoodSpawner owner;
        private bool consumed;

        public int ScoreValue { get; private set; } = 1;

        public void Configure(FoodSpawner spawner, int scoreValue)
        {
            owner = spawner;
            ScoreValue = Mathf.Max(1, scoreValue);
            consumed = false;
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
