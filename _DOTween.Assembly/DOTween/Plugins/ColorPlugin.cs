using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class ColorPlugin : TweenPlugin<Color>
    {
        public static readonly ColorPlugin Instance = new();

        public override void SetFrom(TweenerCore<Color> t, bool isRelative)
        {
            Assert.IsFalse(isRelative, "Color tweens cannot be relative");
            t.endValue = t.getter();
            t.startValue = t.endValue;
            t.setter(t.startValue);
        }

        public override void SetFrom(TweenerCore<Color> t, Color fromValue, bool setImmediately, bool isRelative)
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

        public override void SetRelativeEndValue(TweenerCore<Color> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Color> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(TweenerCore<Color> t, bool useInversePosition)
        {
            var pos = DOTweenUtils.CalculateCumulativePosition(t, useInversePosition);
            t.setter(t.startValue + t.changeValue * pos);
        }
    }
}