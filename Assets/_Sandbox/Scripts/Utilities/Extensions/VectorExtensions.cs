using UnityEngine;

namespace Utilities.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3Int ToVector3Int(this Vector3 value) {
            return new Vector3Int(
                Mathf.RoundToInt(value.x),
                Mathf.RoundToInt(value.y),
                Mathf.RoundToInt(value.z)
            );
        }
        
        public static Vector3 Floor(this Vector3 value) {
            return new Vector3(
                Mathf.Floor(value.x),
                Mathf.Floor(value.y),
                Mathf.Floor(value.z)
            );
        }
        
        public static Vector3 GetDecimal(this Vector3 value) {
            return new Vector3(
                value.x - Mathf.Floor(value.x),
                value.y - Mathf.Floor(value.y),
                value.z - Mathf.Floor(value.z)
            );
        }
        
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max) {
            return new Vector3(
                Mathf.Clamp(value.x, min.x, max.x), 
                Mathf.Clamp(value.y, min.y,  max.y), 
                Mathf.Clamp(value.z, min.z,  max.z)
            );
        }
        
        public static Vector3Int ToVector3Int(this Vector2Int value)
        {
            return new Vector3Int(value.x, value.y);
        }
        
        public static Vector3 ToVector3(this Vector2Int value)
        {
            return new Vector3(value.x, value.y, 0);
        }
    }
}