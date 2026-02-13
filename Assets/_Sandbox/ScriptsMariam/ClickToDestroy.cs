using System.Collections;
using _Sandbox.Scripts.Hand;
using _Sandbox.Scripts.Managers;
using DG.Tweening;
using UnityEngine;

public class ClickToDestroy : MonoBehaviour
{
    private HandController touchingHand = null;
    [SerializeField] private AudioClip destroySound;

    private bool isDead = false;
    
    void OnMouseDown()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (touchingHand != null && touchingHand.IsClosed) {
            if (isDead) return;
            isDead = true;
            StartCoroutine(ShrinkAndDestroyRoutine());
        }
    }

    private IEnumerator ShrinkAndDestroyRoutine()
    {
        // 1. Play Sound
        if (AudioFXManager.Instance != null && destroySound != null) 
            AudioFXManager.Instance.PlayFXClip(destroySound);

        yield return transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack)
            .WaitForCompletion();

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var hand = other.GetComponent<HandController>();
        if (hand == null) return;

        touchingHand = hand;
    }
}
