using _Sandbox.Scripts.Hand;
using UnityEngine;

public class ClickToDestroy : MonoBehaviour
{
    private HandController touchingHand = null;
    void OnMouseDown()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (touchingHand != null && touchingHand.IsClosed)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var hand = other.GetComponent<HandController>();
        if (hand == null) return;

        touchingHand = hand;
    }
}
