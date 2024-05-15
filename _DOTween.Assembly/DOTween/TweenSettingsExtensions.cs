// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/05 16:36
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System.Diagnostics;
using DOVector2 = UnityEngine.Vector2;
using DOVector3 = UnityEngine.Vector3;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using Debugger = DG.Tweening.Core.Debugger;

#pragma warning disable 1573
namespace DG.Tweening
{
    /// <summary>
    /// Methods that extend Tween objects and allow to set their parameters
    /// </summary>
    public static class TweenSettingsExtensions
    {
        #region Tweeners + Sequences

        /// <summary>Sets the autoKill behaviour of the tween to TRUE. 
        /// <code>Has no effect</code> if the tween has already started or if it's added to a Sequence</summary>
        public static T SetAutoKill<T>(this T t) where T : Tween
        {
            if (t is not { active: true } || t.creationLocked) return t;

            t.autoKill = true;
            return t;
        }
        /// <summary>Sets the autoKill behaviour of the tween.
        /// <code>Has no effect</code> if the tween has already started or if it's added to a Sequence</summary>
        /// <param name="autoKillOnCompletion">If TRUE the tween will be automatically killed when complete</param>
        public static T SetAutoKill<T>(this T t, bool autoKillOnCompletion) where T : Tween
        {
            if (t is not { active: true } || t.creationLocked) return t;

            t.autoKill = autoKillOnCompletion;
            return t;
        }

        /// <summary>Sets an int ID for the tween (<see cref="Tween.id"/>), which can then be used as a filter with DOTween's static methods.<para/>
        /// Filtering via int is 4X faster than via object, 2X faster than via string (using the alternate object/string overloads)</summary>
        /// <param name="intId">The int ID to assign to this tween.</param>
        public static T SetId<T>(this T t, int intId) where T : Tween
        {
            Assert.IsNotNull(t);
            Assert.IsTrue(t.active);
            t.id = intId;
            return t;
        }

        /// <summary>Sets the target for the tween, which can then be used as a filter with DOTween's static methods.
        /// <para>IMPORTANT: use it with caution. If you just want to set an ID for the tween use <code>SetId</code> instead.</para>
        /// When using shorcuts the shortcut target is already assigned as the tween's target,
        /// so using this method will overwrite it and prevent shortcut-operations like myTarget.DOPause from working correctly.</summary>
        /// <param name="target">The target to assign to this tween. Can be an int, a string, an object or anything else.</param>
        public static T SetTarget<T>(this T t, object target) where T : Tween
        {
            if (t is not { active: true }) return t;

#if DEBUG
            var obj = target as Object;
            t.debugHint = obj != null ? obj.name : target.ToString();
#endif

            t.target = target;
            return t;
        }

        /// <summary>Sets the looping options for the tween.
        /// Has no effect if the tween has already started</summary>
        /// <param name="loops">Number of cycles to play (-1 for infinite - will be converted to 1 in case the tween is nested in a Sequence)</param>
        /// <param name="loopType">Loop behaviour type (default: LoopType.Restart)</param>
        public static T SetLoops<T>(this T t, int loops, LoopType loopType) where T : Tween
        {
            if (t is not { active: true } || t.creationLocked) return t;

            if (loops < -1) loops = -1;
            else if (loops == 0) loops = 1;
            t.loops = loops;
            t.loopType = loopType;
//            if (t.tweenType == TweenType.Tweener) t.fullDuration = loops > -1 ? t.duration * loops : Mathf.Infinity;
            if (t.tweenType == TweenType.Tweener) {
                if (loops > -1) t.fullDuration = t.duration * loops;
                else t.fullDuration = Mathf.Infinity;
            }
            return t;
        }

        /// <summary>Sets the ease of the tween.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static T SetEase<T>(this T t, Ease ease) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.easeType = ease;
            if (EaseManager.IsFlashEase(ease)) t.easeOvershootOrAmplitude = (int)t.easeOvershootOrAmplitude;

            t.customEase = null;
            ValidateEase(t);
            return t;
        }
        /// <summary>Sets the ease of the tween.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        /// <param name="overshoot">
        /// Eventual overshoot to use with Back or Flash ease (default is 1.70158 - 1 for Flash).
        /// <para>In case of Flash ease it must be an intenger and sets the total number of flashes that will happen.
        /// Using an even number will complete the tween on the starting value, while an odd one will complete it on the end value.</para>
        /// </param>
        public static T SetEase<T>(this T t, Ease ease, float overshoot) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.easeType = ease;
            if (EaseManager.IsFlashEase(ease)) overshoot = (int)overshoot;
            t.easeOvershootOrAmplitude = overshoot;
            t.customEase = null;
            ValidateEase(t);
            return t;
        }
        /// <summary>Sets the ease of the tween.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        /// <param name="amplitude">Eventual amplitude to use with Elastic easeType or overshoot to use with Flash easeType (default is 1.70158 - 1 for Flash).
        /// <para>In case of Flash ease it must be an integer and sets the total number of flashes that will happen.
        /// Using an even number will complete the tween on the starting value, while an odd one will complete it on the end value.</para>
        /// </param>
        /// <param name="period">Eventual period to use with Elastic or Flash easeType (default is 0).
        /// <para>In case of Flash ease it indicates the power in time of the ease, and must be between -1 and 1.
        /// 0 is balanced, 1 weakens the ease with time, -1 starts the ease weakened and gives it power towards the end.</para>
        /// </param>
        public static T SetEase<T>(this T t, Ease ease, float amplitude, float period) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.easeType = ease;
            if (EaseManager.IsFlashEase(ease)) amplitude = (int)amplitude;
            t.easeOvershootOrAmplitude = amplitude;
            t.easePeriod = period;
            t.customEase = null;
            ValidateEase(t);
            return t;
        }
        /// <summary>Sets the ease of the tween using an AnimationCurve.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static T SetEase<T>([NotNull] this T t, AnimationCurve animCurve) where T : Tween
        {
            if (!t.active)
            {
                Debugger.LogInvalidTween(t);
                return t;
            }

            t.easeType = Ease.INTERNAL_Custom;
            t.customEase = new EaseCurve(animCurve).Evaluate;
            ValidateEase(t);
            return t;
        }
        /// <summary>Sets the ease of the tween using a custom ease function (which must return a value between 0 and 1).
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static T SetEase<T>([NotNull] this T t, EaseFunction customEase) where T : Tween
        {
            if (!t.active)
            {
                Debugger.LogInvalidTween(t);
                return t;
            }

            t.easeType = Ease.INTERNAL_Custom;
            t.customEase = customEase;
            ValidateEase(t);
            return t;
        }

        [Conditional("DEBUG")]
        static void ValidateEase(Tween t)
        {
            if (t is TweenerCore<DOVector3> { plugin: Vector3ArrayPlugin }
                && t.easeType is not (Ease.Linear or Ease.OutQuad))
            {
                L.W("Vector3ArrayPlugin only supports Linear and OutQuad ease types", t);
            }
        }

        /// <summary>Sets the <code>onStart</code> callback for the tween, clearing any previous <code>onStart</code> callback that was set.
        /// Called the first time the tween is set in a playing state, after any eventual delay</summary>
        public static T OnStart<T>(this T t, TweenCallback action) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.onStart = action;
            return t;
        }

        /// <summary>Sets the <code>onUpdate</code> callback for the tween, clearing any previous <code>onUpdate</code> callback that was set.
        /// Called each time the tween updates</summary>
        public static T OnUpdate<T>(this T t, TweenCallback action) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.onUpdate = action;
            return t;
        }

        /// <summary>Sets the <code>onComplete</code> callback for the tween, clearing any previous <code>onComplete</code> callback that was set.
        /// Called the moment the tween reaches its final forward position, loops included</summary>
        public static T OnComplete<T>(this T t, TweenCallback action) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.onComplete = action;
            return t;
        }

        /// <summary>Sets the <code>onKill</code> callback for the tween, clearing any previous <code>onKill</code> callback that was set.
        /// Called the moment the tween is killed</summary>
        public static T OnKill<T>(this T t, TweenCallback action) where T : Tween
        {
            if (t is not { active: true }) return t;

            t.onKill = action;
            return t;
        }

        #endregion

        #region Sequences-only

        /// <summary>Adds the given tween to the end of the Sequence.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="t">The tween to append</param>
        public static Sequence Append([NotNull] this Sequence s, [NotNull] Tween t)
        {
            if (!ValidateAddToSequence(s) || !ValidateAddToSequence(t)) return s;
            Sequence.DoInsert(s, t, s.duration);
            return s;
        }
        /// <summary>Adds the given tween to the beginning of the Sequence, pushing forward the other nested content.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="t">The tween to prepend</param>
        public static Sequence Prepend([NotNull] this Sequence s, [NotNull] Tween t)
        {
            if (!ValidateAddToSequence(s) || !ValidateAddToSequence(t)) return s;
            Sequence.DoPrepend(s, t);
            return s;
        }
        /// <summary>Inserts the given tween at the same time position of the last tween, callback or intervale added to the Sequence.
        /// Note that, in case of a Join after an interval, the insertion time will be the time where the interval starts, not where it finishes.
        /// Has no effect if the Sequence has already started</summary>
        public static Sequence Join([NotNull] this Sequence s, [NotNull] Tween t)
        {
            if (!ValidateAddToSequence(s) || !ValidateAddToSequence(t)) return s;
            Sequence.DoInsert(s, t, s.lastTweenInsertTime);
            return s;
        }
        /// <summary>Inserts the given tween at the given time position in the Sequence,
        /// automatically adding an interval if needed.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="atPosition">The time position where the tween will be placed</param>
        /// <param name="t">The tween to insert</param>
        public static Sequence Insert([NotNull] this Sequence s, float atPosition, [NotNull] Tween t)
        {
            if (!ValidateAddToSequence(s) || !ValidateAddToSequence(t)) return s;
            Sequence.DoInsert(s, t, atPosition);
            return s;
        }

        /// <summary>Adds the given interval to the end of the Sequence.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="interval">The interval duration</param>
        public static Sequence AppendInterval([NotNull] this Sequence s, float interval)
        {
            if (!ValidateAddToSequence(s)) return s;
            Sequence.DoAppendInterval(s, interval);
            return s;
        }
        /// <summary>Adds the given interval to the beginning of the Sequence, pushing forward the other nested content.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="interval">The interval duration</param>
        public static Sequence PrependInterval([NotNull] this Sequence s, float interval)
        {
            if (!ValidateAddToSequence(s)) return s;
            Sequence.DoPrependInterval(s, interval);
            return s;
        }

        /// <summary>Adds the given callback to the end of the Sequence.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="callback">The callback to append</param>
        public static Sequence AppendCallback([NotNull] this Sequence s, TweenCallback callback)
        {
            if (!ValidateAddToSequence(s)) return s;
            if (callback == null) return s;

            Sequence.DoInsertCallback(s, callback, s.duration);
            return s;
        }
        /// <summary>Adds the given callback to the beginning of the Sequence, pushing forward the other nested content.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="callback">The callback to prepend</param>
        public static Sequence PrependCallback([NotNull] this Sequence s, TweenCallback callback)
        {
            if (!ValidateAddToSequence(s)) return s;
            if (callback == null) return s;

            Sequence.DoInsertCallback(s, callback, 0);
            return s;
        }
        /// <summary>Inserts the given callback at the given time position in the Sequence,
        /// automatically adding an interval if needed.
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="atPosition">The time position where the callback will be placed</param>
        /// <param name="callback">The callback to insert</param>
        public static Sequence InsertCallback([NotNull] this Sequence s, float atPosition, TweenCallback callback)
        {
            if (!ValidateAddToSequence(s)) return s;
            if (callback == null) return s;

            Sequence.DoInsertCallback(s, callback, atPosition);
            return s;
        }

        static bool ValidateAddToSequence([NotNull] Sequence s)
        {
            if (!s.active) {
                L.W("You can't add elements to an inactive/killed Sequence");
                return false;
            }
            if (s.creationLocked) {
                L.W("The Sequence has started and is now locked, you can only elements to a Sequence before it starts");
                return false;
            }
            return true;
        }

        static bool ValidateAddToSequence([NotNull] Tween t)
        {
            if (!t.active) {
                L.W("You can't add an inactive/killed tween to a Sequence", t);
                return false;
            }
            if (t.isSequenced) {
                L.W("You can't add a tween that is already nested into a Sequence to another Sequence", t);
                return false;
            }
            return true;
        }

        #endregion

        #region Tweeners-only

        #region FROM

        /// <summary>Changes a TO tween into a FROM tween: sets the current target's position as the tween's endValue
        /// then immediately sends the target to the previously set endValue.</summary>
        /// <param name="isRelative">If TRUE the FROM value will be calculated as relative to the current one</param>
        public static T From<T>(this T t, bool isRelative = false) where T : Tweener
        {
            if (t is not { active: true } || t.creationLocked || !t.isFromAllowed) return t;

            t.isFrom = true;
            t.SetFrom(isRelative && !t.isBlendable);
            return t;
        }

        /// <summary>Changes a TO tween into a FROM tween: sets the tween's starting value to the given one
        /// and eventually sets the tween's target to that value immediately.</summary>
        /// <param name="fromValue">Value to start from</param>
        /// <param name="setImmediately">If TRUE sets the target to from value immediately, otherwise waits for the tween to start</param>
        /// <param name="isRelative">If TRUE the FROM/TO values will be calculated as relative to the current ones</param>
        public static TweenerCore<T> From<T>(
            this TweenerCore<T> t, T fromValue, bool setImmediately = true, bool isRelative = false
        )
        {
            if (t is not { active: true } || t.creationLocked || !t.isFromAllowed) return t;

            t.isFrom = true;
            t.SetFrom(fromValue, setImmediately, isRelative);
            return t;
        }

        #endregion

        /// <summary>Sets a delayed startup for the tween.<para/>
        /// In case of Sequences behaves the same as <see cref="PrependInterval"/>,
        /// which means the delay will repeat in case of loops (while with tweens it's ignored after the first loop cycle).<para/>
        /// Has no effect if the tween has already started</summary>
        public static T SetDelay<T>(this T t, float delay) where T : Tween
        {
            if (t is not { active: true } || t.creationLocked) return t;

            if (t is Sequence s) {
                s.PrependInterval(delay);
            } else {
                t.delay = delay;
                t.delayComplete = delay <= 0;
            }
            return t;
        }

        /// <summary>Sets the tween as relative
        /// (the endValue will be calculated as <code>startValue + endValue</code> instead than being used directly).
        /// <para>Has no effect on Sequences or if the tween has already started</para></summary>
        public static T SetRelative<T>(this T t) where T : Tween
        {
            if (t is not { active: true } || t.creationLocked || t.isFrom || t.isBlendable) return t;

            t.isRelative = true;
            return t;
        }
        /// <summary>If isRelative is TRUE sets the tween as relative
        /// (the endValue will be calculated as <code>startValue + endValue</code> instead than being used directly).
        /// <para>Has no effect on Sequences or if the tween has already started</para></summary>
        public static T SetRelative<T>(this T t, bool isRelative) where T : Tween
        {
            if (t is not { active: true } || t.creationLocked || t.isFrom || t.isBlendable) return t;

            t.isRelative = isRelative;
            return t;
        }

        #endregion

        #region Tweeners Extra Options

        /// <summary>Options for Vector2 tweens</summary>
        /// <param name="axisConstraint">Selecting an axis will tween the vector only on that axis, leaving the others untouched</param>
        public static Tweener SetOptions(this TweenerCore<DOVector2> t, AxisConstraint axisConstraint)
        {
            if (t is not { active: true }) return t;

            VectorOptions.SetAxisConstraint(t, axisConstraint);
            return t;
        }

        /// <summary>Options for Vector3 tweens</summary>
        /// <param name="axisConstraint">Selecting an axis will tween the vector only on that axis, leaving the others untouched</param>
        public static Tweener SetOptions(this TweenerCore<DOVector3> t, AxisConstraint axisConstraint)
        {
            if (t is not { active: true }) return t;

            VectorOptions.SetAxisConstraint(t, axisConstraint);
            return t;
        }

        #endregion
    }
}