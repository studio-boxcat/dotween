using DG.Tweening.Core.Easing;
using JetBrains.Annotations;

namespace DG.Tweening.Core
{
    /// <summary>
    /// Various utils
    /// </summary>
    static class DOTweenUtils
    {
        [MustUseReturnValue]
        public static float CalculateCumulativePosition(Tween t, bool useInversePosition)
        {
            var elapsed = useInversePosition ? t.duration - t.position : t.position;
            var pos = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, t.duration, t.easeOvershootOrAmplitude, t.easePeriod);
            var loopCount = CountIncremental(t);
            return pos + loopCount;

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