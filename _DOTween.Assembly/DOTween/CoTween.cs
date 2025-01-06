using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening
{
    public static class CoTween
    {
        public static void CoFade(this Graphic target, float duration, float startValue, float endValue)
        {
            L.I($"[CoTween] Fade: {target}, duration={duration} start={startValue} end={endValue}");

            if (target.isActiveAndEnabled is false)
            {
                L.W("[CoTween] target is not active", target);
                return;
            }

            if (duration is 0)
            {
                L.E("[CoTween] duration is 0", target);
                return;
            }

            target.StartCoroutine(CoTween(target, duration, startValue, endValue));
            return;

            static IEnumerator CoTween(Graphic target, float duration, float startValue, float endValue)
            {
                var startTime = Time.time;

                SetAlpha(target, startValue); // Set initial value
                yield return null;

                while (true)
                {
                    var now = Time.time;
                    var elapsed = now - startTime;

                    if (elapsed >= duration) // End of tween
                    {
                        // L.I($"[CoTween] CoFade: {target}, value={endValue} (End)");
                        SetAlpha(target, endValue);
                        break; // End of tween
                    }

                    // Calculate progress and value
                    var progress = elapsed / duration;
                    var lerpFactor = Evaluate(Config.defaultEaseType, progress, Config.defaultEaseOvershootOrAmplitude);
                    var value = Mathf.Lerp(startValue, endValue, lerpFactor);

                    // Apply value
                    // L.I($"[CoTween] CoFade: {target}, value={value}, progress={progress}");
                    SetAlpha(target, value);
                    yield return null; // Wait for next frame
                }
            }

            static void SetAlpha(Graphic target, float alpha)
            {
                var c = target.color;
                c.a = alpha;
                target.color = c;
            }
        }

        // Simplified version of DG.Tweening.Core.Easing.EaseManager.Evaluate
        private static float Evaluate(Ease easeType, float p, float overshoot)
        {
            const float piOver2 = Mathf.PI * 0.5f;

            if (p is 0) return 0;
            if (p is 1) return 1;

            return easeType switch
            {
                Ease.Linear => p,
                Ease.InSine => -(float) Math.Cos(p * piOver2) + 1,
                Ease.OutSine => (float) Math.Sin(p * piOver2),
                Ease.InOutSine => -0.5f * ((float) Math.Cos(Mathf.PI * p) - 1),
                Ease.InQuad => p * p,
                Ease.OutQuad => -p * (p - 2),
                Ease.InOutQuad => p * 0.5f < 1
                    ? 0.5f * p * p
                    : -0.5f * (--p * (p - 2) - 1),
                Ease.InCubic => p * p * p,
                Ease.OutCubic => (p - 1) * p * p + 1,
                Ease.InOutCubic => p * 0.5f < 1
                    ? 0.5f * p * p * p
                    : 0.5f * ((p -= 2) * p * p + 2),
                Ease.InQuart => p * p * p * p,
                Ease.OutQuart => -((p - 1) * p * p * p - 1),
                Ease.InOutQuart => p * 0.5f < 1
                    ? 0.5f * p * p * p * p
                    : -0.5f * ((p -= 2) * p * p * p - 2),
                Ease.InQuint => p * p * p * p * p,
                Ease.OutQuint => (p - 1) * p * p * p * p + 1,
                Ease.InOutQuint => p * 0.5f < 1
                    ? 0.5f * p * p * p * p * p
                    : 0.5f * ((p -= 2) * p * p * p * p + 2),
                Ease.InExpo => (float) Math.Pow(2, 10 * (p - 1)),
                Ease.OutExpo => -(float) Math.Pow(2, -10 * p) + 1,
                Ease.InOutExpo => p * 0.5f < 1
                    ? 0.5f * (float) Math.Pow(2, 10 * (p - 1))
                    : 0.5f * (-(float) Math.Pow(2, -10 * --p) + 2),
                Ease.InCirc => -((float) Math.Sqrt(1 - p * p) - 1),
                Ease.OutCirc => (float) Math.Sqrt(1 - (p - 1) * p),
                Ease.InOutCirc => p * 0.5f < 1
                    ? -0.5f * ((float) Math.Sqrt(1 - p * p) - 1)
                    : 0.5f * ((float) Math.Sqrt(1 - (p -= 2) * p) + 1),
                Ease.InBack => p * p * ((overshoot + 1) * p - overshoot),
                Ease.OutBack => (p - 1) * p * ((overshoot + 1) * p + overshoot) + 1,
                Ease.InOutBack => p * 0.5f < 1
                    ? 0.5f * (p * p * (((overshoot *= 1.525f) + 1) * p - overshoot))
                    : 0.5f * ((p -= 2) * p * (((overshoot *= 1.525f) + 1) * p + overshoot) + 2),
                _ => throw new ArgumentOutOfRangeException(nameof(easeType), easeType, null)
            };
        }
    }
}