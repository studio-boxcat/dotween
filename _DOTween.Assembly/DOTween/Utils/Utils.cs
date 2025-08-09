#nullable enable

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DG.Tweening
{
    public static class Utils
    {
        internal static void OnTweenCallback(this TweenCallback callback, Tween t)
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

        [MustUseReturnValue]
        public static CustomYieldInstruction? WhilePlaying(this Tweener tween)
        {
            if (tween.active is false)
            {
                L.W("[DOTween] WhilePlaying called on an inactive tween. This will return null.");
                return null;
            }

            return new TweenWhilePlayingInstruction(tween);
        }
    }
}