using System;

namespace Utilities.Extensions
{
    public static class EnumExtensions
    {
        public static T Cycle<T>(this T curValue, bool next) where T : Enum {
            return curValue.Add(next ? 1 : -1);
        }

        public static T Add<T>(this T curValue, int value) where T : Enum {
            T[] values = (T[])Enum.GetValues(typeof(T)); 
            int length = values.Length;

            int currentIndex = Array.IndexOf(values, curValue);
            int nextIndex = (currentIndex + value) % length;
            if (nextIndex < 0) nextIndex += length;

            return values[nextIndex];
        }
    }
}