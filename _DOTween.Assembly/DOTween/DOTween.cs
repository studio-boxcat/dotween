// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 14:05
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php


using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    /// <summary>
    /// Main DOTween class. Contains static methods to create and control tweens in a generic way
    /// </summary>
    public static class DOTween
    {
        // public static readonly string Version = "1.2.751"; // Last version before modules: 1.1.755

        /// <summary>If TRUE (default) makes tweens slightly slower but safer, automatically taking care of a series of things
        /// (like targets becoming null while a tween is playing).
        /// <para>Default: TRUE</para></summary>
        public const bool useSafeMode = true;
        /// <summary>Behaviour in case a tween nested inside a Sequence fails (and is caught by safe mode).
        /// <para>Default: NestedTweenFailureBehaviour.TryToPreserveSequence</para></summary>
        public static NestedTweenFailureBehaviour nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;

        /// <summary>Default ease applied to all new Tweeners (not to Sequences which always have Ease.Linear as default).</summary>
        public const Ease defaultEaseType = Ease.OutQuad;
        /// <summary>Default overshoot/amplitude used for eases.</summary>
        public const float defaultEaseOvershootOrAmplitude = 1.70158f;

        internal static int maxActiveTweenersReached, maxActiveSequencesReached; // Controlled by DOTweenInspector if showUnityEditorReport is active
        internal static bool initialized; // Can be set to false by DOTweenComponent OnDestroy

        #region Public Methods

        static void InitCheck()
        {
            if (initialized || !DOTweenUnityBridge.IsPlaying())
                return;

            L.I("[DOTween] Init");

            initialized = true;

            DOTweenUnityBridge.Create();

            // Assign settings
            var settings = DOTweenSettings.Instance;
            nestedTweenFailureBehaviour = settings.safeModeOptions.nestedTweenFailureBehaviour;
        }

        /// <summary>
        /// Kills all tweens, clears all cached tween pools and plugins and resets the max Tweeners/Sequences capacities to the default values.
        /// </summary>
        /// (so that next time you use it it will need to be re-initialized)</param>
        public static void Clear()
        {
            L.I("[DOTween] Clear");

            initialized = false;

            TweenManager.PurgeAll();

            nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
            maxActiveTweenersReached = maxActiveSequencesReached = 0;
        }

        public static void Update()
        {
            if (TweenManager.hasActiveDefaultTweens)
            {
                Assert.IsTrue(initialized, "hasActiveTweens but DOTween is not initialized");
                TweenManager.Update(UpdateType.Normal, Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        public static void ManualUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (TweenManager.hasActiveManualTweens)
            {
                TweenManager.Update(UpdateType.Manual, deltaTime, unscaledDeltaTime);
            }
        }

        #endregion

        // ===================================================================================
        // PUBLIC TWEEN CREATION METHODS -----------------------------------------------------

        #region Tween TO

        static TweenerCore<T> To<T>(DOGetter<T> getter, DOSetter<T> setter, T endValue, float duration, TweenPlugin<T> plugin)
        {
            InitCheck();
            L.I($"[DOTween] To: endValue={endValue}, duration={duration}");
            var t = TweenManager.GetTweener<T>();
            t.Setup(getter, setter, endValue, duration, plugin);
            return t;
        }

        public static TweenerCore<float> To(DOGetter<float> getter, DOSetter<float> setter, float endValue, float duration)
            => To(getter, setter, endValue, duration, FloatPlugin.Instance);
        public static TweenerCore<int> To(DOGetter<int> getter, DOSetter<int> setter, int endValue, float duration)
            => To(getter, setter, endValue, duration, IntPlugin.Instance);
        public static TweenerCore<Vector2> To(DOGetter<Vector2> getter, DOSetter<Vector2> setter, Vector2 endValue, float duration)
            => To(getter, setter, endValue, duration, Vector2Plugin.Instance);
        public static TweenerCore<Vector3> To(DOGetter<Vector3> getter, DOSetter<Vector3> setter, Vector3 endValue, float duration)
            => To(getter, setter, endValue, duration, Vector3Plugin.Instance);
        public static TweenerCore<Color> To(DOGetter<Color> getter, DOSetter<Color> setter, Color endValue, float duration)
            => To(getter, setter, endValue, duration, ColorPlugin.Instance);

        /// <summary>Tweens only the alpha of a Color to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<float> ToAlpha(DOGetter<Color> getter, DOSetter<Color> setter, float endValue, float duration)
        {
            return To(
                () => getter().a,
                x =>
                {
                    var c = getter();
                    c.a = x;
                    setter(c);
                },
                endValue, duration);
        }

        #endregion

        #region Special TOs (No FROMs)

        static TweenerCore<Vector3> To(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration, Vector3ArrayOptions opts)
        {
            var t = To(getter, setter, default, duration, Vector3ArrayPlugin.Instance);
            t.plugOptions = opts;
            return t;
        }

        /// <summary>Punches a Vector3 towards the given direction and then back to the starting one
        /// as if it was connected to the starting position via an elastic.
        /// <para>This tween type generates some GC allocations at startup</para></summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="direction">The direction and strength of the punch</param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="vibrato">Indicates how much will the punch vibrate</param>
        /// <param name="elasticity">Represents how much (0 to 1) the vector will go beyond the starting position when bouncing backwards.
        /// 1 creates a full oscillation between the direction and the opposite decaying direction,
        /// while 0 oscillates only between the starting position and the decaying direction</param>
        public static TweenerCore<Vector3> Punch(DOGetter<Vector3> getter, DOSetter<Vector3> setter, Vector3 direction, float duration, int vibrato = 10, float elasticity = 1)
        {
            var segmentCount = (int) (vibrato * duration);
            if (segmentCount < 2) segmentCount = 2;
            var opts = SpecialTweenUtils.CalculatePunch(direction, segmentCount, elasticity);
            var t = To(getter, setter, duration, opts);
            SpecialTweenUtils.SetupPunch(t);
            return t;
        }

        /// <summary>Shakes a Vector3 with the given values.</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction and behave like a random punch.</param>
        /// <param name="ignoreZAxis">If TRUE only shakes on the X Y axis (looks better with things like cameras).</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static TweenerCore<Vector3> Shake(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration,
            float strength = 3, int vibrato = 10, float randomness = 90, bool ignoreZAxis = true,
            bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full
        )
        {
            return Shake(getter, setter, duration, new Vector3(strength, strength, strength), vibrato, randomness, ignoreZAxis, false, fadeOut, randomnessMode);
        }
        /// <summary>Shakes a Vector3 with the given values.</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="duration">The duration of the tween</param>
        /// <param name="strength">The shake strength on each axis</param>
        /// <param name="vibrato">Indicates how much will the shake vibrate</param>
        /// <param name="randomness">Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware).
        /// Setting it to 0 will shake along a single direction and behave like a random punch.</param>
        /// <param name="fadeOut">If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not</param>
        /// <param name="randomnessMode">Randomness mode</param>
        public static TweenerCore<Vector3> Shake(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration,
            Vector3 strength, int vibrato = 10, float randomness = 90,
            bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full
        )
        {
            return Shake(getter, setter, duration, strength, vibrato, randomness, false, true, fadeOut, randomnessMode);
        }
        static TweenerCore<Vector3> Shake(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration,
            Vector3 strength, int vibrato, float randomness, bool ignoreZAxis, bool vectorBased,
            bool fadeOut, ShakeRandomnessMode randomnessMode
        )
        {
            var segmentCount = (int) (vibrato * duration);
            if (segmentCount < 2) segmentCount = 2;
            var opts = SpecialTweenUtils.CalculateShake(strength, segmentCount, randomness, ignoreZAxis, vectorBased, fadeOut, randomnessMode);
            var t = To(getter, setter, duration, opts);
            SpecialTweenUtils.SetupShake(t);
            return t;
        }

        #endregion

        #region Tween SEQUENCE

        /// <summary>
        /// Returns a new <see cref="DG.Tweening.Sequence"/> to be used for tween groups.<para/>
        /// Mind that Sequences don't have a target applied automatically like Tweener creation shortcuts,
        /// so if you want to be able to kill this Sequence when calling DOTween.Kill(target) you'll have to add
        /// the target manually; you can do that directly by using the <see cref="Sequence(object)"/> overload instead of this one
        /// </summary>
        public static Sequence Sequence()
        {
            InitCheck();
            Sequence sequence = TweenManager.GetSequence();
            Tweening.Sequence.Setup(sequence);
            return sequence;
        }
        /// <summary>
        /// Returns a new <see cref="DG.Tweening.Sequence"/> to be used for tween groups, and allows to set a target
        /// (because Sequences don't have their target set automatically like Tweener creation shortcuts).
        /// That way killing/controlling tweens by target will apply to this Sequence too.
        /// </summary>
        /// <param name="target">The target of the Sequence. Relevant only for static target-based methods like DOTween.Kill(target),
        /// useless otherwise</param>
        public static Sequence Sequence(object target)
        { return Sequence().SetTarget(target); }

        #endregion

        /////////////////////////////////////////////////////////////////////
        // OTHER STUFF //////////////////////////////////////////////////////

        #region Play Operations

        /// <summary>Kills all tweens with the given ID or target and returns the number of actual tweens killed</summary>
        /// <param name="targetOrId"></param>
        /// <param name="complete">If TRUE completes the tweens before killing them</param>
        public static void Kill(Object targetOrId, bool complete = false)
            => targetOrId.DOKill(complete);

        #endregion

        #region Global Info Getters

        /// <summary>
        /// Returns TRUE if a tween with the given ID or target is active.
        /// <para>You can also use this to know if a shortcut tween is active for a given target.</para>
        /// <para>Example:</para>
        /// <para><code>transform.DOMoveX(45, 1); // transform is automatically added as the tween target</code></para>
        /// <para><code>DOTween.IsTweening(transform); // Returns true</code></para>
        /// </summary>
        /// <param name="targetOrId">The target or ID to look for</param>
        /// <param name="alsoCheckIfIsPlaying">If FALSE (default) returns TRUE as long as a tween for the given target/ID is active,
        /// otherwise also requires it to be playing</param>
        public static bool IsTweening(object targetOrId, bool alsoCheckIfIsPlaying = false)
        {
            return TweenManager.FilteredOperation(OperationType.IsTweening, targetOrId, alsoCheckIfIsPlaying, 0) > 0;
        }

        #endregion
    }
}