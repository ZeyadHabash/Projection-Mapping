using System;

namespace _Sandbox.Scripts.Structs
{
    [Serializable]
    public struct WordDataSet
    {
        public WordDataEntry[] words;
    }
    
    [Serializable]
    public struct WordDataEntry
    {
        public string word;
        public float[] rgby;
    }
}