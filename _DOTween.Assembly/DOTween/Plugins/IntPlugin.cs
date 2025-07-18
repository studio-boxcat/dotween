using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening
{
    public class IntPlugin : TweenPlugin<int>
    {
        public static readonly IntPlugin Instance = new();

        public override void SetFrom(Tweener<int> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }

        public override void SetFrom(Tweener<int> t, int fromValue, bool setImmediately, bool isRelative)
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

        public override void SetRelativeEndValue(Tweener<int> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(Tweener<int> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(Tweener<int> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
            t.setter(Mathf.RoundToInt(t.startValue + t.changeValue * pos));
        }
    }
}