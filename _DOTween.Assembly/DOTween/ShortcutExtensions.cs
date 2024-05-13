// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/28 10:40
//
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DOVector3 = UnityEngine.Vector3;
using DOQuaternion = UnityEngine.Quaternion;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.CustomPlugins;
using DG.Tweening.Plugins.Options;
using UnityEngine;

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
        public static TweenerCore<float, float, NoOptions> DOOrthoSize(this Camera target, float endValue, float duration)
        {
            TweenerCore<float, float, NoOptions> t = DOTween.To(() => target.orthographicSize, x => target.orthographicSize = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Shakes a Camera's localPosition along its relative X Y axes with the given values.
        /// Also stores the camera as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). 
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakePosition(this Camera target, float duration, float strength = 3, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.transform.localPosition, x => target.transform.localPosition = x, duration, strength, vibrato, randomness, true, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetCameraShakePosition);
        }
        /// <summary>Shakes a Camera's localPosition along its relative X Y axes with the given values.
        /// Also stores the camera as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). 
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakePosition(this Camera target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.transform.localPosition, x => target.transform.localPosition = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetCameraShakePosition);
        }

        /// <summary>Shakes a Camera's localRotation.
        /// Also stores the camera as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). 
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeRotation(this Camera target, float duration, float strength = 90, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakeRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.transform.localEulerAngles, x => target.transform.localRotation = Quaternion.Euler(x), duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
        }
        /// <summary>Shakes a Camera's localRotation.
        /// Also stores the camera as the tween's target so it can be used for filtered operations</summary>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). 
        /// Setting it to 0 will shake along a single direction.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static Tweener DOShakeRotation(this Camera target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            if (duration <= 0) {
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakeRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.transform.localEulerAngles, x => target.transform.localRotation = Quaternion.Euler(x), duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
        }

        #endregion

        #region Material Shortcuts

        /// <summary>Tweens a Material's color to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, NoOptions> DOColor(this Material target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, NoOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named color property to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="property">The name of the material property to tween (like _Tint or _SpecColor)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, NoOptions> DOColor(this Material target, Color endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            TweenerCore<Color, Color, NoOptions> t = DOTween.To(() => target.GetColor(property), x => target.SetColor(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named color property with the given ID to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="propertyID">The ID of the material property to tween (also called nameID in Unity's manual)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, NoOptions> DOColor(this Material target, Color endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(propertyID);
                return null;
            }
            TweenerCore<Color, Color, NoOptions> t = DOTween.To(() => target.GetColor(propertyID), x => target.SetColor(propertyID, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's alpha color to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float, float, NoOptions> DOFade(this Material target, float endValue, float duration)
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
        public static TweenerCore<float, float, NoOptions> DOFade(this Material target, float endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(property);
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
        public static TweenerCore<float, float, NoOptions> DOFade(this Material target, float endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(propertyID);
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
        public static TweenerCore<float, float, NoOptions> DOFloat(this Material target, float endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            TweenerCore<float, float, NoOptions> t = DOTween.To(() => target.GetFloat(property), x => target.SetFloat(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named float property with the given ID to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="propertyID">The ID of the material property to tween (also called nameID in Unity's manual)</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<float, float, NoOptions> DOFloat(this Material target, float endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(propertyID);
                return null;
            }
            TweenerCore<float, float, NoOptions> t = DOTween.To(() => target.GetFloat(propertyID), x => target.SetFloat(propertyID, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Material's texture offset to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2, Vector2, VectorOptions> DOOffset(this Material target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.mainTextureOffset, x => target.mainTextureOffset = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        /// <summary>Tweens a Material's named texture offset property to the given value.
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="property">The name of the material property to tween</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector2, Vector2, VectorOptions> DOOffset(this Material target, Vector2 endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(property);
                return null;
            }
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.GetTextureOffset(property), x => target.SetTextureOffset(property, x), endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Transform Shortcuts

        /// <summary>Tweens a Transform's position to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOMove(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.position, x => target.position = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's X position to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<DOVector3,DOVector3,VectorOptions> DOMoveX(this Transform target, float endValue, float duration)
        {
            TweenerCore<DOVector3,DOVector3,VectorOptions> t = DOTween.To(() => target.position, x => target.position = x, new Vector3(endValue, 0, 0), duration);
            t.SetOptions(AxisConstraint.X).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's Y position to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveY(this Transform target, float endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.position, x => target.position = x, new Vector3(0, endValue, 0), duration);
            t.SetOptions(AxisConstraint.Y).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localPosition to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMove(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.localPosition, x => target.localPosition = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's X localPosition to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveX(this Transform target, float endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(endValue, 0, 0), duration);
            t.SetOptions(AxisConstraint.X).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's Y localPosition to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveY(this Transform target, float endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(0, endValue, 0), duration);
            t.SetOptions(AxisConstraint.Y).SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localRotation to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<DOQuaternion, DOVector3, NoOptions> DOLocalRotate(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<DOQuaternion, DOVector3, NoOptions> t = DOTween.To(() => target.localRotation, x => target.localRotation = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's rotation to the given value using pure quaternion values.
        /// Also stores the transform as the tween's target so it can be used for filtered operations.
        /// <para>PLEASE NOTE: DOLocalRotate, which takes Vector3 values, is the preferred rotation method.
        /// This method was implemented for very special cases, and doesn't support LoopType.Incremental loops
        /// (neither for itself nor if placed inside a LoopType.Incremental Sequence)</para>
        /// </summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Quaternion, Quaternion, NoOptions> DOLocalRotateQuaternion(this Transform target, Quaternion endValue, float duration)
        {
            TweenerCore<Quaternion, Quaternion, NoOptions> t = DOTween.To(PureQuaternionPlugin.Plug(), () => target.localRotation, x => target.localRotation = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localScale to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.localScale, x => target.localScale = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's localScale uniformly to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(endValue, endValue, endValue);
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.localScale, x => target.localScale = x, endValueV3, duration);
            t.SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's X localScale to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleX(this Transform target, float endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t =  DOTween.To(() => target.localScale, x => target.localScale = x, new Vector3(endValue, 0, 0), duration);
            t.SetOptions(AxisConstraint.X)
                .SetTarget(target);
            return t;
        }

        /// <summary>Tweens a Transform's Y localScale to the given value.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleY(this Transform target, float endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.localScale, x => target.localScale = x, new Vector3(0, endValue, 0), duration);
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOPunchPosition: duration can't be 0, returning NULL without creating a tween");
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOPunchScale: duration can't be 0, returning NULL without creating a tween");
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOPunchRotation: duration can't be 0, returning NULL without creating a tween");
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localPosition, x => target.localPosition = x, duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localPosition, x => target.localPosition = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakeRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localEulerAngles, x => target.localRotation = Quaternion.Euler(x), duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakeRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localEulerAngles, x => target.localRotation = Quaternion.Euler(x), duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
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
                Debug.Log(Debugger.logPriority);
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakeScale: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localScale, x => target.localScale = x, duration, strength, vibrato, randomness, false, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOShakeScale: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            return DOTween.Shake(() => target.localScale, x => target.localScale = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake);
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
            float startPosY = target.position.y; // Temporary fix for OnStart not being called when using Goto instead of GotoWithCallbacks
            float offsetY = -1;
            bool offsetYSet = false;

            // Separate Y Tween so we can elaborate elapsedPercentage on that instead of on the Sequence
            // (in case users add a delay or other elements to the Sequence)
            Sequence s = DOTween.Sequence();
            Tween yTween = DOTween.To(() => target.position, x => target.position = x, new Vector3(0, jumpPower, 0), duration / (numJumps * 2))
                .SetOptions(AxisConstraint.Y).SetEase(Ease.OutQuad).SetRelative()
                .SetLoops(numJumps * 2, LoopType.Yoyo)
                .OnStart(()=> startPosY = target.position.y); // FIXME not called if you only use Goto (and not GotoWithCallbacks)
            s.Append(DOTween.To(() => target.position, x => target.position = x, new Vector3(endValue.x, 0, 0), duration)
                    .SetOptions(AxisConstraint.X).SetEase(Ease.Linear)
                ).Join(yTween)
                .SetTarget(target).SetEase(DOTween.defaultEaseType);
            yTween.OnUpdate(() => {
                if (!offsetYSet) {
                    offsetYSet = true;
                    offsetY = s.isRelative ? endValue.y : endValue.y - startPosY;
                }
                Vector3 pos = target.position;
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
            float startPosY = target.localPosition.y; // Temporary fix for OnStart not being called when using Goto instead of GotoWithCallbacks
            float offsetY = -1;
            bool offsetYSet = false;

            // Separate Y Tween so we can elaborate elapsedPercentage on that instead of on the Sequence
            // (in case users add a delay or other elements to the Sequence)
            Sequence s = DOTween.Sequence();
            Tween yTween = DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(0, jumpPower, 0), duration / (numJumps * 2))
                .SetOptions(AxisConstraint.Y).SetEase(Ease.OutQuad).SetRelative()
                .SetLoops(numJumps * 2, LoopType.Yoyo)
                .OnStart(()=> startPosY = target.localPosition.y); // FIXME not called if you only use Goto (and not GotoWithCallbacks)
            s.Append(DOTween.To(() => target.localPosition, x => target.localPosition = x, new Vector3(endValue.x, 0, 0), duration)
                    .SetOptions(AxisConstraint.X).SetEase(Ease.Linear)
                ).Join(yTween)
                .SetTarget(target).SetEase(DOTween.defaultEaseType);
            yTween.OnUpdate(() => {
                if (!offsetYSet) {
                    offsetYSet = true;
                    offsetY = s.isRelative ? endValue.y : endValue.y - startPosY;
                }
                Vector3 pos = target.localPosition;
                pos.y += EaseManager.Evaluate(0, offsetY, yTween.ElapsedPercentage(), Ease.OutQuad);
                target.localPosition = pos;
            });
            return s;
        }

        #endregion

        #endregion

        #region Tween

        /// <summary>Tweens a Tween's timeScale to the given value.
        /// Also stores the Tween as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float, float, NoOptions> DOTimeScale(this Tween target, float endValue, float duration)
        {
            TweenerCore<float, float, NoOptions> t = DOTween.To(() => target.timeScale, x => target.timeScale = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        #endregion

        #region Blendables

        #region Material

        /// <summary>Tweens a Material's color to the given value,
        /// in a way that allows other DOBlendableColor tweens to work together on the same target,
        /// instead than fight each other as multiple DOColor would do.
        /// Also stores the Material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The value to tween to</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableColor(this Material target, Color endValue, float duration)
        {
            endValue = endValue - target.color;
            Color to = new Color(0, 0, 0, 0);
            return DOTween.To(() => to, x => {
#if COMPATIBLE
                Color diff = x.value - to;
#else
                Color diff = x - to;
#endif
                to = x;
                target.color += diff;
            }, endValue, duration)
                .Blendable().SetTarget(target);
        }
        /// <summary>Tweens a Material's named color property to the given value,
        /// in a way that allows other DOBlendableColor tweens to work together on the same target,
        /// instead than fight each other as multiple DOColor would do.
        /// Also stores the Material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The value to tween to</param>
        /// <param name="property">The name of the material property to tween (like _Tint or _SpecColor)</param>
        /// <param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableColor(this Material target, Color endValue, string property, float duration)
        {
            if (!target.HasProperty(property)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(property);
                return null;
            }

            endValue = endValue - target.GetColor(property);
            Color to = new Color(0, 0, 0, 0);
            return DOTween.To(() => to, x => {
#if COMPATIBLE
                Color diff = x.value - to;
#else
                Color diff = x - to;
#endif
                to = x;
                target.SetColor(property, target.GetColor(property) + diff);
            }, endValue, duration)
                .Blendable().SetTarget(target);
        }
        /// <summary>Tweens a Material's named color property with the given ID to the given value,
        /// in a way that allows other DOBlendableColor tweens to work together on the same target,
        /// instead than fight each other as multiple DOColor would do.
        /// Also stores the Material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The value to tween to</param>
        /// <param name="propertyID">The ID of the material property to tween (also called nameID in Unity's manual)</param>
        /// <param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableColor(this Material target, Color endValue, int propertyID, float duration)
        {
            if (!target.HasProperty(propertyID)) {
                if (Debugger.logPriority > 0) Debugger.LogMissingMaterialProperty(propertyID);
                return null;
            }

            endValue = endValue - target.GetColor(propertyID);
            Color to = new Color(0, 0, 0, 0);
            return DOTween.To(() => to, x => {
#if COMPATIBLE
                Color diff = x.value - to;
#else
                    Color diff = x - to;
#endif
                    to = x;
                    target.SetColor(propertyID, target.GetColor(propertyID) + diff);
                }, endValue, duration)
                .Blendable().SetTarget(target);
        }

        #endregion

        #region Transform

        /// <summary>Tweens a Transform's position BY the given value (as if you chained a <code>SetRelative</code>),
        /// in a way that allows other DOBlendableMove tweens to work together on the same target,
        /// instead than fight each other as multiple DOMove would do.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="byValue">The value to tween by</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableMoveBy(this Transform target, Vector3 byValue, float duration)
        {
            Vector3 to = Vector3.zero;
            return DOTween.To(() => to, x => {
#if COMPATIBLE
                Vector3 diff = x.value - to;
#else
                Vector3 diff = x - to;
#endif
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
            Vector3 to = Vector3.zero;
            return DOTween.To(() => to, x => {
#if COMPATIBLE
                Vector3 diff = x.value - to;
#else
                Vector3 diff = x - to;
#endif
                to = x;
                target.localPosition += diff;
            }, byValue, duration)
                .Blendable().SetTarget(target);
        }

#if !COMPATIBLE
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
                if (Debugger.logPriority > 0) Debug.LogWarning("DOBlendablePunchRotation: duration can't be 0, returning NULL without creating a tween");
                return null;
            }
            Vector3 to = Vector3.zero;
            TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> t = DOTween.Punch(() => to, v => {
                Quaternion qto = Quaternion.Euler(to.x, to.y, to.z);
                Quaternion qnew = Quaternion.Euler(v.x, v.y, v.z);
#if COMPATIBLE
                Quaternion diff = x.value * Quaternion.Inverse(qto);
#else
                Quaternion diff = qnew * Quaternion.Inverse(qto);
#endif
                to = v;
                Quaternion currRot = target.rotation;
                target.rotation = currRot * Quaternion.Inverse(currRot) * diff * currRot;
            }, punch, duration, vibrato, elasticity)
                .Blendable().SetTarget(target);
            return t;
        }
#endif

        /// <summary>Tweens a Transform's localScale BY the given value (as if you chained a <code>SetRelative</code>),
        /// in a way that allows other DOBlendableScale tweens to work together on the same target,
        /// instead than fight each other as multiple DOScale would do.
        /// Also stores the transform as the tween's target so it can be used for filtered operations</summary>
        /// <param name="byValue">The value to tween by</param><param name="duration">The duration of the tween</param>
        public static Tweener DOBlendableScaleBy(this Transform target, Vector3 byValue, float duration)
        {
            Vector3 to = Vector3.zero;
            return DOTween.To(() => to, x => {
#if COMPATIBLE
                Vector3 diff = x.value - to;
#else
                Vector3 diff = x - to;
#endif
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
        public static int DOComplete(this Component target, bool withCallbacks = false)
        {
            return DOTween.Complete(target, withCallbacks);
        }
        /// <summary>
        /// Completes all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens completed
        /// (meaning the tweens that don't have infinite loops and were not already complete)
        /// </summary>
        /// <param name="withCallbacks">For Sequences only: if TRUE also internal Sequence callbacks will be fired,
        /// otherwise they will be ignored</param>
        public static int DOComplete(this Material target, bool withCallbacks = false)
        {
            return DOTween.Complete(target, withCallbacks);
        }

        /// <summary>
        /// Kills all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens killed.
        /// </summary>
        /// <param name="complete">If TRUE completes the tween before killing it</param>
        public static int DOKill(this Component target, bool complete = false)
        {
//            int tot = complete ? DOTween.CompleteAndReturnKilledTot(target) : 0;
//            return tot + DOTween.Kill(target);
            return DOTween.Kill(target, complete);
        }
        /// <summary>
        /// Kills all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens killed.
        /// </summary>
        /// <param name="complete">If TRUE completes the tween before killing it</param>
        public static int DOKill(this Material target, bool complete = false)
        {
            return DOTween.Kill(target, complete);
        }

        /// <summary>
        /// Flips the direction (backwards if it was going forward or viceversa) of all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens flipped.
        /// </summary>
        public static int DOFlip(this Component target)
        {
            return DOTween.Flip(target);
        }
        /// <summary>
        /// Flips the direction (backwards if it was going forward or viceversa) of all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens flipped.
        /// </summary>
        public static int DOFlip(this Material target)
        {
            return DOTween.Flip(target);
        }

        /// <summary>
        /// Sends to the given position all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens involved.
        /// </summary>
        /// <param name="to">Time position to reach
        /// (if higher than the whole tween duration the tween will simply reach its end)</param>
        /// <param name="andPlay">If TRUE will play the tween after reaching the given position, otherwise it will pause it</param>
        public static int DOGoto(this Component target, float to, bool andPlay = false)
        {
            return DOTween.Goto(target, to, andPlay);
        }
        /// <summary>
        /// Sends to the given position all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens involved.
        /// </summary>
        /// <param name="to">Time position to reach
        /// (if higher than the whole tween duration the tween will simply reach its end)</param>
        /// <param name="andPlay">If TRUE will play the tween after reaching the given position, otherwise it will pause it</param>
        public static int DOGoto(this Material target, float to, bool andPlay = false)
        {
            return DOTween.Goto(target, to, andPlay);
        }

        /// <summary>
        /// Pauses all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens paused.
        /// </summary>
        public static int DOPause(this Component target)
        {
            return DOTween.Pause(target);
        }
        /// <summary>
        /// Pauses all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens paused.
        /// </summary>
        public static int DOPause(this Material target)
        {
            return DOTween.Pause(target);
        }

        /// <summary>
        /// Plays all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static int DOPlay(this Component target)
        {
            return DOTween.Play(target);
        }
        /// <summary>
        /// Plays all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static int DOPlay(this Material target)
        {
            return DOTween.Play(target);
        }

        /// <summary>
        /// Plays backwards all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static int DOPlayBackwards(this Component target)
        {
            return DOTween.PlayBackwards(target);
        }
        /// <summary>
        /// Plays backwards all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static int DOPlayBackwards(this Material target)
        {
            return DOTween.PlayBackwards(target);
        }

        /// <summary>
        /// Plays forward all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static int DOPlayForward(this Component target)
        {
            return DOTween.PlayForward(target);
        }
        /// <summary>
        /// Plays forward all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens played.
        /// </summary>
        public static int DOPlayForward(this Material target)
        {
            return DOTween.PlayForward(target);
        }

        /// <summary>
        /// Restarts all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens restarted.
        /// </summary>
        public static int DORestart(this Component target, bool includeDelay = true)
        {
            return DOTween.Restart(target, includeDelay);
        }
        /// <summary>
        /// Restarts all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens restarted.
        /// </summary>
        public static int DORestart(this Material target, bool includeDelay = true)
        {
            return DOTween.Restart(target, includeDelay);
        }

        /// <summary>
        /// Rewinds all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens rewinded.
        /// </summary>
        public static int DORewind(this Component target, bool includeDelay = true)
        {
            return DOTween.Rewind(target, includeDelay);
        }
        /// <summary>
        /// Rewinds all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens rewinded.
        /// </summary>
        public static int DORewind(this Material target, bool includeDelay = true)
        {
            return DOTween.Rewind(target, includeDelay);
        }

        /// <summary>
        /// Smoothly rewinds all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens rewinded.
        /// </summary>
        public static int DOSmoothRewind(this Component target)
        {
            return DOTween.SmoothRewind(target);
        }
        /// <summary>
        /// Smoothly rewinds all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens rewinded.
        /// </summary>
        public static int DOSmoothRewind(this Material target)
        {
            return DOTween.SmoothRewind(target);
        }

        /// <summary>
        /// Toggles the paused state (plays if it was paused, pauses if it was playing) of all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens involved.
        /// </summary>
        public static int DOTogglePause(this Component target)
        {
            return DOTween.TogglePause(target);
        }
        /// <summary>
        /// Toggles the paused state (plays if it was paused, pauses if it was playing) of all tweens that have this target as a reference
        /// (meaning tweens that were started from this target, or that had this target added as an Id)
        /// and returns the total number of tweens involved.
        /// </summary>
        public static int DOTogglePause(this Material target)
        {
            return DOTween.TogglePause(target);
        }

        #endregion
    }
}