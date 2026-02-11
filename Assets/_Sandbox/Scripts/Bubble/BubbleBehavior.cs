using System;
using UnityEngine;

public enum BubbleType
{
    Basic,
    BothHands,
    DoubleTap
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BubbleBehavior : MonoBehaviour
{
    [SerializeField] private string word;
    [SerializeField] private float yValue;
    [SerializeField] private BubbleType type;

    private HandController rightHand;
    private HandController leftHand;

    private bool rightTouching;
    private bool leftTouching;

    private bool wasRightInteracting;
    private bool wasLeftInteracting;

    private int interactions;

    public string Word => word;
    public BubbleType Type => type;

    public event Action<BubbleBehavior> OnCollected;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Configure(string bubbleWord, float bubbleYValue, BubbleType bubbleType)
    {
        word = bubbleWord;
        yValue = bubbleYValue;
        type = bubbleType;
    }

    public void Initialize(HandController right, HandController left)
    {
        rightHand = right;
        leftHand = left;
    }

    private void OnTriggerEnter(Collider other)
    {
        var hand = other.GetComponent<HandController>();
        if (hand == null) return;

        if (hand == rightHand)
            rightTouching = true;

        if (hand == leftHand)
            leftTouching = true;
    }

    private void OnTriggerExit(Collider other)
    {
        var hand = other.GetComponent<HandController>();
        if (hand == null) return;

        if (hand == rightHand)
            rightTouching = false;

        if (hand == leftHand)
            leftTouching = false;
    }

    private void Update()
    {
        if (rightHand == null && leftHand == null)
            return;

        bool rightInteracting = rightTouching && rightHand != null && rightHand.IsClosed;
        bool leftInteracting = leftTouching && leftHand != null && leftHand.IsClosed;

        switch (type)
        {
            case BubbleType.Basic:
                if (rightInteracting || leftInteracting)
                {
                    Collect();
                }
                break;

            case BubbleType.BothHands:
                if (rightHand != null && leftHand != null)
                {
                    if (rightInteracting && leftInteracting)
                        Collect();
                }
                else if (rightInteracting || leftInteracting)
                {
                    Collect();
                }
                break;

            case BubbleType.DoubleTap:

                if (rightInteracting && !wasRightInteracting)
                    interactions++;

                if (leftInteracting && !wasLeftInteracting)
                    interactions++;

                if (interactions >= 2)
                    Collect();

                break;
        }

        wasRightInteracting = rightInteracting;
        wasLeftInteracting = leftInteracting;
    }

    private void Collect()
    {
        OnCollected?.Invoke(this);
    }
}
