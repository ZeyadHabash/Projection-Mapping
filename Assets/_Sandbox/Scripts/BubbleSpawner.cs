using System;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField] private TestTrackingNew tracking;
    [SerializeField] private TextAsset bubbleDataFile;
    [SerializeField] private GameObject basicBubblePrefab;
    [SerializeField] private GameObject bothHandsBubblePrefab;
    [SerializeField] private GameObject doubleTapBubblePrefab;
    [SerializeField] private Color basicBubbleColor = Color.blue;
    [SerializeField] private Color bothHandsBubbleColor = Color.red;
    [SerializeField] private Color doubleTapBubbleColor = Color.green;
    [SerializeField] private Vector2 playAreaMin = new Vector2(-5f, 2f);
    [SerializeField] private Vector2 playAreaMax = new Vector2(5f, 10f);
    [SerializeField] private float bubbleRadius = 0.5f;
    [SerializeField] private float interactionRadius = 0.5f;
    [SerializeField] private float wordHeightOffset = 0.65f;
    [SerializeField] private int labelFontSize = 96;
    [SerializeField] private float labelCharacterSize = 0.08f;
    [SerializeField] private int maxBubbles = 10;
    [SerializeField] private float spawnInterval = 1.25f;
    [SerializeField] private int spawnBurstCount = 3;

    private BubbleDataSet bubbleData;
    private readonly List<BubbleBehavior> activeBubbles = new List<BubbleBehavior>();
    private readonly HashSet<string> activeWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> spawnedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> collectedWords = new List<string>();
    private float spawnTimer;

    public IReadOnlyList<string> CollectedWords => collectedWords;

    private void Awake()
    {
        if (tracking == null)
        {
            tracking = FindFirstObjectByType<TestTrackingNew>();
        }

        LoadBubbleData();
    }

    private void Update()
    {
        if (tracking == null || bubbleData == null || bubbleData.bubbles == null || bubbleData.bubbles.Length == 0)
        {
            return;
        }

        PruneDestroyedBubbles();
        TrySpawnBubble();
        UpdateBubbleInteractions();
    }

    private void LoadBubbleData()
    {
        if (bubbleDataFile == null)
        {
            bubbleDataFile = Resources.Load<TextAsset>("BubbleData");
        }

        if (bubbleDataFile == null)
        {
            Debug.LogWarning("BubbleSpawner: No bubble data file assigned.");
            return;
        }

        bubbleData = JsonUtility.FromJson<BubbleDataSet>(bubbleDataFile.text);
        if (bubbleData == null || bubbleData.bubbles == null || bubbleData.bubbles.Length == 0)
        {
            Debug.LogWarning("BubbleSpawner: Bubble data file has no entries.");
        }
    }

    private void TrySpawnBubble()
    {
        if (activeBubbles.Count >= maxBubbles)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
        {
            return;
        }

        spawnTimer = spawnInterval;
        var spawnCount = Mathf.Max(1, spawnBurstCount);
        for (var i = 0; i < spawnCount && activeBubbles.Count < maxBubbles; i++)
        {
            var data = GetUnusedBubbleEntry();
            if (data == null)
            {
                break;
            }

            SpawnBubble(data);
        }
    }

    private BubbleDataEntry GetUnusedBubbleEntry()
    {
        for (var i = 0; i < bubbleData.bubbles.Length; i++)
        {
            var candidate = bubbleData.bubbles[UnityEngine.Random.Range(0, bubbleData.bubbles.Length)];
            if (candidate == null || string.IsNullOrWhiteSpace(candidate.word))
            {
                continue;
            }

            if (CanSpawnWord(candidate.word))
            {
                return candidate;
            }
        }

        return null;
    }

    private bool CanSpawnWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return false;
        }

        return !spawnedWords.Contains(word);
    }

    private void SpawnBubble(BubbleDataEntry data)
    {
        if (!CanSpawnWord(data.word))
        {
            return;
        }

        var bubbleType = ParseBubbleType(data.type);
        var prefab = GetPrefabForType(bubbleType);
        if (prefab == null)
        {
            Debug.LogWarning($"BubbleSpawner: Missing prefab for type {bubbleType}.");
            return;
        }

        var bubbleObject = Instantiate(prefab, transform);
        bubbleObject.name = $"Bubble_{data.word}";
        bubbleObject.transform.localScale = Vector3.one * (bubbleRadius * 2f);

        var bubbleZ = GetHandZ();
        bubbleObject.transform.position = new Vector3(
            UnityEngine.Random.Range(playAreaMin.x, playAreaMax.x),
            UnityEngine.Random.Range(playAreaMin.y, playAreaMax.y),
            bubbleZ);

        var bubbleColor = GetColorForType(bubbleType);
        ApplyBubbleColor(bubbleObject, bubbleColor);

        CreateLabel(bubbleObject.transform, data.word, bubbleColor);

        var bubble = bubbleObject.GetComponent<BubbleBehavior>();
        if (bubble == null)
        {
            bubble = bubbleObject.AddComponent<BubbleBehavior>();
        }

        bubble.Configure(data.word, GetBubbleYValue(data.rgby), bubbleType, bubbleRadius);

        activeBubbles.Add(bubble);
        if (!string.IsNullOrWhiteSpace(data.word))
        {
            activeWords.Add(data.word);
            spawnedWords.Add(data.word);
        }
    }

    private void CreateLabel(Transform parent, string word, Color color)
    {
        var labelObject = new GameObject("Label");
        labelObject.transform.SetParent(parent);
        labelObject.transform.localPosition = new Vector3(0f, wordHeightOffset, 0f);

        var textMesh = labelObject.AddComponent<TextMesh>();
        textMesh.text = word;
        textMesh.color = color;
        textMesh.fontSize = labelFontSize;
        textMesh.characterSize = labelCharacterSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
    }

    private void UpdateBubbleInteractions()
    {
        var rightHand = tracking.RightHandTransform;
        var leftHand = tracking.LeftHandTransform;
        var rightClosed = tracking.RightHandClosed;
        var leftClosed = tracking.LeftHandClosed;
        var hasRight = rightHand != null;
        var hasLeft = leftHand != null;

        for (var i = activeBubbles.Count - 1; i >= 0; i--)
        {
            var bubble = activeBubbles[i];
            if (bubble == null)
            {
                activeBubbles.RemoveAt(i);
                continue;
            }

            var bubblePosition = bubble.transform.position;
            var rightOnTop = IsHandOnTop(rightHand, bubblePosition, interactionRadius);
            var leftOnTop = IsHandOnTop(leftHand, bubblePosition, interactionRadius);
            var rightInteracting = hasRight && rightOnTop && rightClosed;
            var leftInteracting = hasLeft && leftOnTop && leftClosed;

            switch (bubble.Type)
            {
                case BubbleType.Basic:
                    if (rightInteracting || leftInteracting)
                    {
                        CollectBubble(i, bubble);
                    }

                    break;
                case BubbleType.BothHands:
                    if (hasRight && hasLeft)
                    {
                        if (rightInteracting && leftInteracting)
                        {
                            CollectBubble(i, bubble);
                        }
                    }
                    else if (rightInteracting || leftInteracting)
                    {
                        CollectBubble(i, bubble);
                    }

                    break;
                case BubbleType.DoubleTap:
                    if (rightInteracting && !bubble.WasRightInteracting)
                    {
                        bubble.RegisterInteraction();
                    }

                    if (leftInteracting && !bubble.WasLeftInteracting)
                    {
                        bubble.RegisterInteraction();
                    }

                    if (bubble.Interactions >= 2)
                    {
                        CollectBubble(i, bubble);
                    }

                    break;
            }

            bubble.WasRightInteracting = rightInteracting;
            bubble.WasLeftInteracting = leftInteracting;
        }
    }

    private void CollectBubble(int index, BubbleBehavior bubble)
    {
        activeBubbles.RemoveAt(index);
        if (!string.IsNullOrWhiteSpace(bubble.Word))
        {
            activeWords.Remove(bubble.Word);
            collectedWords.Add(bubble.Word);
            for (int i = 0; i < collectedWords.Count; i++)
            {
                Debug.Log(collectedWords[i]);
            }
        }

        Destroy(bubble.gameObject);
    }

    private void PruneDestroyedBubbles()
    {
        for (var i = activeBubbles.Count - 1; i >= 0; i--)
        {
            if (activeBubbles[i] == null)
            {
                activeBubbles.RemoveAt(i);
            }
        }

        activeWords.Clear();
        for (var i = 0; i < activeBubbles.Count; i++)
        {
            var bubble = activeBubbles[i];
            if (bubble != null && !string.IsNullOrWhiteSpace(bubble.Word))
            {
                activeWords.Add(bubble.Word);
            }
        }
    }

    private static bool IsHandOnTop(Transform handTransform, Vector3 bubblePosition, float radius)
    {
        if (handTransform == null)
        {
            return false;
        }

        var handPosition = handTransform.position;
        var delta = new Vector2(handPosition.x - bubblePosition.x, handPosition.y - bubblePosition.y);
        return delta.sqrMagnitude <= radius * radius;
    }

    private float GetHandZ()
    {
        if (tracking.RightHandTransform != null)
        {
            return tracking.RightHandTransform.position.z;
        }

        if (tracking.LeftHandTransform != null)
        {
            return tracking.LeftHandTransform.position.z;
        }

        return -0.5f;
    }

    private static BubbleType ParseBubbleType(string rawType)
    {
        if (!string.IsNullOrWhiteSpace(rawType) && Enum.TryParse(rawType, true, out BubbleType result))
        {
            return result;
        }

        return BubbleType.Basic;
    }

    private GameObject GetPrefabForType(BubbleType type)
    {
        switch (type)
        {
            case BubbleType.BothHands:
                return bothHandsBubblePrefab != null ? bothHandsBubblePrefab : basicBubblePrefab;
            case BubbleType.DoubleTap:
                return doubleTapBubblePrefab != null ? doubleTapBubblePrefab : basicBubblePrefab;
            default:
                return basicBubblePrefab;
        }
    }

    private Color GetColorForType(BubbleType type)
    {
        switch (type)
        {
            case BubbleType.BothHands:
                return bothHandsBubbleColor;
            case BubbleType.DoubleTap:
                return doubleTapBubbleColor;
            default:
                return basicBubbleColor;
        }
    }

    private static void ApplyBubbleColor(GameObject bubbleObject, Color color)
    {
        var renderers = bubbleObject.GetComponentsInChildren<MeshRenderer>();
        for (var i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }

    private static float GetBubbleYValue(float[] rgby)
    {
        if (rgby == null || rgby.Length < 4)
        {
            return 0f;
        }

        return rgby[3];
    }

    [Serializable]
    private class BubbleDataSet
    {
        public BubbleDataEntry[] bubbles;
    }

    [Serializable]
    private class BubbleDataEntry
    {
        public string word;
        public float[] rgby;
        public string type;
    }
}