using System;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField] private HandController rightHand;
    [SerializeField] private HandController leftHand;

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
    [SerializeField] private float wordHeightOffset = 0.65f;

    [SerializeField] private int labelFontSize = 96;
    [SerializeField] private float labelCharacterSize = 0.08f;

    [SerializeField] private int maxBubbles = 10;
    [SerializeField] private float spawnInterval = 1.25f;
    [SerializeField] private int spawnBurstCount = 3;

    private BubbleDataSet bubbleData;
    private readonly List<BubbleBehavior> activeBubbles = new();
    private readonly HashSet<string> spawnedWords = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> collectedWords = new();

    private float spawnTimer;

    public IReadOnlyList<string> CollectedWords => collectedWords;

    private void Awake()
    {
        LoadBubbleData();
    }

    private void Update()
    {
        if (bubbleData == null || bubbleData.bubbles == null || bubbleData.bubbles.Length == 0)
            return;

        TrySpawnBubble();
    }

    private void TrySpawnBubble()
    {
        if (activeBubbles.Count >= maxBubbles)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
            return;

        spawnTimer = spawnInterval;

        for (int i = 0; i < spawnBurstCount && activeBubbles.Count < maxBubbles; i++)
        {
            var data = GetUnusedBubbleEntry();
            if (data == null)
                break;

            SpawnBubble(data);
        }
    }

    private BubbleDataEntry GetUnusedBubbleEntry()
    {
        for (int i = 0; i < bubbleData.bubbles.Length; i++)
        {
            var candidate = bubbleData.bubbles[UnityEngine.Random.Range(0, bubbleData.bubbles.Length)];
            if (candidate == null || string.IsNullOrWhiteSpace(candidate.word))
                continue;

            if (!spawnedWords.Contains(candidate.word))
                return candidate;
        }

        return null;
    }

    private void SpawnBubble(BubbleDataEntry data)
    {
        var type = ParseBubbleType(data.type);
        var prefab = GetPrefabForType(type);

        var bubbleObject = Instantiate(prefab, transform);
        bubbleObject.transform.localScale = Vector3.one * (bubbleRadius * 2f);

        bubbleObject.transform.position = new Vector3(
            UnityEngine.Random.Range(playAreaMin.x, playAreaMax.x),
            UnityEngine.Random.Range(playAreaMin.y, playAreaMax.y),
            -0.5f);

        ApplyBubbleColor(bubbleObject, GetColorForType(type));
        CreateLabel(bubbleObject.transform, data.word, GetColorForType(type));

        var bubble = bubbleObject.GetComponent<BubbleBehavior>();
        if (bubble == null)
            bubble = bubbleObject.AddComponent<BubbleBehavior>();

        bubble.Configure(data.word, GetBubbleYValue(data.rgby), type);
        bubble.Initialize(rightHand, leftHand);
        bubble.OnCollected += HandleBubbleCollected;

        activeBubbles.Add(bubble);
        spawnedWords.Add(data.word);
    }

    private void HandleBubbleCollected(BubbleBehavior bubble)
    {
        activeBubbles.Remove(bubble);

        if (!string.IsNullOrWhiteSpace(bubble.Word))
            collectedWords.Add(bubble.Word);

        Destroy(bubble.gameObject);
    }

    private void LoadBubbleData()
    {
        if (bubbleDataFile == null)
            return;

        bubbleData = JsonUtility.FromJson<BubbleDataSet>(bubbleDataFile.text);
    }

    private static BubbleType ParseBubbleType(string rawType)
    {
        if (!string.IsNullOrWhiteSpace(rawType) &&
            Enum.TryParse(rawType, true, out BubbleType result))
            return result;

        return BubbleType.Basic;
    }

    private GameObject GetPrefabForType(BubbleType type)
    {
        return type switch
        {
            BubbleType.BothHands => bothHandsBubblePrefab ?? basicBubblePrefab,
            BubbleType.DoubleTap => doubleTapBubblePrefab ?? basicBubblePrefab,
            _ => basicBubblePrefab
        };
    }

    private Color GetColorForType(BubbleType type)
    {
        return type switch
        {
            BubbleType.BothHands => bothHandsBubbleColor,
            BubbleType.DoubleTap => doubleTapBubbleColor,
            _ => basicBubbleColor
        };
    }

    private static void ApplyBubbleColor(GameObject bubbleObject, Color color)
    {
        var renderers = bubbleObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers)
            r.material.color = color;
    }

    private void CreateLabel(Transform parent, string word, Color color)
    {
        var label = new GameObject("Label");
        label.transform.SetParent(parent);
        label.transform.localPosition = new Vector3(0f, wordHeightOffset, 0f);

        var text = label.AddComponent<TextMesh>();
        text.text = word;
        text.color = color;
        text.fontSize = labelFontSize;
        text.characterSize = labelCharacterSize;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
    }

    private static float GetBubbleYValue(float[] rgby)
    {
        if (rgby == null || rgby.Length < 4)
            return 0f;

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
