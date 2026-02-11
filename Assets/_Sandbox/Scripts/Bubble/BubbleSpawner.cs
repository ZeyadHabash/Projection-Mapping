using System;
using System.Collections;
using System.Collections.Generic;
using _Sandbox.Scripts.Hand;
using _Sandbox.Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private HandController rightHand;
        [SerializeField] private HandController leftHand;

        [SerializeField] private TextAsset bubbleDataFile;
        [SerializeField] private GameObject basicBubblePrefab;
        [SerializeField] private GameObject bothHandsBubblePrefab;
        [SerializeField] private GameObject doubleTapBubblePrefab;

        [SerializeField] private Vector2 playAreaMin = new Vector2(-5f, 2f);
        [SerializeField] private Vector2 playAreaMax = new Vector2(5f, 10f);

        [SerializeField] private float bubbleRadius = 0.5f;
        [SerializeField] private float wordHeightOffset = 0.65f;
        [SerializeField] private float minSpawnSeparation = 1.1f;
        [SerializeField] private int spawnPositionAttempts = 12;
        [SerializeField] private float bubbleLifetime = 10f;

        [SerializeField] private int maxBubbles = 10;
        [SerializeField] private float spawnInterval = 1.25f;
        [SerializeField] private float minSpawnInterval = 0.35f;
        [FormerlySerializedAs("spawnIntervalDecreasePerSecond")]
        [SerializeField] private float spawnRateIncreasePerSecond = 0.02f;
        [SerializeField] private int spawnBurstCount = 3;

        [Header("Bubble Type Weights")]
        [SerializeField] private BubbleTypeWeight[] bubbleTypeWeights;

        [Header("Audio")]
        [SerializeField] private AudioClip[] spawnAudioClips;
        [SerializeField] private AudioClip[] collectAudioClips;

        private BubbleDataSet bubbleData;
        private static readonly BubbleType[] bubbleTypes = (BubbleType[])Enum.GetValues(typeof(BubbleType));
        private readonly List<BubbleBehavior> activeBubbles = new();
        private readonly HashSet<string> spawnedWords = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> collectedWords = new();
        private readonly Dictionary<BubbleBehavior, Coroutine> despawnRoutines = new();

        private float spawnTimer;
        private float elapsedTime;
        private float currentSpawnInterval;

        public IReadOnlyList<string> CollectedWords => collectedWords;

        private void Awake()
        {
            LoadBubbleData();
        }

        private void Update()
        {
            if (bubbleData == null || bubbleData.bubbles == null || bubbleData.bubbles.Length == 0)
                return;

            elapsedTime += Time.deltaTime;
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                spawnInterval - (spawnRateIncreasePerSecond * elapsedTime));

            TrySpawnBubble();
        }

        private void TrySpawnBubble()
        {
            if (activeBubbles.Count >= maxBubbles)
                return;

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f)
                return;

            spawnTimer = currentSpawnInterval;

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
            var type = GetWeightedBubbleType();
            var prefab = GetPrefabForType(type);

            if (!TryGetSpawnPosition(out var spawnPosition))
                return;

            var bubbleObject = Instantiate(prefab, transform);
            bubbleObject.transform.localScale = Vector3.one * (bubbleRadius * 2f);

            bubbleObject.transform.position = spawnPosition;

            // ApplyBubbleColor(bubbleObject, GetColorForType(type));
            // CreateLabel(bubbleObject.transform, data.word, GetColorForType(type));

            var bubble = bubbleObject.GetComponent<BubbleBehavior>();
            if (bubble == null)
                bubble = bubbleObject.AddComponent<BubbleBehavior>();

            bubble.Configure(data.word, GetBubbleYValue(data.rgby), type);
            bubble.Initialize(rightHand, leftHand);

            activeBubbles.Add(bubble);
            spawnedWords.Add(data.word);

            StartDespawnRoutine(bubble);

            PlayRandomAudio(spawnAudioClips);
        }

        private void HandleBubbleCollected(BubbleBehavior bubble)
        {
            StopDespawnRoutine(bubble);
            activeBubbles.Remove(bubble);

            if (!string.IsNullOrWhiteSpace(bubble.Word))
                collectedWords.Add(bubble.Word);

            PlayRandomAudio(collectAudioClips);
            // StartCoroutine(DestroyAfterHide(bubble));
            Destroy(bubble.gameObject);
        }

        private void HandleBubbleExpired(BubbleBehavior bubble)
        {
            StopDespawnRoutine(bubble);
            activeBubbles.Remove(bubble);
            StartCoroutine(DestroyAfterHide(bubble));
        }

        private bool TryGetSpawnPosition(out Vector3 position)
        {
            float minDistance = Mathf.Max(minSpawnSeparation, bubbleRadius * 2f);
            float minDistanceSqr = minDistance * minDistance;

            for (int attempt = 0; attempt < spawnPositionAttempts; attempt++)
            {
                position = new Vector3(
                    UnityEngine.Random.Range(playAreaMin.x, playAreaMax.x),
                    UnityEngine.Random.Range(playAreaMin.y, playAreaMax.y),
                    -0.5f);

                if (IsPositionClear(position, minDistanceSqr))
                    return true;
            }

            position = Vector3.zero;
            return false;
        }

        private bool IsPositionClear(Vector3 position, float minDistanceSqr)
        {
            for (int i = 0; i < activeBubbles.Count; i++)
            {
                var bubble = activeBubbles[i];
                if (bubble == null)
                    continue;

                if ((bubble.transform.position - position).sqrMagnitude < minDistanceSqr)
                    return false;
            }

            return true;
        }

        private void PlayRandomAudio(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return;

            if (AudioFXManager.Instance == null)
                return;

            AudioFXManager.Instance.PlayRandomFXClip(clips);
        }

        private void PlayRandomVfx(GameObject[] prefabs, Vector3 position, float lifetime)
        {
            if (AudioFXManager.Instance == null)
                return;

            AudioFXManager.Instance.SpawnRandomVfx(prefabs, position, lifetime, transform);
        }

        private IEnumerator DestroyAfterHide(BubbleBehavior bubble)
        {
            if (bubble == null)
                yield break;

            var effect = bubble.GetComponent<BubbleEffect>();
            if (effect != null)
            {
                effect.Hide();
                yield return new WaitForSeconds(effect.HideDuration);
            }

            if (bubble != null)
                Destroy(bubble.gameObject);
        }

        private void StartDespawnRoutine(BubbleBehavior bubble)
        {
            if (bubble == null || bubbleLifetime <= 0f)
                return;

            StopDespawnRoutine(bubble);
            despawnRoutines[bubble] = StartCoroutine(DespawnAfterLifetime(bubble));
        }

        private void StopDespawnRoutine(BubbleBehavior bubble)
        {
            if (bubble == null)
                return;

            if (despawnRoutines.TryGetValue(bubble, out var routine))
            {
                StopCoroutine(routine);
                despawnRoutines.Remove(bubble);
            }
        }

        private IEnumerator DespawnAfterLifetime(BubbleBehavior bubble)
        {
            yield return new WaitForSeconds(bubbleLifetime);

            if (bubble == null)
                yield break;

            if (!activeBubbles.Contains(bubble))
            {
                StopDespawnRoutine(bubble);
                yield break;
            }

            HandleBubbleExpired(bubble);
        }

        private void LoadBubbleData()
        {
            if (bubbleDataFile == null)
                return;

            bubbleData = JsonUtility.FromJson<BubbleDataSet>(bubbleDataFile.text);
        }

        private BubbleType GetWeightedBubbleType()
        {
            if (bubbleTypeWeights == null || bubbleTypeWeights.Length == 0)
                return GetRandomBubbleTypeFallback();

            float totalWeight = 0f;
            for (int i = 0; i < bubbleTypeWeights.Length; i++)
            {
                var entry = bubbleTypeWeights[i];
                if (entry == null)
                    continue;

                totalWeight += Mathf.Max(0f, entry.BaseWeight + (entry.WeightIncreasePerSecond * elapsedTime));
            }

            if (totalWeight <= 0f)
                return GetRandomBubbleTypeFallback();

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            for (int i = 0; i < bubbleTypeWeights.Length; i++)
            {
                var entry = bubbleTypeWeights[i];
                if (entry == null)
                    continue;

                float weight = Mathf.Max(0f, entry.BaseWeight + (entry.WeightIncreasePerSecond * elapsedTime));
                if (weight <= 0f)
                    continue;

                roll -= weight;
                if (roll <= 0f)
                    return entry.Type;
            }

            return GetRandomBubbleTypeFallback();
        }

        private static BubbleType GetRandomBubbleTypeFallback()
        {
            if (bubbleTypes == null || bubbleTypes.Length == 0)
                return BubbleType.Basic;

            return bubbleTypes[UnityEngine.Random.Range(0, bubbleTypes.Length)];
        }

        private GameObject GetPrefabForType(BubbleType type)
        {
            return type switch
            {
                BubbleType.BothHands => bothHandsBubblePrefab ? bothHandsBubblePrefab : basicBubblePrefab,
                BubbleType.DoubleTap => doubleTapBubblePrefab ? doubleTapBubblePrefab : basicBubblePrefab,
                _ => basicBubblePrefab
            };
        }

        private static float GetBubbleYValue(float[] rgby)
        {
            if (rgby == null || rgby.Length < 4)
                return 0f;

            return rgby[3];
        }

        private void OnEnable()
        {
            BubbleBehavior.OnCollected += HandleBubbleCollected;
        }

        private void OnDisable()
        {
            BubbleBehavior.OnCollected -= HandleBubbleCollected;
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
        }

        [Serializable]
        private class BubbleTypeWeight
        {
            [SerializeField] private BubbleType type;
            [SerializeField] private float baseWeight = 1f;
            [SerializeField] private float weightIncreasePerSecond = 0f;

            public BubbleType Type => type;
            public float BaseWeight => baseWeight;
            public float WeightIncreasePerSecond => weightIncreasePerSecond;
        }
    }
}
