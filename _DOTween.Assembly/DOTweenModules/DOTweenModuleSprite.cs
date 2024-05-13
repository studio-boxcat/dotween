// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/07/13

using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

#pragma warning disable 1591
namespace DG.Tweening
{
    public static class DOTweenModuleSprite
    {
        /// <summary>Tweens a SpriteRenderer's color to the given value.
        /// Also stores the spriteRenderer as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, NoOptions> DOColor(this SpriteRenderer target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, NoOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's alpha color to the given value.
        /// Also stores the spriteRenderer as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float, float, NoOptions> DOFade(this SpriteRenderer target, float endValue, float duration)
        {
            var t = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a SpriteRenderer's color to the given value,
        /// in a way that allows other DOBlendableColor tweens to work together on the same target,
        /// instead than fight each other as multiple DOColor would do.
        /// Also stores the SpriteRenderer as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The value to tween to</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableColor(this SpriteRenderer target, Color endValue, float duration)
        {
            endValue = endValue - target.color;
            Color to = new Color(0, 0, 0, 0);
            return DOTween.To(() => to, x =>
                {
                    Color diff = x - to;
                    to = x;
                    target.color += diff;
                }, endValue, duration)
                .Blendable().SetTarget(target);
        }
    }
}