#pragma warning disable 1591
namespace DG.Tweening
{
    public class FloatPlugin : TweenPlugin<float>
    {
        public static readonly FloatPlugin Instance = new();

        public override void SetFrom(Tweener<float> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }

        public override void SetFrom(Tweener<float> t, float fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                float currVal = t.getter!();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter!(fromValue);
        }

        public override void SetRelativeEndValue(Tweener<float> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(Tweener<float> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(Tweener<float> t, float elapsed)
        {
            var pos = DOTweenUtils.Evaluate(t, elapsed);
            t.setter(t.startValue + t.changeValue * pos);
        }
    }
}