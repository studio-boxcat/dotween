using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    /// <summary>
    /// Public so it can be used by lose scripts related to DOTween (like DOTweenAnimation)
    /// </summary>
    public static class Debugger
    {
        const string _prefix = "[DOTween] ";

        #region Public Methods

        [Conditional("DEBUG")]
        public static void LogWarning(object message, Tween t = null)
        {
            Debug.LogWarning(_prefix + GetDebugDataMessage(t) + message, t?.target as Object);
        }

        public static void LogError(object message, Tween t = null)
        {
            Debug.LogError(_prefix + GetDebugDataMessage(t) + message, t?.target as Object);
        }

        [Conditional("DEBUG")]
        public static void LogSafeModeCapturedError(Exception e, Tween t = null)
        {
            Debug.LogWarning($"{_prefix}Exception in DOTween SafeMode ({GetDebugDataMessage(t)})\n{e.Message} at:\n{e.StackTrace}", t?.target as Object);
        }

        [Conditional("DEBUG")]
        public static void LogInvalidTween(Tween t)
        {
            LogWarning("This Tween has been killed and is now invalid");
        }

        [Conditional("DEBUG")]
        public static void LogNestedTween(Tween t)
        {
            LogWarning("This Tween was added to a Sequence and can't be controlled directly", t);
        }

        [Conditional("DEBUG")]
        public static void LogNullTween(Tween t)
        {
            LogWarning("Null Tween");
        }

        public static void LogMissingMaterialProperty(string propertyName)
        {
            LogError($"This material doesn't have a {propertyName} property");
        }

        public static void LogMissingMaterialProperty(int propertyId)
        {
            LogError($"This material doesn't have a {propertyId} property ID");
        }

        #endregion

        #region Methods

        static string GetDebugDataMessage(Tween t)
        {
            return t?.ToString() ?? "null";
        }

        #endregion
    }
}