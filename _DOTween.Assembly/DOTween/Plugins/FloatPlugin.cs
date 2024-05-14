using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class FloatPlugin : TweenPlugin<float>
    {
        public static readonly FloatPlugin Instance = new();

        public override void SetFrom(TweenerCore<float> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }

        public override void SetFrom(TweenerCore<float> t, float fromValue, bool setImmediately, bool isRelative)
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

        public override void SetRelativeEndValue(TweenerCore<float> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<float> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(TweenerCore<float> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
            t.setter(t.startValue + t.changeValue * pos);
        }
    }
}