using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace IO.Game.Gameplay
{
    /// <summary>Maintains a bounded number of food items using Unity's built-in object pool.</summary>
    public sealed class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private FoodItem foodPrefab;
        [SerializeField, Min(1)] private int targetFoodCount = 90;
        [SerializeField, Min(1)] private int maxPoolSize = 160;
        [SerializeField] private Vector2 arenaSize = new Vector2(32f, 20f);
        [SerializeField] private Vector2Int scoreRange = new Vector2Int(1, 4);

        private ObjectPool<FoodItem> pool;
        private readonly HashSet<FoodItem> activeFoods = new HashSet<FoodItem>();

        public int ActiveFoodCount => activeFoods.Count;
        public Vector2 ArenaSize => arenaSize;
        public Vector2 ArenaCenter => transform.position;

        private void Awake()
        {
            if (foodPrefab == null)
            {
                Debug.LogError("FoodSpawner requires a FoodItem prefab.", this);
                enabled = false;
                return;
            }

            pool = new ObjectPool<FoodItem>(
                CreateFood,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPooledFood,
                true,
                Mathf.Min(targetFoodCount, maxPoolSize),
                Mathf.Max(1, maxPoolSize));
        }

        private void Start()
        {
            for (int i = 0; i < Mathf.Min(targetFoodCount, maxPoolSize); i++)
                SpawnFood();
        }

        private void Update()
        {
            int missing = targetFoodCount - activeFoods.Count;
            int spawnThisFrame = Mathf.Min(Mathf.Max(0, missing), 3);
            for (int i = 0; i < spawnThisFrame; i++)
                SpawnFood();
        }

        public FoodItem GetNearestFood(Vector3 position)
        {
            FoodItem nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (FoodItem food in activeFoods)
            {
                if (food == null || !food.gameObject.activeInHierarchy)
                    continue;

                float distance = (food.transform.position - position).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = food;
                }
            }

            return nearest;
        }

        public void Release(FoodItem food)
        {
            if (food == null || !activeFoods.Remove(food))
                return;

            pool.Release(food);
        }

        private void SpawnFood()
        {
            if (pool == null || activeFoods.Count >= maxPoolSize)
                return;

            FoodItem food = pool.Get();
            float halfWidth = Mathf.Max(0.5f, arenaSize.x * 0.5f);
            float halfHeight = Mathf.Max(0.5f, arenaSize.y * 0.5f);
            Vector3 center = transform.position;
            food.transform.position = new Vector3(
                center.x + Random.Range(-halfWidth, halfWidth),
                center.y + Random.Range(-halfHeight, halfHeight),
                0f);

            int minScore = Mathf.Max(1, scoreRange.x);
            int maxScore = Mathf.Max(minScore, scoreRange.y);
            food.Configure(this, Random.Range(minScore, maxScore + 1));
            activeFoods.Add(food);
        }

        private FoodItem CreateFood()
        {
            FoodItem food = Instantiate(foodPrefab, transform);
            food.gameObject.SetActive(false);
            return food;
        }

        private static void OnTakeFromPool(FoodItem food) => food.gameObject.SetActive(true);
        private static void OnReturnToPool(FoodItem food) => food.gameObject.SetActive(false);

        private void OnDestroyPooledFood(FoodItem food)
        {
            if (food != null)
                Destroy(food.gameObject);
        }
    }
}
