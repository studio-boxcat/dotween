// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/07/13

using UnityEngine;
using DG.Tweening.Core;

#pragma warning disable 1591
namespace DG.Tweening
{
    public static class DOTweenModuleSprite
    {
        /// <summary>Tweens a SpriteRenderer's color to the given value.
        /// Also stores the spriteRenderer as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color> DOColor(this SpriteRenderer target, Color endValue, float duration)
        {
            TweenerCore<Color> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's alpha color to the given value.
        /// Also stores the spriteRenderer as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this SpriteRenderer target, float endValue, float duration)
        {
            var t = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
    }
}