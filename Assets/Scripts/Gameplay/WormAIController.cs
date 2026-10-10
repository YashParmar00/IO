using UnityEngine;
using IO.Game.Config;
using IO.Game.Core;
using IO.Game.Player;

namespace IO.Game.Gameplay
{
    /// <summary>Basic offline bot that chooses a food target on an interval and otherwise wanders.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(WormController))]
    public sealed class WormAIController : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private PlayerController movement;
        [SerializeField] private WormController worm;
        [SerializeField] private FoodSpawner foodSpawner;
        [SerializeField, Min(0.05f)] private float decisionInterval = 0.2f;
        [SerializeField, Min(0.5f)] private float wanderTargetReachDistance = 1.2f;
        [SerializeField, Min(0.1f)] private float wanderRadius = 8f;

        private Vector2 wanderTarget;
        private float nextDecisionTime;

        private void Awake()
        {
            if (movement == null)
                movement = GetComponent<PlayerController>();
            if (worm == null)
                worm = GetComponent<WormController>();
            if (foodSpawner == null)
                foodSpawner = FindFirstObjectByType<FoodSpawner>();

            if (gameConfig != null)
            {
                decisionInterval = gameConfig.BotDecisionInterval;
                wanderRadius = gameConfig.BotWanderRadius;
            }

            if (movement != null)
                movement.SetPlayerInputEnabled(false);
            if (worm != null)
                worm.SetPlayerControlled(false);

            ChooseWanderTarget();
            nextDecisionTime = 0f;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanMove)
                return;

            if (movement == null || foodSpawner == null || Time.time < nextDecisionTime)
                return;

            nextDecisionTime = Time.time + Mathf.Max(0.05f, decisionInterval);
            FoodItem nearestFood = foodSpawner.GetNearestFood(transform.position);
            if (nearestFood != null)
            {
                movement.SetSteeringTarget(nearestFood.transform.position);
                return;
            }

            if (Vector2.Distance(transform.position, wanderTarget) <= wanderTargetReachDistance
                || !foodSpawner.IsInsideArena(wanderTarget))
                ChooseWanderTarget();

            movement.SetSteeringTarget(wanderTarget);
        }

        private void ChooseWanderTarget()
        {
            if (foodSpawner != null)
            {
                wanderTarget = foodSpawner.GetRandomPointInArena();
                return;
            }

            wanderTarget = (Vector2)transform.position
                + Random.insideUnitCircle * Mathf.Max(1f, wanderRadius);
        }
    }
}
