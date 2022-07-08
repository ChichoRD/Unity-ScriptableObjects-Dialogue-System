/*
Copyright(c) 2021 Chicho Studio

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/

using System;
using UnityEngine;

namespace ChichoExtensions
{
    internal static partial class ExtensionMethods
    {
        private static int HexToDec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        private static string DecToHex(int value)
        {
            return value.ToString("X2");
        }

        private static string NormailizedFloatToHex(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255));
        }

        private static float HexToNormalizedFloat(string hex)
        {
            return HexToDec(hex) / 255f;
        }

        public static Color ColorFromHex(string hexString)
        {
            float red = HexToNormalizedFloat(hexString.Substring(0, 2));
            float green = HexToNormalizedFloat(hexString.Substring(2, 2));
            float blue = HexToNormalizedFloat(hexString.Substring(4, 2));

            return new Color(red, green, blue, hexString.Length >= 8 ? HexToNormalizedFloat(hexString.Substring(6, 2)) : 1);
        }

        public static string HexFromColor(Color color, bool useAlpha = false)
        {
            string red = NormailizedFloatToHex(color.r);
            string green = NormailizedFloatToHex(color.g);
            string blue = NormailizedFloatToHex(color.b);

            return red + green + blue + (useAlpha ? NormailizedFloatToHex(color.a) : string.Empty);
        }

        public static void SpeedMeasurer(Action action)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            action?.Invoke();

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");
        }

        public static void SpeedMeasurer<T>(Action<T> action, T t)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            action?.Invoke(t);

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");
        }

        public static void SpeedMeasurer<T, U>(Action<T, U> action, T t, U u)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            action?.Invoke(t, u);

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");
        }

        public static void SpeedMeasurer<T, U, V>(Action<T, U, V> action, T t, U u, V v)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            action?.Invoke(t, u, v);

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");
        }

        public static T SpeedMeasurer<T>(Func<T> func)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            var result = func.Invoke();

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");

            return result;
        }

        public static U SpeedMeasurer<T, U>(Func<T, U> func, T t)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            var result = func.Invoke(t);

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");

            return result;
        }

        public static V SpeedMeasurer<T, U, V>(Func<T, U, V> func, T t, U u)
        {
            long before = GC.GetTotalMemory(false);
            var temp = Time.realtimeSinceStartup;

            var result = func.Invoke(t, u);

            long after = GC.GetTotalMemory(false);
            long diff = after - before;

            Debug.Log("Allocated bytes = " + diff.ToString() + 'B');
            Debug.Log(((Time.realtimeSinceStartup - temp) * 1000).ToString("f6") + "ms");

            return result;
        }
    }
}