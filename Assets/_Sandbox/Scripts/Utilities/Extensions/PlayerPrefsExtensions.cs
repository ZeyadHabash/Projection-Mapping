using System;
using UnityEngine;

namespace Utilities.Extensions
{
    public static class PlayerPrefsExtensions
    {
        public static void SetStringArray(string key, string[] array, string separator = ",") {
            PlayerPrefs.SetString(key, string.Join(separator, array));
        }

        public static string[] GetStringArray(string key, string separator = ",") {
            if (!PlayerPrefs.HasKey(key))
                return Array.Empty<string>();

            return PlayerPrefs.GetString(key).Split(new[] { separator }, StringSplitOptions.None);
        }
    }
}