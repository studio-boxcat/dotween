using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class IntPlugin : TweenPlugin<int>
    {
        public static readonly IntPlugin Instance = new();

        public override void SetFrom(TweenerCore<int> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }

        public override void SetFrom(TweenerCore<int> t, int fromValue, bool setImmediately, bool isRelative)
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

        public override void SetRelativeEndValue(TweenerCore<int> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<int> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(TweenerCore<int> t, bool useInversePosition)
        {
            var pos = DOTweenUtils.CalculateCumulativePosition(t, useInversePosition);
            t.setter(Mathf.RoundToInt(t.startValue + t.changeValue * pos));
        }
    }
}