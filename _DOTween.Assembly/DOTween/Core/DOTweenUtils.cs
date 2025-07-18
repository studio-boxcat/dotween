using JetBrains.Annotations;

namespace DG.Tweening
{
    /// <summary>
    /// Various utils
    /// </summary>
    internal static class DOTweenUtils
    {
        [MustUseReturnValue]
        public static float Evaluate(Tween t, float elapsed)
        {
            return EaseManager.Evaluate(t.easeType, t.customEase, elapsed, t.duration, t.easeOvershootOrAmplitude, t.easePeriod);
        }
    }
}