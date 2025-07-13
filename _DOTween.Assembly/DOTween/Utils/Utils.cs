#nullable enable

using System;

namespace DG.Tweening
{
    internal static class Utils
    {
        public static void OnTweenCallback(this TweenCallback callback, Tween t)
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debugger.LogSafeModeCapturedError(e, t);
            }
        }
    }
}