using DG.Tweening.Core.Easing;
using JetBrains.Annotations;
using UnityEngine;

namespace DG.Tweening.Core
{
    /// <summary>
    /// Various utils
    /// </summary>
    static class DOTweenUtils
    {
        /// <summary>
        /// Returns a Vector3 with z = 0
        /// </summary>
        internal static Vector3 Vector3FromAngle(float degrees, float magnitude)
        {
            var radians = degrees * Mathf.Deg2Rad;
            return new Vector3(magnitude * Mathf.Cos(radians), magnitude * Mathf.Sin(radians), 0);
        }

        [MustUseReturnValue]
        public static float CalculatePosition(Tween t, float elapsed, float duration)
        {
            var eval = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
            var loopCount = CountIncremental(t);
            return eval + loopCount;

            static int CountIncremental(Tween t)
            {
                var loopCount = 0;

                if (t.loopType == LoopType.Incremental)
                    loopCount += t.isComplete ? t.completedLoops - 1 : t.completedLoops;

                if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
                {
                    var seq = t.sequenceParent;
                    loopCount += (t.loopType == LoopType.Incremental ? t.loops : 1)
                                 * (seq.isComplete ? seq.completedLoops - 1 : seq.completedLoops);
                }

                return loopCount;
            }
        }
    }
}