using UnityEngine;

namespace Project.Scripts.Utilities
{
    public enum FourDirections
    {
        Up,
        Right,
        Down,
        Left,
    }
    public static class GeneralUtilities
    {
        public static readonly Vector2Int[] NeighbourDirections4 =
            { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        public static Vector3 ScreenToWorld(Camera cam, Vector3 position)
        {
            position.z = cam.nearClipPlane;
            return cam.ScreenToWorldPoint(position);
        }
        
    }
}