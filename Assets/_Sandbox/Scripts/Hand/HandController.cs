using UnityEngine;

namespace _Sandbox.Scripts.Hand
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class HandController : MonoBehaviour
    {
        // [SerializeField] private MeshRenderer rend;

        private float x;
        private float y;
        private float closedValue;

        private const float HandZ = -0.5f;
        private float handYOffset = 4.5f * 2;
        private const float UNITS_RATIO = 10f;

        public bool IsClosed => closedValue > 0.5f;
        public Transform HandTransform => transform;

        private void Awake()
        {
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            var col = GetComponent<Collider>();
            col.isTrigger = true;

            // if (rend == null)
            //     rend = GetComponent<MeshRenderer>();
        }

        #region OSC INPUT

        public void SetRawX(float rawX)
        {
            x = rawX * UNITS_RATIO;
            UpdatePosition();
        }

        public void SetRawY(float rawY)
        {
            y = rawY * UNITS_RATIO + handYOffset;
            UpdatePosition();
        }

        public void SetClosed(float value)
        {
            closedValue = value;
            UpdateVisual();
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
            UpdateVisual();
        }

        #endregion

        private void UpdatePosition()
        {
            transform.position = new Vector3(x, y, HandZ);
        }

        private void UpdateVisual()
        {
            // if (rend != null)
            //     rend.material.color = IsClosed ? Color.red : Color.cyan;
        }
    }
}
