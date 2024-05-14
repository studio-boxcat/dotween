using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening
{
    public static class DOTweenModuleAudio
    {
        /// <summary>Tweens an AudioSource's volume to the given value.
        /// Also stores the AudioSource as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach (0 to 1)</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this AudioSource target, float endValue, float duration)
        {
            endValue = Mathf.Clamp01(endValue);
            var t = DOTween.To(() => target.volume, x => target.volume = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens an AudioSource's pitch to the given value.
        /// Also stores the AudioSource as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOPitch(this AudioSource target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.pitch, x => target.pitch = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
    }
}