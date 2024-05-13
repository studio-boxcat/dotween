using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace DG.Tweening
{
    static class L
    {
        [Conditional("DEBUG")]
        public static void I(string message, Object context = null) => Debug.Log(message, context);
        [Conditional("DEBUG")]
        public static void W(string message, Object context = null) => Debug.LogWarning(message, context);
        [Conditional("DEBUG")]
        public static void E(string message, Object context = null) => Debug.LogError(message, context);
        [Conditional("DEBUG")]
        public static void E(Exception e, Object context = null) => Debug.LogException(e, context);
    }
}