// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/28 10:40
//
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1573
namespace DG.Tweening
{
    /// <summary>
    /// Methods that extend known Unity objects and allow to directly create and control tweens from their instances
    /// </summary>
    public static class ShortcutExtensions
    {
        // ===================================================================================
        // CREATION SHORTCUTS ----------------------------------------------------------------

        #region Camera Shortcuts

        /// <summary>Tweens a Camera's <code>orthographicSize</code> to the given value.
        /// Also stores the camera as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOOrthoSize(this Camera target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.orthographicSize, x => target.orthographicSize = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Material Shortcuts

        /// <summary>Tweens a Material's color to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color> DOColor(this Material target, Color endValue, float duration)
        {
            var t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named color property to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="property">The name of the material property to tween (like _Tint or _SpecColor)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color> DOColor(this Material target, Color endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            var t = DOTween.To(() => target.GetColor(property), x => target.SetColor(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named color property with the given ID to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="propertyID">The ID of the material property to tween (also called nameID in Unity's manual)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color> DOColor(this Material target, Color endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                Debugger.LogMissingMaterialProperty(propertyID);
                return null;
            }
            var t = DOTween.To(() => target.GetColor(propertyID), x => target.SetColor(propertyID, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's alpha color to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this Material target, float endValue, float duration)
        {
            var t = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's alpha color to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="property">The name of the material property to tween (like _Tint or _SpecColor)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this Material target, float endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            var t = DOTween.ToAlpha(() => target.GetColor(property), x => target.SetColor(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's alpha color with the given ID to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="propertyID">The ID of the material property to tween (also called nameID in Unity's manual)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFade(this Material target, float endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                Debugger.LogMissingMaterialProperty(propertyID);
                return null;
            }
            var t = DOTween.ToAlpha(() => target.GetColor(propertyID), x => target.SetColor(propertyID, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's named float property to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="property">The name of the material property to tween</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFloat(this Material target, float endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            var t = DOTween.To(() => target.GetFloat(property), x => target.SetFloat(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named float property with the given ID to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="propertyID">The ID of the material property to tween (also called nameID in Unity's manual)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOFloat(this Material target, float endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                Debugger.LogMissingMaterialProperty(propertyID);
                return null;
            }
            var t = DOTween.To(() => target.GetFloat(propertyID), x => target.SetFloat(propertyID, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's texture offset to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOOffset(this Material target, Vector2 endValue, float duration)
        {
            var t = DOTween.To(() => target.mainTextureOffset, x => target.mainTextureOffset = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named texture offset property to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="property">The name of the material property to tween</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2> DOOffset(this Material target, Vector2 endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            var t = DOTween.To(() => target.GetTextureOffset(property), x => target.SetTextureOffset(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Transform Shortcuts

        /// <summary>Tweens a Transform's position to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOMove(this Transform target, Vector3 endValue, float duration)
        {
            var t = DOTween.To(() => target.position, x => target.position = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's X position to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOMoveX(this Transform target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.position, x => target.position = x, new Vector3(endValue, 0, 0), duration);
            t.SetOptions(AxisConstraint.X).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's Y position to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOMoveY(this Transform target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.position, x => target.position = x, new Vector3(0, endValue, 0), duration);
            t.SetOptions(AxisConstraint.Y).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localPosition to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOLocalMove(this Transform target, Vector3 endValue, float duration)
        {
            var t = DOTween.To(() => target.localPosition, x => target.localPosition = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's X localPosition to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOLocalMoveX(this Transform target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(endValue, 0, 0), duration);
            t.SetOptions(AxisConstraint.X).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's Y localPosition to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOLocalMoveY(this Transform target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(0, endValue, 0), duration);
            t.SetOptions(AxisConstraint.Y).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localRotation to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<float> DOLocalRotateZ(this Transform target, float endValue, float duration)
        {
            var t = DOTween.To
                (() => target.localEulerAngles.z,
                x => target.localEulerAngles = new Vector3(0, 0, x),
                endValue,
                duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localScale to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOScale(this Transform target, Vector3 endValue, float duration)
        {
            var t = DOTween.To(() => target.localScale, x => target.localScale = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localScale uniformly to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOScale(this Transform target, float endValue, float duration)
        {
            var endValueV3 = new Vector3(endValue, endValue, endValue);
            var t = DOTween.To(() => target.localScale, x => target.localScale = x, endValueV3, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's X localScale to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOScaleX(this Transform target, float endValue, float duration)
        {
            var t =  DOTween.To(() => target.localScale, x => target.localScale = x, new Vector3(endValue, 0, 0), duration);
            t.SetOptions(AxisConstraint.X)
                .SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's Y localScale to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3> DOScaleY(this Transform target, float endValue, float duration)
        {
            var t = DOTween.To(() => target.localScale, x => target.localScale = x, new Vector3(0, endValue, 0), duration);
            t.SetOptions(AxisConstraint.Y)
                .SetTarget(target);
            return t;
        }

        /// <summary>Punches a Transform's localPosition towards the given direction and then back to the starting one
        /// as if it was connected to the starting position via an elastic.</summary>
        /// <param name="punch">The direction and strength of the punch (added to the Transform's current position)</param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="vibrato">Indicates how much will the punch vibrate</param>
        /// <param name="elasticity">Represents how much (0 to 1) the vector will go beyond the starting position when bouncing backwards.
        /// 1 creates a full oscillation between the punch direction and the opposite direction,
        /// while 0 oscillates only between the punch and the start position</param>
        public static Tweener DOPunchPosition(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOPunchPosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Punch(() => target.localPosition, x => target.localPosition = x, punch, duration, vibrato, elasticity)
                .SetTarget(target);
        }
        /// <summary>Punches a Transform's localScale towards the given size and then back to the starting one
        /// as if it was connected to the starting scale via an elastic.</summary>
        /// <param name="punch">The punch strength (added to the Transform's current scale)</param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="vibrato">Indicates how much will the punch vibrate</param>
        /// <param name="elasticity">Represents how much (0 to 1) the vector will go beyond the starting size when bouncing backwards.
        /// 1 creates a full oscillation between the punch scale and the opposite scale,
        /// while 0 oscillates only between the punch scale and the start scale</param>
        public static Tweener DOPunchScale(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOPunchScale: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Punch(() => target.localScale, x => target.localScale = x, punch, duration, vibrato, elasticity)
                .SetTarget(target);
        }
        /// <summary>Punches a Transform's localRotation towards the given size and then back to the starting one
        /// as if it was connected to the starting rotation via an elastic.</summary>
        /// <param name="punch">The punch strength (added to the Transform's current rotation)</param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="vibrato">Indicates how much will the punch vibrate</param>
        /// <param name="elasticity">Represents how much (0 to 1) the vector will go beyond the starting rotation when bouncing backwards.
        /// 1 creates a full oscillation between the punch rotation and the opposite rotation,
        /// while 0 oscillates only between the punch and the start rotation</param>
        public static Tweener DOPunchRotation(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOPunchRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Punch(() => target.localEulerAngles, x => target.localRotation = Quaternion.Euler(x), punch, duration, vibrato, elasticity)
                .SetTarget(target);
        }

        /// <summary>Shakes a Transform's localPosition with the given values.</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakePosition(this Transform target, float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localPosition, x => target.localPosition = x, duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target);
        }
        /// <summary>Shakes a Transform's localPosition with the given values.</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakePosition(this Transform target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localPosition, x => target.localPosition = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target);
        }
        /// <summary>Shakes a Transform's localRotation.</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeRotation(this Transform target, float duration, float strength = 90, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOShakeRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localEulerAngles, x => target.localRotation = Quaternion.Euler(x), duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target);
        }
        /// <summary>Shakes a Transform's localRotation.</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeRotation(this Transform target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOShakeRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localEulerAngles, x => target.localRotation = Quaternion.Euler(x), duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target);
        }
        /// <summary>Shakes a Transform's localScale.</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeScale(this Transform target, float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOShakeScale: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localScale, x => target.localScale = x, duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target);
        }
        /// <summary>Shakes a Transform's localScale.</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeScale(this Transform target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOShakeScale: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localScale, x => target.localScale = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target);
        }

        #region Special

        /// <summary>Tweens a Transform's position to the given value, while also applying a jump effect along the Y axis.
        /// Returns a Sequence instead of a Tweener.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="jumpPower">Power of the jump (the max height of the jump is represented by this plus the final Y offset)</param>
        /// <param name="numJumps">Total number of jumps</param>
        /// <param name="duration">The duration of the tween</param>
        public static Sequence DOJump(this Transform target, Vector2 endValue, float jumpPower, int numJumps, float duration)
        {
            if (numJumps < 1) numJumps = 1;
            var startPosY = target.position.y; // Temporary fix for OnStart not being called when using Goto instead of GotoWithCallbacks
            float offsetY = -1;
            var offsetYSet = false;

            // Separate Y Tween so we can elaborate elapsedPercentage on that instead of on the Sequence
            // (in case users add a delay or other elements to the Sequence)
            var s = DOTween.Sequence();
            Tween yTween = DOTween.To(() => target.position, x => target.position = x, new Vector3(0, jumpPower, 0), duration / (numJumps * 2))
                .SetOptions(AxisConstraint.Y).SetEase(Ease.OutQuad).SetRelative()
                .SetLoops(numJumps * 2, LoopType.Yoyo)
                .OnStart(()=> startPosY = target.position.y); // FIXME not called if you only use Goto (and not GotoWithCallbacks)
            s.Append(DOTween.To(() => target.position, x => target.position = x, new Vector3(endValue.x, 0, 0), duration)
                    .SetOptions(AxisConstraint.X).SetEase(Ease.Linear)
                ).Join(yTween)
                .SetTarget(target).SetEase(Config.defaultEaseType);
            yTween.OnUpdate(() => {
                if (!offsetYSet) {
                    offsetYSet = true;
                    offsetY = s.isRelative ? endValue.y : endValue.y - startPosY;
                }
                var pos = target.position;
                pos.y += EaseManager.Evaluate(0, offsetY, yTween.ElapsedPercentage(), Ease.OutQuad);
                target.position = pos;
            });
            return s;
        }
        /// <summary>Tweens a Transform's localPosition to the given value, while also applying a jump effect along the Y axis.
        /// Returns a Sequence instead of a Tweener.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="jumpPower">Power of the jump (the max height of the jump is represented by this plus the final Y offset)</param>
        /// <param name="numJumps">Total number of jumps</param>
        /// <param name="duration">The duration of the tween</param>
        public static Sequence DOLocalJump(this Transform target, Vector3 endValue, float jumpPower, int numJumps, float duration)
        {
            if (numJumps < 1) numJumps = 1;
            var startPosY = target.localPosition.y; // Temporary fix for OnStart not being called when using Goto instead of GotoWithCallbacks
            float offsetY = -1;
            var offsetYSet = false;

            // Separate Y Tween so we can elaborate elapsedPercentage on that instead of on the Sequence
            // (in case users add a delay or other elements to the Sequence)
            var s = DOTween.Sequence();
            Tween yTween = DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(0, jumpPower, 0), duration / (numJumps * 2))
                .SetOptions(AxisConstraint.Y).SetEase(Ease.OutQuad).SetRelative()
                .SetLoops(numJumps * 2, LoopType.Yoyo)
                .OnStart(()=> startPosY = target.localPosition.y); // FIXME not called if you only use Goto (and not GotoWithCallbacks)
            s.Append(DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(endValue.x, 0, 0), duration)
                    .SetOptions(AxisConstraint.X).SetEase(Ease.Linear)
                ).Join(yTween)
                .SetTarget(target).SetEase(Config.defaultEaseType);
            yTween.OnUpdate(() => {
                if (!offsetYSet) {
                    offsetYSet = true;
                    offsetY = s.isRelative ? endValue.y : endValue.y - startPosY;
                }
                var pos = target.localPosition;
                pos.y += EaseManager.Evaluate(0, offsetY, yTween.ElapsedPercentage(), Ease.OutQuad);
                target.localPosition = pos;
            });
            return s;
        }

        #endregion

        #endregion

        #region Blendables

        #region Transform

        /// <summary>Tweens a Transform's position BY the given value (as if you chained a <code>SetRelative</code>),
        /// in a way that allows other DOBlendableMove tweens to work together on the same target,
        /// instead than fight each other as multiple DOMove would do.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="byValue">The value to tween by</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableMoveBy(this Transform target, Vector3 byValue, float duration)
        {
            var to = Vector3.zero;
            return DOTween.To(() => to, x => {
                var diff = x - to;
                to = x;
                target.position += diff;
            }, byValue, duration)
                .Blendable().SetTarget(target);
        }

        /// <summary>Tweens a Transform's localPosition BY the given value (as if you chained a <code>SetRelative</code>),
        /// in a way that allows other DOBlendableMove tweens to work together on the same target,
        /// instead than fight each other as multiple DOMove would do.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="byValue">The value to tween by</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableLocalMoveBy(this Transform target, Vector3 byValue, float duration)
        {
            var to = Vector3.zero;
            return DOTween.To(() => to, x => {
                var diff = x - to;
                to = x;
                target.localPosition += diff;
            }, byValue, duration)
                .Blendable().SetTarget(target);
        }

        // Added by Steve Streeting > https://github.com/sinbad
        /// <summary>Punches a Transform's localRotation BY the given value and then back to the starting one
        /// as if it was connected to the starting rotation via an elastic. Does it in a way that allows other
        /// DOBlendableRotate tweens to work together on the same target</summary>
        /// <param name="punch">The punch strength (added to the Transform's current rotation)</param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="vibrato">Indicates how much will the punch vibrate</param>
        /// <param name="elasticity">Represents how much (0 to 1) the vector will go beyond the starting rotation when bouncing backwards.
        /// 1 creates a full oscillation between the punch rotation and the opposite rotation,
        /// while 0 oscillates only between the punch and the start rotation</param>
        public static Tweener DOBlendablePunchRotation(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            if (duration <= 0) {
                Debug.LogWarning("DOBlendablePunchRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            var to = Vector3.zero;
            var t = DOTween.Punch(() => to, v => {
                var qto = Quaternion.Euler(to.x, to.y, to.z);
                var qnew = Quaternion.Euler(v.x, v.y, v.z);
                var diff = qnew * Quaternion.Inverse(qto);
                to = v;
                var currRot = target.rotation;
                target.rotation = currRot * Quaternion.Inverse(currRot) * diff * currRot;
            }, punch, duration, vibrato, elasticity)
                .Blendable().SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localScale BY the given value (as if you chained a <code>SetRelative</code>),
        /// in a way that allows other DOBlendableScale tweens to work together on the same target,
        /// instead than fight each other as multiple DOScale would do.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="byValue">The value to tween by</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableScaleBy(this Transform target, Vector3 byValue, float duration)
        {
            var to = Vector3.zero;
            return DOTween.To(() => to, x => {
                var diff = x - to;
                to = x;
                target.localScale += diff;
            }, byValue, duration)
                .Blendable().SetTarget(target);
        }

        #endregion

        #endregion

        // ===================================================================================
        // OPERATION SHORTCUTS ---------------------------------------------------------------

        #region Operation Shortcuts

        /// <summary>
        /// Completes all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens completed
        /// (meaning the tweens that don't have infinite loops and were not already complete)
        /// </summary>
        /// <param name="withCallbacks">For Sequences only: if TRUE also internal Sequence callbacks will be fired,
        /// otherwise they will be ignored</param>
        public static void DOComplete([NotNull] this Object target, bool withCallbacks = false)
            => TweenManager.ExecuteOperation(OperationType.Complete, target, false, withCallbacks ? 1 : 0);

        /// <summary>
        /// Kills all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens killed.
        /// </summary>
        /// <param name="complete">If TRUE completes the tween before killing it</param>
        public static void DOKill([NotNull] this object target, bool complete = false)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (target is null)
            {
                L.W("[DOTween] Object target is NULL");
                return;
            }

            if (complete) TweenManager.ExecuteOperation(OperationType.Complete, target, true, 0);
            TweenManager.ExecuteOperation(OperationType.Despawn, target, false, 0);
        }

        /// <summary>
        /// Flips the direction (backwards if it was going forward or viceversa) of all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens flipped.
        /// </summary>
        public static void DOFlip([NotNull] this Object target)
            => TweenManager.ExecuteOperation(OperationType.Flip, target, false, 0);

        /// <summary>
        /// Sends to the given position all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens involved.
        /// </summary>
        /// <param name="to">Time position to reach
        /// (if higher than the whole tween duration the tween will simply reach its end)</param>
        /// <param name="andPlay">If TRUE will play the tween after reaching the given position, otherwise it will pause it</param>
        public static void DOGoto([NotNull] this Object target, float to, bool andPlay = false)
            => TweenManager.ExecuteOperation(OperationType.Goto, target, andPlay, to);

        /// <summary>
        /// Pauses all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens paused.
        /// </summary>
        public static void DOPause([NotNull] this Object target)
            => TweenManager.ExecuteOperation(OperationType.Pause, target, false, 0);

        /// <summary>
        /// Plays all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static void DOPlay([NotNull] this Object target)
            => TweenManager.ExecuteOperation(OperationType.Play, target, false, 0);

        /// <summary>
        /// Plays backwards all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static void DOPlayBackwards([NotNull] this Object target)
            => TweenManager.ExecuteOperation(OperationType.PlayBackwards, target, false, 0);

        /// <summary>
        /// Plays forward all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static void DOPlayForward([NotNull] this Object target)
            => TweenManager.ExecuteOperation(OperationType.PlayForward, target, false, 0);

        /// <summary>
        /// Restarts all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens restarted.
        /// </summary>
        public static void DORestart([NotNull] this Object target, bool includeDelay = true)
            => TweenManager.ExecuteOperation(OperationType.Restart, target, includeDelay, -1);

        /// <summary>
        /// Rewinds all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens rewinded.
        /// </summary>
        public static void DORewind([NotNull] this Object target, bool includeDelay = true)
            => TweenManager.ExecuteOperation(OperationType.Rewind, target, includeDelay, 0);

        /// <summary>
        /// Toggles the paused state (plays if it was paused, pauses if it was playing) of all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens involved.
        /// </summary>
        public static void DOTogglePause([NotNull] this Object target)
            => TweenManager.ExecuteOperation(OperationType.TogglePause, target, false, 0);

        #endregion
    }
}