using UnityEngine;

public enum BubbleType
{
    Basic,
    BothHands,
    DoubleTap
}

public class BubbleBehavior : MonoBehaviour
{
    [SerializeField] private string word;
    [SerializeField] private float yValue;
    [SerializeField] private BubbleType type;
    [SerializeField] private float radius;

    private int interactions;

    public string Word => word;
    public float YValue => yValue;
    public BubbleType Type => type;
    public float Radius => radius;
    public int Interactions => interactions;

    public bool WasRightInteracting { get; set; }
    public bool WasLeftInteracting { get; set; }

    public void Configure(string bubbleWord, float bubbleYValue, BubbleType bubbleType, float bubbleRadius)
    {
        word = bubbleWord;
        yValue = bubbleYValue;
        type = bubbleType;
        radius = bubbleRadius;
    }

    public void RegisterInteraction()
    {
        interactions++;
    }
}
