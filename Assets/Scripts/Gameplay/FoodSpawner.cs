using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using IO.Game.Config;
using IO.Game.Core;

namespace IO.Game.Gameplay
{
    /// <summary>Owns food pooling, placement, refill, and nearest-food queries.</summary>
    [DisallowMultipleComponent]
    public sealed class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ArenaBounds arenaBounds;
        [SerializeField] private FoodItem foodPrefab;
        [SerializeField, Min(1)] private int targetFoodCount = 90;
        [SerializeField, Min(1)] private int maxPoolSize = 160;
        [SerializeField] private Vector2 arenaSize = new Vector2(32f, 20f);
        [SerializeField] private Vector2Int scoreRange = new Vector2Int(1, 4);

        private ObjectPool<FoodItem> pool;
        private readonly HashSet<FoodItem> activeFoods = new HashSet<FoodItem>();

        public int ActiveFoodCount => activeFoods.Count;
        public int GrowthPerFood => gameConfig != null ? gameConfig.GrowthPerFood : 1;
        public Vector2 ArenaSize => arenaBounds != null ? arenaBounds.Size : arenaSize;
        public Vector2 ArenaCenter => arenaBounds != null ? arenaBounds.Center : (Vector2)transform.position;

        private void Awake()
        {
            if (arenaBounds == null)
                arenaBounds = GetComponent<ArenaBounds>();

            if (gameConfig != null)
            {
                targetFoodCount = gameConfig.TargetFoodCount;
                maxPoolSize = gameConfig.MaximumPooledFood;
                arenaSize = gameConfig.ArenaSize;
                scoreRange = new Vector2Int(gameConfig.MinimumFoodScore, gameConfig.MaximumFoodScore);
            }

            targetFoodCount = Mathf.Max(1, targetFoodCount);
            maxPoolSize = Mathf.Max(targetFoodCount, maxPoolSize);

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
                maxPoolSize);
        }

        private void Start()
        {
            for (int i = 0; i < Mathf.Min(targetFoodCount, maxPoolSize); i++)
                SpawnFood();
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanMove)
                return;

            int missing = targetFoodCount - activeFoods.Count;
            int spawnThisFrame = Mathf.Min(Mathf.Max(0, missing), 3);
            for (int i = 0; i < spawnThisFrame; i++)
                SpawnFood();
        }

        public bool IsInsideArena(Vector3 position)
        {
            if (arenaBounds != null)
                return arenaBounds.Contains(position);

            Vector2 offset = (Vector2)position - ArenaCenter;
            return Mathf.Abs(offset.x) <= arenaSize.x * 0.5f
                && Mathf.Abs(offset.y) <= arenaSize.y * 0.5f;
        }

        public Vector2 GetRandomPointInArena(float margin = 0.5f)
        {
            if (arenaBounds != null)
                return arenaBounds.RandomPointInside(margin);

            float halfWidth = Mathf.Max(0f, arenaSize.x * 0.5f - margin);
            float halfHeight = Mathf.Max(0f, arenaSize.y * 0.5f - margin);
            return ArenaCenter + new Vector2(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight));
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
            if (food == null || !activeFoods.Remove(food) || pool == null)
                return;

            pool.Release(food);
        }

        private void SpawnFood()
        {
            if (pool == null || activeFoods.Count >= maxPoolSize)
                return;

            FoodItem food = pool.Get();
            Vector2 position = GetRandomPointInArena();
            food.transform.position = new Vector3(position.x, position.y, 0f);

            int minScore = Mathf.Max(1, scoreRange.x);
            int maxScore = Mathf.Max(minScore, scoreRange.y);
            food.Configure(this, Random.Range(minScore, maxScore + 1), GrowthPerFood);
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
