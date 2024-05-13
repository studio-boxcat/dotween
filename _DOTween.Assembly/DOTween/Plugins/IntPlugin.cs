using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class IntPlugin : ABSTweenPlugin<int, NoOptions>
    {
        public static readonly IntPlugin Instance = new();

        public override void SetFrom(TweenerCore<int, int, NoOptions> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }

        public override void SetFrom(TweenerCore<int, int, NoOptions> t, int fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                var currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(fromValue);
        }

        public override void SetRelativeEndValue(TweenerCore<int, int, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<int, int, NoOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(NoOptions options, Tween t, DOGetter<int> getter, DOSetter<int> setter,
            float elapsed, int startValue, int changeValue, float duration
        )
        {
            var pos = DOTweenUtils.CalculatePosition(t, elapsed, duration);
            setter(Mathf.RoundToInt(startValue + changeValue * pos));
        }
    }
}