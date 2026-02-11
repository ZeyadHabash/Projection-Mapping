using System;
using _Sandbox.Scripts.Bubble;
using _Sandbox.Scripts.Hand;
using TMPro;
using UnityEngine;

public enum BubbleType
{
    Basic,
    BothHands,
    DoubleTap
}

[RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(Rigidbody))]
public class BubbleBehavior : MonoBehaviour
{
    [SerializeField] private string word;
    [SerializeField] private float yValue;
    [SerializeField] private BubbleType type;

    private HandController rightHand;
    private HandController leftHand;
    private TextMeshPro wordText;
    private BubbleEffect bubbleEffect;

    private bool rightTouching;
    private bool leftTouching;
    private int interactions;

    public string Word => word;
    public BubbleType Type => type;

    public static event Action<BubbleBehavior> OnCollected;

    private void Awake()
    {
        wordText = GetComponentInChildren<TextMeshPro>();
        bubbleEffect = GetComponent<BubbleEffect>();
        // var rb = GetComponent<Rigidbody>();
        // rb.isKinematic = true;
        // rb.useGravity = false;
    }

    public void Configure(string bubbleWord, float bubbleYValue, BubbleType bubbleType)
    {
        word = bubbleWord;
        yValue = bubbleYValue;
        type = bubbleType;
        wordText.text = word;
    }

    public void Initialize(HandController right, HandController left)
    {
        rightHand = right;
        leftHand = left;
        bubbleEffect.Init(right, left);
    }

    private void OnTriggerEnter(Collider other)
    {
        var hand = other.GetComponent<HandController>();
        if (hand == null) return;

        if (hand == rightHand)
        {
            rightTouching = true;
            rightHand.OnHandClosed += OnRightHandClosed;
        }

        if (hand == leftHand)
        {
            leftTouching = true;
            leftHand.OnHandClosed += OnLeftHandClosed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var hand = other.GetComponent<HandController>();
        if (hand == null) return;

        if (hand == rightHand)
        {
            rightTouching = false;
            rightHand.OnHandClosed -= OnRightHandClosed;
        }

        if (hand == leftHand)
        {
            leftTouching = false;
            leftHand.OnHandClosed -= OnLeftHandClosed;
        }
    }

    private void OnRightHandClosed()
    {
        HandleHandClose(isRight: true);
    }

    private void OnLeftHandClosed()
    {
        HandleHandClose(isRight: false);
    }

    private void HandleHandClose(bool isRight)
    {
        switch (type)
        {
            case BubbleType.Basic:
                Collect();
                break;

            case BubbleType.BothHands:
                // Need both hands touching
                if (rightTouching && leftTouching)
                    Collect();
                break;

            case BubbleType.DoubleTap:
                interactions++;
                if (interactions >= 2)
                    Collect();
                break;
        }
    }

    private void Update()
    {
    }

    private void Collect()
    {
        OnCollected?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (rightHand != null)
            rightHand.OnHandClosed -= OnRightHandClosed;

        if (leftHand != null)
            leftHand.OnHandClosed -= OnLeftHandClosed;
    }
}
