using UnityEngine;

namespace IO.Game.Gameplay
{
    /// <summary>Detects trigger contacts on a worm head and forwards them to that worm's rules component.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WormController))]
    public sealed class WormCollisionHandler : MonoBehaviour
    {
        [SerializeField] private WormController owner;

        private void Awake()
        {
            if (owner == null)
                owner = GetComponent<WormController>();

            if (owner == null)
                Debug.LogError("WormCollisionHandler requires a WormController reference.", this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            owner?.HandleTriggerContact(other);
        }
    }
}
