// ReSharper disable InconsistentNaming
#nullable enable
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening
{
    public class Vector2Plugin : TweenPlugin<Vector2>
    {
        public static readonly Vector2Plugin Instance = new();

        public override void SetFrom(Tweener<Vector2> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter!();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter!(VectorOptions.Composite(baseValue: t.endValue, overlayValue: t.startValue, t.plugOptions));
        }

        public override void SetFrom(Tweener<Vector2> t, Vector2 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                var currVal = t.getter!();
                t.endValue += currVal;
                fromValue += currVal;
            }

            t.startValue = fromValue;
            if (setImmediately)
                t.setter!(VectorOptions.Composite(baseValue: t.getter!(), overlayValue: fromValue, t.plugOptions));
        }

        public override void SetRelativeEndValue(Tweener<Vector2> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(Tweener<Vector2> t)
        {
            VectorOptions.GetControlAxis(t.plugOptions, out var x, out var y);
            t.changeValue.x = x ? t.endValue.x - t.startValue.x : 0;
            t.changeValue.y = y ? t.endValue.y - t.startValue.y : 0;
        }

        public override void EvaluateAndApply(Tweener<Vector2> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
            VectorOptions.GetControlAxis(t.plugOptions, out var x, out var y);
            var value = t.getter!();
            if (x) value.x = t.startValue.x + t.changeValue.x * pos;
            if (y) value.y = t.startValue.y + t.changeValue.y * pos;
            t.setter!(value);
        }
    }

    public class VectorOptions
    {
        public static readonly VectorOptions ControlX = new(x: true, y: false);
        public static readonly VectorOptions ControlY = new(x: false, y: true);

        private readonly bool x; // controls x
        private readonly bool y; // controls y

        private VectorOptions(bool x, bool y)
        {
            this.x = x;
            this.y = y;
        }

        public static void GetControlAxis(object? opts, out bool x, out bool y)
        {
            x = y = true;
            if (opts is null) return;

            var o = (VectorOptions) opts;
            x = o.x;
            y = o.y;
        }

        public static Vector2 Composite(Vector2 baseValue, Vector2 overlayValue, object? opts)
        {
            if (opts is null) return overlayValue;
            var o = (VectorOptions) opts;
            if (o.x) baseValue.x = overlayValue.x;
            if (o.y) baseValue.y = overlayValue.y;
            return baseValue;
        }
    }
}