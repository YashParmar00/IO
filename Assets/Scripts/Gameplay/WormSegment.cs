using UnityEngine;

namespace IO.Game.Gameplay
{
    /// <summary>Identifies a body segment so the colliding head can apply its own collision rule.</summary>
    [DisallowMultipleComponent]
    public sealed class WormSegment : MonoBehaviour
    {
        public WormController Owner { get; private set; }

        public void SetOwner(WormController owner)
        {
            Owner = owner;
        }
    }
}
