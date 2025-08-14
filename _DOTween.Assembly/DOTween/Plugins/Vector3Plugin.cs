using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening
{
    public class Vector3Plugin : TweenPlugin<Vector3>
    {
        public static readonly Vector3Plugin Instance = new();

        public override void SetFrom(Tweener<Vector3> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter!();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter!(VectorOptions.Composite(t.endValue, t.startValue, t.plugOptions));
        }

        public override void SetFrom(Tweener<Vector3> t, Vector3 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                var currVal = t.getter!();
                t.endValue += currVal;
                fromValue += currVal;
            }

            t.startValue = fromValue;
            if (setImmediately)
                t.setter!(VectorOptions.Composite(t.getter!(), fromValue, t.plugOptions));
        }

        public override void SetRelativeEndValue(Tweener<Vector3> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(Tweener<Vector3> t)
        {
            VectorOptions.GetControlAxis(t.plugOptions, out var x, out var y);
            t.changeValue.x = x ? t.endValue.x - t.startValue.x : 0;
            t.changeValue.y = y ? t.endValue.y - t.startValue.y : 0;
            t.changeValue.z = 0;
        }

        public override void EvaluateAndApply(Tweener<Vector3> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
            VectorOptions.GetControlAxis(t.plugOptions, out var x, out var y);
            var value = t.getter!();
            if (x) value.x = t.startValue.x + t.changeValue.x * pos;
            if (y) value.y = t.startValue.y + t.changeValue.y * pos;
            t.setter!(value);
        }
    }
}