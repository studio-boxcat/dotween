using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class Vector3Plugin : ABSTweenPlugin<Vector3,VectorOptions>
    {
        public static readonly Vector3Plugin Instance = new();

        public override void SetFrom(TweenerCore<Vector3, Vector3, VectorOptions> t, bool isRelative)
        {
            Vector3 prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            Vector3 to = t.endValue;
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

        public override void SetFrom(TweenerCore<Vector3, Vector3, VectorOptions> t, Vector3 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative) {
                Vector3 currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) {
                Vector3 to;
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

        public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3, VectorOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Vector3, Vector3, VectorOptions> t)
        {
            switch (t.plugOptions.axisConstraint) {
            case AxisConstraint.X:
                t.changeValue = new Vector3(t.endValue.x - t.startValue.x, 0, 0);
                break;
            case AxisConstraint.Y:
                t.changeValue = new Vector3(0, t.endValue.y - t.startValue.y, 0);
                break;
            default:
                t.changeValue = t.endValue - t.startValue;
                break;
            }
        }

        public override void EvaluateAndApply(
            VectorOptions options, Tween t, DOGetter<Vector3> getter, DOSetter<Vector3> setter,
            float elapsed, Vector3 startValue, Vector3 changeValue, float duration
        ){
            var pos = DOTweenUtils.CalculatePosition(t, elapsed, duration);

            switch (options.axisConstraint) {
            case AxisConstraint.X:
                Vector3 resX = getter();
                resX.x = startValue.x + changeValue.x * pos;
                setter(resX);
                break;
            case AxisConstraint.Y:
                Vector3 resY = getter();
                resY.y = startValue.y + changeValue.y * pos;
                setter(resY);
                break;
            default:
                startValue.x += changeValue.x * pos;
                startValue.y += changeValue.y * pos;
                startValue.z += changeValue.z * pos;
                setter(startValue);
                break;
            }
        }
    }
}