using _Sandbox.Scripts.Hand;
using UnityEngine;

namespace _Sandbox.ScriptsMariam
{
    public class HandsToDestroy : MonoBehaviour
    {
        [Header("Optional Visual Feedback")]
        [SerializeField] private Renderer rend;
        [SerializeField] private Color singleHandColor = Color.yellow;
        private Color originalColor;

        private bool leftTouching = false;
        private bool rightTouching = false;

        private void Start()
        {
            if (rend == null)
                rend = GetComponent<Renderer>();

            if (rend != null)
                originalColor = rend.material.color;
        }

        private void Update()
        {
        
            if (leftTouching && rightTouching)
            {
                Destroy(gameObject);
                return;
            }

        
            if (rend != null)
            {
                if (leftTouching || rightTouching)
                    rend.material.color = singleHandColor;
                else
                    rend.material.color = originalColor;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            HandController hand = other.GetComponent<HandController>();
            if (hand == null) return;

            if (hand.CompareTag("LeftHand"))
                leftTouching = true;
            else if (hand.CompareTag("RightHand"))
                rightTouching = true;
        }

        private void OnTriggerExit(Collider other)
        {
            HandController hand = other.GetComponent<HandController>();
            if (hand == null) return;

            if (hand.CompareTag("LeftHand"))
                leftTouching = false;
            else if (hand.CompareTag("RightHand"))
                rightTouching = false;
        }
    }
}
