using System;

namespace _Sandbox.Scripts.Structs
{
    [Serializable]
    public class WordDataSet
    {
        public WordDataEntry[] words;
    }
    
    [Serializable]
    public class WordDataEntry
    {
        public string word;
        public float[] rgby;
    }
}