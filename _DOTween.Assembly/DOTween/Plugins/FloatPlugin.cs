using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class FloatPlugin : ABSTweenPlugin<float, NoOptions>
    {
        public static readonly FloatPlugin Instance = new();

        public override void SetFrom(TweenerCore<float, float, NoOptions> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }

        public override void SetFrom(TweenerCore<float, float, NoOptions> t, float fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                float currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(fromValue);
        }

        public override void SetRelativeEndValue(TweenerCore<float, float, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<float, float, NoOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(
            NoOptions options, Tween t, DOGetter<float> getter, DOSetter<float> setter,
            float elapsed, float startValue, float changeValue, float duration
        )
        {
            var pos = DOTweenUtils.CalculatePosition(t, elapsed, duration);
            setter(startValue + changeValue * pos);
        }
    }
}