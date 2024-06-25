// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/07/13

#if true && (UNITY_4_6 || UNITY_5 || UNITY_2017_1_OR_NEWER) // MODULE_MARKER

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening.Core;

#pragma warning disable 1591
namespace DG.Tweening
{
	public static class DOTweenModuleUI
    {
        #region Shortcuts

        #region CanvasGroup

        /// <summary>Tweens a CanvasGroup's alpha color to the given value.
        /// Also stores the canvasGroup as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this CanvasGroup target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.alpha, x => target.alpha = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Graphic

        /// <summary>Tweens an Graphic's color to the given value.
        /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color> DOColor(this Graphic target, Color endValue, float duration)
        {
            var t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens an Graphic's alpha color to the given value.
        /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this Graphic target, float endValue, float duration)
        {
            var t = DOTween.To(
                () => target.color.a,
                a =>
                {
                    var c = target.color;
                    c.a = a;
                    target.color = c;
                },
                endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Image

        /// <summary>Tweens an Image's fillAmount to the given value.
        /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach (0 to 1)</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFillAmount(this Image target, float endValue, float duration)
        {
            if (endValue > 1) endValue = 1;
            else if (endValue < 0) endValue = 0;
            TweenerCore<float> t = DOTween.To(() => target.fillAmount, x => target.fillAmount = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Shadow

        /// <summary>Tweens a Shadow's effectColor to the given value.
        /// Also stores the Shadow as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color> DOColor(this Shadow target, Color endValue, float duration)
        {
            TweenerCore<Color> t = DOTween.To(() => target.effectColor, x => target.effectColor = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Shadow's effectColor alpha to the given value.
        /// Also stores the Shadow as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this Shadow target, float endValue, float duration)
        {
            var t = DOTween.ToAlpha(() => target.effectColor, x => target.effectColor = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region RectTransform

        /// <summary>Tweens a RectTransform's anchoredPosition to the given value.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOAnchorPos(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a RectTransform's anchoredPosition X to the given value.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOAnchorPosX(this RectTransform target, float endValue, float duration)
        {
            TweenerCore<Vector2> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(endValue, 0), duration);
            t.SetOptions(AxisConstraint.X).SetTarget(target);
            return t;
        }
        /// <summary>Tweens a RectTransform's anchoredPosition Y to the given value.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOAnchorPosY(this RectTransform target, float endValue, float duration)
        {
            TweenerCore<Vector2> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, endValue), duration);
            t.SetOptions(AxisConstraint.Y).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a RectTransform's anchorMax to the given value.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOAnchorMax(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2> t = DOTween.To(() => target.anchorMax, x => target.anchorMax = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a RectTransform's anchorMin to the given value.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOAnchorMin(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2> t = DOTween.To(() => target.anchorMin, x => target.anchorMin = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a RectTransform's sizeDelta to the given value.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOSizeDelta(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2> t = DOTween.To(() => target.sizeDelta, x => target.sizeDelta = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Punches a RectTransform's anchoredPosition towards the given direction and then back to the starting one
        /// as if it was connected to the starting position via an elastic.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="punch">The direction and strength of the punch (added to the RectTransform's current position)</param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="vibrato">Indicates how much will the punch vibrate</param>
        /// <param name="elasticity">Represents how much (0 to 1) the vector will go beyond the starting position when bouncing backwards.
        /// 1 creates a full oscillation between the punch direction and the opposite direction,
        /// while 0 oscillates only between the punch and the start position</param>
        public static Tweener DOPunchAnchorPos(this RectTransform target, Vector2 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            return DOTween.Punch(() => target.anchoredPosition, x => target.anchoredPosition = x, punch, duration, vibrato, elasticity)
                .SetTarget(target);
        }

        /// <summary>Shakes a RectTransform's anchoredPosition with the given values.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). 
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeAnchorPos(this RectTransform target, float duration, float strength = 100, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            return DOTween.Shake(() => target.anchoredPosition, x => target.anchoredPosition = x, duration, strength, vibrato, randomness, true, fadeOut, randomnessMode)
                .SetTarget(target);
        }
        /// <summary>Shakes a RectTransform's anchoredPosition with the given values.
        /// Also stores the RectTransform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). 
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeAnchorPos(this RectTransform target, float duration, Vector2 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            return DOTween.Shake(() => target.anchoredPosition, x => target.anchoredPosition = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target);
        }

        #endregion

        #region ScrollRect

        /// <summary>Tweens a ScrollRect's horizontal/verticalNormalizedPosition to the given value.
        /// Also stores the ScrollRect as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static Tweener DONormalizedPos(this ScrollRect target, Vector2 endValue, float duration)
        {
            return DOTween.To(() => new Vector2(target.horizontalNormalizedPosition, target.verticalNormalizedPosition),
                x => {
                    target.horizontalNormalizedPosition = x.x;
                    target.verticalNormalizedPosition = x.y;
                }, endValue, duration)
                .SetTarget(target);
        }
        /// <summary>Tweens a ScrollRect's horizontalNormalizedPosition to the given value.
        /// Also stores the ScrollRect as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static Tweener DOHorizontalNormalizedPos(this ScrollRect target, float endValue, float duration)
        {
            return DOTween.To(() => target.horizontalNormalizedPosition, x => target.horizontalNormalizedPosition = x, endValue, duration)
                .SetTarget(target);
        }
        /// <summary>Tweens a ScrollRect's verticalNormalizedPosition to the given value.
        /// Also stores the ScrollRect as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static Tweener DOVerticalNormalizedPos(this ScrollRect target, float endValue, float duration)
        {
            return DOTween.To(() => target.verticalNormalizedPosition, x => target.verticalNormalizedPosition = x, endValue, duration)
                .SetTarget(target);
        }

        #endregion

        #region Slider

        /// <summary>Tweens a Slider's value to the given value.
        /// Also stores the Slider as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOValue(this Slider target, float endValue, float duration)
        {
            TweenerCore<float> t = DOTween.To(() => target.value, x => target.value = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #endregion
	}
}
#endif
