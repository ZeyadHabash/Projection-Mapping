using UnityEngine;

namespace _Sandbox.Scripts.Hand
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class HandController : MonoBehaviour
    {
        private HandEffect handEffect;
        
        private float x;
        private float y;
        private float closedValue;

        private const float HandZ = -0.5f;
        private const float HandYOffset = 4.5f * 2;
        private const float UnitsRatio = 10f;

        public bool IsClosed => closedValue > 0.5f;
        public Transform HandTransform => transform;

        private void Awake()
        {
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            var col = GetComponent<Collider>();
            col.isTrigger = true;

            handEffect = GetComponent<HandEffect>();
        }

        #region OSC INPUT

        public void SetRawX(float rawX)
        {
            x = rawX * UnitsRatio;
            UpdatePosition();
        }

        public void SetRawY(float rawY)
        {
            y = rawY * UnitsRatio + HandYOffset;
            UpdatePosition();
        }

        public void SetClosed(float value)
        {
            closedValue = value;
            handEffect.SetState(closedValue);
        }

        #endregion

        #region KEYBOARD INPUT

        public void ApplyKeyboardDelta(Vector2 delta)
        {
            x += delta.x;
            y += delta.y;
            UpdatePosition();
        }

        public void SetKeyboardClosed(bool isClosed)
        {
            closedValue = isClosed ? 1f : 0f;
            handEffect.SetState(closedValue);
        }

        #endregion

        private void UpdatePosition()
        {
            transform.position = new Vector3(x, y, HandZ);
        }
        
    }
}
