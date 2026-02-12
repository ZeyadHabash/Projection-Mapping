using UnityEngine;

namespace _Sandbox.Scripts.IdentityCore
{
    public class CoreMovement : MonoBehaviour
    {
        private float x;
        private float y;

        [SerializeField] private float yOffset = 15f;
        private const float UnitsRatio = 10f;

        public void SetRawX(float rawX) {
            x = rawX * UnitsRatio;
            UpdatePosition();
        }

        public void SetRawY(float rawY) {
            y = rawY * UnitsRatio + yOffset;
            UpdatePosition();
        }


        private void UpdatePosition() {
            transform.position = new Vector3(x, y, 0);
        }
    }
}