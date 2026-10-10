using UnityEngine;

namespace IO.Game.Gameplay
{
    /// <summary>Food spawning placeholder; spawn bounds and prefab will be configured in the scene.</summary>
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject foodPrefab;

        public GameObject FoodPrefab => foodPrefab;
    }
}
