#if !COMPATIBLE
// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/10 16:51
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class Vector2Plugin : ABSTweenPlugin<Vector2, VectorOptions>
    {
        public static Vector2Plugin Instance = new();

        public override void SetFrom(TweenerCore<Vector2, Vector2, VectorOptions> t, bool isRelative)
        {
            Vector2 prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            Vector2 to = t.endValue;
            switch (t.plugOptions.axisConstraint) {
            case AxisConstraint.X:
                to.x = t.startValue.x;
                break;
            case AxisConstraint.Y:
                to.y = t.startValue.y;
                break;
            default:
                to = t.startValue;
                break;
            }
            t.setter(to);
        }

        public override void SetFrom(TweenerCore<Vector2, Vector2, VectorOptions> t, Vector2 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative) {
                Vector2 currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) {
                Vector2 to;
                switch (t.plugOptions.axisConstraint) {
                case AxisConstraint.X:
                    to = t.getter();
                    to.x = fromValue.x;
                    break;
                case AxisConstraint.Y:
                    to = t.getter();
                    to.y = fromValue.y;
                    break;
                default:
                    to = fromValue;
                    break;
                }
                t.setter(to);
            }
        }

        public override void SetRelativeEndValue(TweenerCore<Vector2, Vector2, VectorOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Vector2, Vector2, VectorOptions> t)
        {
            switch (t.plugOptions.axisConstraint) {
            case AxisConstraint.X:
                t.changeValue = new Vector2(t.endValue.x - t.startValue.x, 0);
                break;
            case AxisConstraint.Y:
                t.changeValue = new Vector2(0, t.endValue.y - t.startValue.y);
                break;
            default:
                t.changeValue = t.endValue - t.startValue;
                break;
            }
        }

        public override void EvaluateAndApply(
            VectorOptions options, Tween t, DOGetter<Vector2> getter, DOSetter<Vector2> setter,
            float elapsed, Vector2 startValue, Vector2 changeValue, float duration
        )
        {
            var pos = DOTweenUtils.CalculatePosition(t, elapsed, duration);

            switch (options.axisConstraint) {
            case AxisConstraint.X:
                var resX = getter();
                resX.x = startValue.x + changeValue.x * pos;
                setter(resX);
                break;
            case AxisConstraint.Y:
                var resY = getter();
                resY.y = startValue.y + changeValue.y * pos;
                setter(resY);
                break;
            default:
                startValue.x += changeValue.x * pos;
                startValue.y += changeValue.y * pos;
                setter(startValue);
                break;
            }
        }
    }
}
#endif