using UnityEngine;
using IO.Game.Player;

namespace IO.Game.Gameplay
{
    /// <summary>Basic offline bot: seeks the nearest food and wanders when no food is available.</summary>
    [DisallowMultipleComponent]
    public sealed class WormAIController : MonoBehaviour
    {
        [SerializeField] private PlayerController movement;
        [SerializeField] private WormController worm;
        [SerializeField] private FoodSpawner foodSpawner;
        [SerializeField, Min(0.5f)] private float wanderTargetReachDistance = 1.2f;
        [SerializeField, Min(0.1f)] private float wanderRadius = 8f;

        private Vector2 wanderTarget;

        private void Awake()
        {
            if (movement == null)
                movement = GetComponent<PlayerController>();
            if (worm == null)
                worm = GetComponent<WormController>();
            if (foodSpawner == null)
                foodSpawner = FindFirstObjectByType<FoodSpawner>();

            if (movement != null)
                movement.SetPlayerInputEnabled(false);
            if (worm != null)
                worm.SetPlayerControlled(false);

            ChooseWanderTarget();
        }

        private void Update()
        {
            if (IO.Game.Core.GameManager.Instance != null && !IO.Game.Core.GameManager.Instance.CanMove)
                return;

            if (movement == null || foodSpawner == null)
                return;

            FoodItem nearestFood = foodSpawner.GetNearestFood(transform.position);
            if (nearestFood != null)
            {
                movement.SetSteeringTarget(nearestFood.transform.position);
                return;
            }

            if (Vector2.Distance(transform.position, wanderTarget) <= wanderTargetReachDistance)
                ChooseWanderTarget();

            movement.SetSteeringTarget(wanderTarget);
        }

        private void ChooseWanderTarget()
        {
            Vector2 center = foodSpawner != null ? (Vector2)foodSpawner.transform.position : Vector2.zero;
            Vector2 halfSize = foodSpawner != null ? foodSpawner.ArenaSize * 0.5f : Vector2.one * wanderRadius;
            float x = Random.Range(center.x - Mathf.Min(wanderRadius, halfSize.x), center.x + Mathf.Min(wanderRadius, halfSize.x));
            float y = Random.Range(center.y - Mathf.Min(wanderRadius, halfSize.y), center.y + Mathf.Min(wanderRadius, halfSize.y));
            wanderTarget = new Vector2(x, y);
        }
    }
}
