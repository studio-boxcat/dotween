using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DG.Tweening
{
    public static class Logger
    {
        [Conditional("DEBUG")]
        public static void Warning(string message) => Debug.LogWarning(message);

        [Conditional("DEBUG")]
        public static void Warning(string message, Object context) => Debug.LogWarning(message, context);
    }
}