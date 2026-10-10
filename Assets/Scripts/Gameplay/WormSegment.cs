using UnityEngine;

namespace IO.Game.Gameplay
{
    /// <summary>Identifies a body segment so its owner can ignore its own body during collision checks.</summary>
    [DisallowMultipleComponent]
    public sealed class WormSegment : MonoBehaviour
    {
        public WormController Owner { get; private set; }

        public void SetOwner(WormController owner)
        {
            Owner = owner;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Owner == null)
                return;

            WormController otherHead = other.GetComponentInParent<WormController>();
            if (otherHead != null && otherHead != Owner)
            {
                Owner.HandleCollisionWith(otherHead);
                return;
            }

            WormSegment otherSegment = other.GetComponentInParent<WormSegment>();
            if (otherSegment != null && otherSegment.Owner != null && otherSegment.Owner != Owner)
                Owner.HandleCollisionWith(otherSegment.Owner);
        }
    }
}
