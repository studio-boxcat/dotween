using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class Vector3Plugin : ABSTweenPlugin<Vector3>
    {
        public static readonly Vector3Plugin Instance = new();

        public override void SetFrom(TweenerCore<Vector3, Vector3> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            var to = t.endValue;
            if (VectorOptions.GetAxisConstraints(t.plugOptions, out var x, out var y))
            {
                if (x) to.x = t.startValue.x;
                if (y) to.y = t.startValue.y;
            }
            else
            {
                to = t.startValue;
            }
            t.setter(to);
        }

        public override void SetFrom(TweenerCore<Vector3, Vector3> t, Vector3 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                var currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }

            t.startValue = fromValue;
            if (setImmediately)
            {
                Vector3 to;
                if (VectorOptions.GetAxisConstraints(t.plugOptions, out var x, out var y))
                {
                    to = t.getter();
                    if (x) to.x = fromValue.x;
                    if (y) to.y = fromValue.y;
                }
                else
                {
                    to = fromValue;
                }
                t.setter(to);
            }
        }

        public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Vector3, Vector3> t)
        {
            if (VectorOptions.GetAxisConstraints(t.plugOptions, out var x, out var y))
            {
                t.changeValue.x = x ? t.endValue.x - t.startValue.x : 0;
                t.changeValue.y = y ? t.endValue.y - t.startValue.y : 0;
                t.changeValue.z = 0;
            }
            else
            {
                t.changeValue = t.endValue - t.startValue;
            }
        }

        public override void EvaluateAndApply(TweenerCore<Vector3, Vector3> t, bool useInversePosition)
        {
            var pos = DOTweenUtils.CalculateCumulativePosition(t, useInversePosition);
            if (VectorOptions.GetAxisConstraints(t.plugOptions, out var x, out var y))
            {
                var value = t.getter();
                if (x) value.x = t.startValue.x + t.changeValue.x * pos;
                if (y) value.y = t.startValue.y + t.changeValue.y * pos;
                t.setter(value);
            }
            else
            {
                t.setter(t.startValue + t.changeValue * pos);
            }
        }
    }
}