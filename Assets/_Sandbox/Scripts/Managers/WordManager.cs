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
            LoadWordsData();
        }

        private void LoadWordsData()
        {
            if (wordsDataFile == null) return;

            wordData = JsonUtility.FromJson<WordDataSet>(wordsDataFile.text);
        }

        public void CollectWord(string word) {
            collectedWords.Add(word);
        }

        // Word Pool
        // Collected Word

        // Read Words
        // Pop Word
        // Collect Word
        // Get Random Word

    }
}