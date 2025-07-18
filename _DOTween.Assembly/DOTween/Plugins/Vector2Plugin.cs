#if !COMPATIBLE
// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/10 16:51
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

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

        public override void SetFrom(Tweener<Vector2> t, Vector2 fromValue, bool setImmediately, bool isRelative)
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
                Vector2 to;
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

        public override void SetRelativeEndValue(Tweener<Vector2> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(Tweener<Vector2> t)
        {
            if (VectorOptions.GetAxisConstraints(t.plugOptions, out var x, out var y))
            {
                t.changeValue.x = x ? t.endValue.x - t.startValue.x : 0;
                t.changeValue.y = y ? t.endValue.y - t.startValue.y : 0;
            }
            else
            {
                t.changeValue = t.endValue - t.startValue;
            }
        }

        public override void EvaluateAndApply(Tweener<Vector2> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
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

    public class VectorOptions
    {
        public AxisConstraint axisConstraint;

        public static bool GetAxisConstraints(object? opts, out bool x, out bool y)
        {
            x = y = false;
            if (opts is null) return false;

            var constraint = ((VectorOptions) opts).axisConstraint;
            if ((constraint & AxisConstraint.X) is AxisConstraint.X)
                x = true;
            if ((constraint & AxisConstraint.Y) is AxisConstraint.Y)
                y = true;
            return x || y;
        }

        public static void SetAxisConstraint(Tweener<Vector2> t, AxisConstraint axisConstraint)
        {
            var o = t.plugOptions;
            if (o is null)
            {
                t.plugOptions = new VectorOptions { axisConstraint = axisConstraint };
                return;
            }

            var opts = (VectorOptions) o;
            opts.axisConstraint = axisConstraint;
        }

        public static void SetAxisConstraint(Tweener<Vector3> t, AxisConstraint axisConstraint)
        {
            var o = t.plugOptions;
            if (o is null)
            {
                t.plugOptions = new VectorOptions { axisConstraint = axisConstraint };
                return;
            }

            var opts = (VectorOptions) o;
            opts.axisConstraint = axisConstraint;
        }
    }
}
#endif