using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class ColorPlugin : ABSTweenPlugin<Color, NoOptions>
    {
        public static readonly ColorPlugin Instance = new();

        public override void SetFrom(TweenerCore<Color, Color, NoOptions> t, bool isRelative)
        {
            Assert.IsFalse(isRelative, "Color tweens cannot be relative");
            t.endValue = t.getter();
            t.startValue = t.endValue;
            t.setter(t.startValue);
        }

        public override void SetFrom(TweenerCore<Color, Color, NoOptions> t, Color fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                var currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately)
                t.setter(fromValue);
        }

        public override void SetRelativeEndValue(TweenerCore<Color, Color, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Color, Color, NoOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(
            NoOptions options, Tween t, DOGetter<Color> getter, DOSetter<Color> setter,
            float elapsed, Color startValue, Color changeValue, float duration
        )
        {
            var pos = DOTweenUtils.CalculatePosition(t, elapsed, duration);
            startValue.r += changeValue.r * pos;
            startValue.g += changeValue.g * pos;
            startValue.b += changeValue.b * pos;
            startValue.a += changeValue.a * pos;
            setter(startValue);
        }
    }
}