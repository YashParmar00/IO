using UnityEngine;

namespace IO.Game.Config
{
    /// <summary>Central tuning asset for the first playable. Keep balance values out of gameplay code.</summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "IO/Game/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Runtime")]
        [SerializeField, Range(30, 120)] private int targetFrameRate = 60;

        [Header("Worm Movement")]
        [SerializeField, Min(0.1f)] private float playerMoveSpeed = 4.5f;
        [SerializeField, Min(1f)] private float turnSpeedDegrees = 240f;
        [SerializeField, Range(0.25f, 1.5f)] private float botSpeedMultiplier = 0.9f;
        [SerializeField, Min(0.01f)] private float segmentSpacing = 0.38f;
        [SerializeField, Min(0.01f)] private float pathResolution = 0.06f;
        [SerializeField, Min(1)] private int maximumSegments = 500;

        [Header("Arena")]
        [SerializeField] private Vector2 arenaSize = new Vector2(32f, 20f);

        [Header("Food")]
        [SerializeField, Min(1)] private int targetFoodCount = 90;
        [SerializeField, Min(1)] private int maximumPooledFood = 160;
        [SerializeField, Min(1)] private int minimumFoodScore = 1;
        [SerializeField, Min(1)] private int maximumFoodScore = 4;
        [SerializeField, Min(1)] private int growthPerFood = 1;

        [Header("Bots")]
        [SerializeField, Min(0.05f)] private float botDecisionInterval = 0.2f;
        [SerializeField, Min(0.5f)] private float botWanderRadius = 8f;

        public int TargetFrameRate => targetFrameRate;
        public float PlayerMoveSpeed => playerMoveSpeed;
        public float TurnSpeedDegrees => turnSpeedDegrees;
        public float BotSpeedMultiplier => botSpeedMultiplier;
        public float SegmentSpacing => segmentSpacing;
        public float PathResolution => pathResolution;
        public int MaximumSegments => maximumSegments;
        public Vector2 ArenaSize => new Vector2(Mathf.Max(4f, arenaSize.x), Mathf.Max(4f, arenaSize.y));
        public int TargetFoodCount => targetFoodCount;
        public int MaximumPooledFood => Mathf.Max(targetFoodCount, maximumPooledFood);
        public int MinimumFoodScore => Mathf.Min(minimumFoodScore, maximumFoodScore);
        public int MaximumFoodScore => Mathf.Max(minimumFoodScore, maximumFoodScore);
        public int GrowthPerFood => growthPerFood;
        public float BotDecisionInterval => botDecisionInterval;
        public float BotWanderRadius => botWanderRadius;

        private void OnValidate()
        {
            targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 120);
            arenaSize = new Vector2(Mathf.Max(4f, arenaSize.x), Mathf.Max(4f, arenaSize.y));
            maximumFoodScore = Mathf.Max(minimumFoodScore, maximumFoodScore);
            maximumPooledFood = Mathf.Max(targetFoodCount, maximumPooledFood);
        }
    }
}
