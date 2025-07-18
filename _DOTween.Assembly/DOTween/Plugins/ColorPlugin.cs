using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening
{
    public class ColorPlugin : TweenPlugin<Color>
    {
        public static readonly ColorPlugin Instance = new();

        public override void SetFrom(Tweener<Color> t, bool isRelative)
        {
            Assert.IsFalse(isRelative, "Color tweens cannot be relative");
            t.endValue = t.getter();
            t.startValue = t.endValue;
            t.setter(t.startValue);
        }

        public override void SetFrom(Tweener<Color> t, Color fromValue, bool setImmediately, bool isRelative)
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

        public override void SetRelativeEndValue(Tweener<Color> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(Tweener<Color> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(Tweener<Color> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
            t.setter(t.startValue + t.changeValue * pos);
        }
    }
}