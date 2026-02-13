using System.Collections.Generic;
using _Sandbox.Scripts.Structs;
using _Sandbox.Scripts.Utilities.Bases;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    public class WordManager : Singleton<WordManager>
    {
        [SerializeField] private TextAsset wordsDataFile;
        private WordDataSet wordData;
        private List<string> wordPool = new List<string>();
        private List<string> collectedWords = new List<string>();
       


        public IReadOnlyList<string> CollectedWords => collectedWords;
        public WordDataSet WordData => wordData;

        protected override void Awake() {
            base.Awake();
            LoadWordsData();
        }

        public void Reset() {
            collectedWords.Clear();
        }

        private void LoadWordsData() {
            if (wordsDataFile == null) return;
            wordData = JsonUtility.FromJson<WordDataSet>(wordsDataFile.text);
        }

        public void CollectWord(string word) {
            collectedWords.Add(word);
        }

        public void RemoveRandomCollectedWord()
        {
            if (collectedWords.Count == 0)
                return;

            int randomIndex = Random.Range(0, collectedWords.Count);
            collectedWords.RemoveAt(randomIndex);
        }

     

        public enum WordColor
        {
            Red,
            Green,
            Blue,
            Yellow,
            White,
            Gray,
        }

        public WordColor GetDominantCollectedColor()
        {
                 
            Debug.Log($"--- collection? {collectedWords.Count}");
            
            if (collectedWords.Count == 0 || wordData == null || wordData.words == null)
                return WordColor.Gray;

            float totalR = 0f;
            float totalG = 0f;
            float totalB = 0f;
            float totalY = 0f;

            
            foreach (string collectedWord in collectedWords)
            {
                foreach (WordDataEntry entry in wordData.words)
                {
                    if (entry.word == collectedWord)
                    {
                        if (entry.rgby != null && entry.rgby.Length >= 4)
                        {
                            totalR += entry.rgby[0];
                            totalG += entry.rgby[1];
                            totalB += entry.rgby[2];
                            totalY += entry.rgby[3];
                        }
                        break; 
                    }
                }
            }

            
            if (Mathf.Approximately(totalR, totalG) &&
                Mathf.Approximately(totalR, totalB) &&
                Mathf.Approximately(totalR, totalY))
            {
                return WordColor.White;
            }

            
            WordColor dominant = WordColor.Red;
            float max = totalR;

            if (totalG > max)
            {
                max = totalG;
                dominant = WordColor.Green;
            }
            if (totalB > max)
            {
                max = totalB;
                dominant = WordColor.Blue;
            }
            if (totalY > max)
            {
                dominant = WordColor.Yellow;
            }

            return dominant;
        }





    }



    // Word Pool
    // Collected Word

    // Read Words
    // Pop Word
    // Collect Word
    // Get Random Word
}
