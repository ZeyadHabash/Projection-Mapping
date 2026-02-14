using System;
using System.Collections;
using System.Collections.Generic;
using _Sandbox.Scripts.Enums;
using _Sandbox.Scripts.Managers;
using _Sandbox.Scripts.Structs;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform[] spawnPointsTwo;

        [SerializeField] private GameObject basicBubblePrefab;
        [SerializeField] private GameObject bothHandsBubblePrefab;
        [SerializeField] private GameObject doubleTapBubblePrefab;

        [SerializeField] private float bubbleRadius = 0.5f;
        [SerializeField] private float bubbleLifetime = 10f;

        [SerializeField] private int maxBubbles = 10;
        [SerializeField] private float spawnInterval = 1.25f;
        [SerializeField] private float minSpawnInterval = 0.35f;
        [FormerlySerializedAs("spawnIntervalDecreasePerSecond")]
        [SerializeField] private float spawnRateIncreasePerSecond = 0.02f;

        [Header("Bubble Type Weights")]
        [SerializeField] private BubbleTypeWeight[] bubbleTypeWeights;

        [Header("Audio")]
        [SerializeField] private AudioClip[] spawnAudioClips;

        private bool isFirstSpawnPattern = false;

        private static readonly BubbleType[] bubbleTypes = (BubbleType[])Enum.GetValues(typeof(BubbleType));
        private readonly List<BubbleBehavior> activeBubbles = new();
        private readonly HashSet<string> spawnedWords = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<BubbleBehavior, Coroutine> despawnRoutines = new();

        private float spawnTimer;
        private float elapsedTime;
        private float currentSpawnInterval;
        
        private void Update()
        {
            // if (bubbleData == null || bubbleData.bubbles == null || bubbleData.bubbles.Length == 0)
            //     return;

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

            var activeSpawnPoints = isFirstSpawnPattern ? spawnPoints : spawnPointsTwo;
            isFirstSpawnPattern = !isFirstSpawnPattern;

            for (int i = 0; i < 4 && activeBubbles.Count < maxBubbles; i++)
            {
                var data = GetUnusedWordEntry();
                if (data != null) SpawnBubble(data, activeSpawnPoints[i]);
            }
        }

        private void SpawnBubble(WordDataEntry data, Transform point)
        {
            var type = GetWeightedBubbleType();
            var prefab = GetPrefabForType(type);

            var bubbleObject = Instantiate(prefab, transform);
            bubbleObject.transform.localScale = Vector3.one * (bubbleRadius * 2f);

            bubbleObject.transform.position = point.position;

            var bubble = bubbleObject.GetComponent<BubbleBehavior>();
            if (bubble == null)
                bubble = bubbleObject.AddComponent<BubbleBehavior>();

            bubble.Configure(data.word, GetBubbleYValue(data.rgby), type);
            bubble.EnableAndAppear();

            activeBubbles.Add(bubble);
            spawnedWords.Add(data.word);

            StartDespawnRoutine(bubble);
            PlayRandomAudio(spawnAudioClips);
        }
        
        private void PlayRandomAudio(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return;

            if (AudioFXManager.Instance == null)
                return;

            AudioFXManager.Instance.PlayRandomFXClip(clips);
        }
        
        private WordDataEntry GetUnusedWordEntry() {
            var wordData = WordManager.Instance.WordData;
            for (int i = 0; i < wordData.words.Length; i++)
            {
                var candidate = wordData.words[Random.Range(0, wordData.words.Length)];
                if (string.IsNullOrWhiteSpace(candidate.word))
                    continue;

                if (!spawnedWords.Contains(candidate.word))
                    return candidate;
            }

            return null;
        }

        private void HandleBubbleCollected(BubbleBehavior bubble)
        {
            StopDespawnRoutine(bubble);
            activeBubbles.Remove(bubble);

            if (!string.IsNullOrWhiteSpace(bubble.Word)) WordManager.Instance.CollectWord(bubble.Word);

            bubble.CollectAndHide();
            // StartCoroutine(DestroyAfterHide(bubble));
            // Destroy(bubble.gameObject);
        }

        private void HandleBubbleExpired(BubbleBehavior bubble)
        {
            StopDespawnRoutine(bubble);
            activeBubbles.Remove(bubble);
            bubble.DisableAndHide();
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
