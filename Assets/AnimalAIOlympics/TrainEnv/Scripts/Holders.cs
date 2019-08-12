using UnityEngine;

namespace Holders
{
    class PositionRotation
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public PositionRotation(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}