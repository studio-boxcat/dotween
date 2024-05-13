// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 14:05
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php


using DOVector2 = UnityEngine.Vector2;
using DOVector3 = UnityEngine.Vector3;
using DOColor = UnityEngine.Color;
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
    public class DOTween
    {
        // public static readonly string Version = "1.2.751"; // Last version before modules: 1.1.755

        ///////////////////////////////////////////////
        // Options ////////////////////////////////////

        /// <summary>If TRUE (default) makes tweens slightly slower but safer, automatically taking care of a series of things
        /// (like targets becoming null while a tween is playing).
        /// <para>Default: TRUE</para></summary>
        public static bool useSafeMode = true;
        /// <summary>Behaviour in case a tween nested inside a Sequence fails (and is caught by safe mode).
        /// <para>Default: NestedTweenFailureBehaviour.TryToPreserveSequence</para></summary>
        public static NestedTweenFailureBehaviour nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
        // DEBUG OPTIONS
        /// <summary>If TRUE activates various debug options</summary>
        public const bool debugMode =
#if DEBUG
            true;
#else
            false;
#endif
        /// <summary>Stores the target id so it can be used to give more info in case of safeMode error capturing.
        /// Only active if both <code>debugMode</code> and <code>useSafeMode</code> are TRUE</summary>
        public const bool debugStoreTargetId = debugMode;

        ///////////////////////////////////////////////
        // Default options for Tweens /////////////////

        /// <summary>Default ease applied to all new Tweeners (not to Sequences which always have Ease.Linear as default).
        /// <para>Default: Ease.InOutQuad</para></summary>
        public static Ease defaultEaseType = Ease.OutQuad;
        /// <summary>Default overshoot/amplitude used for eases
        /// <para>Default: 1.70158f</para></summary>
        public static float defaultEaseOvershootOrAmplitude = 1.70158f;
        /// <summary>Default period used for eases
        /// <para>Default: 0</para></summary>
        public static float defaultEasePeriod = 0;

        internal static int maxActiveTweenersReached, maxActiveSequencesReached; // Controlled by DOTweenInspector if showUnityEditorReport is active
        internal static bool initialized; // Can be set to false by DOTweenComponent OnDestroy

        #region Public Methods

        static void InitCheck()
        {
            if (initialized || !Application.isPlaying)
                return;

            L.I("[DOTween] Init");

            initialized = true;

            DOTweenUnityBridge.Create();

            // Assign settings
            var settings = DOTweenSettings.Instance;
            useSafeMode = settings.useSafeMode;
            nestedTweenFailureBehaviour = settings.safeModeOptions.nestedTweenFailureBehaviour;
            defaultEaseType = settings.defaultEaseType;
            defaultEaseOvershootOrAmplitude = settings.defaultEaseOvershootOrAmplitude;
            defaultEasePeriod = settings.defaultEasePeriod;
        }

        /// <summary>
        /// Kills all tweens, clears all cached tween pools and plugins and resets the max Tweeners/Sequences capacities to the default values.
        /// </summary>
        /// (so that next time you use it it will need to be re-initialized)</param>
        public static void Clear()
        {
            L.I("[DOTween] Clear");

            TweenManager.PurgeAll();

            DOTweenUnityBridge.DestroyInstance();

            initialized = false;
            useSafeMode = false;
            nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
            defaultEaseType = Ease.OutQuad;
            defaultEaseOvershootOrAmplitude = 1.70158f;
            defaultEasePeriod = 0;
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

        // Sadly can't make generic versions of default tweens with additional options
        // where the TO method doesn't contain the options param, otherwise the correct Option type won't be inferred.
        // So: overloads. Sigh.
        // Also, Unity has a bug which doesn't allow method overloading with its own implicitly casteable types (like Vector4 and Color)
        // and additional parameters, so in those cases I have to create overloads instead than using optionals. ARARGH!

        #region Tween TO

        static TweenerCore<T1, T2, TPlugOptions> ApplyTo<T1, T2, TPlugOptions>(
            DOGetter<T1> getter, DOSetter<T1> setter, T2 endValue, float duration, ABSTweenPlugin<T1, T2, TPlugOptions> plugin = null
        )
            where TPlugOptions : struct, IPlugOptions
        {
            InitCheck();
            var tweener = TweenManager.GetTweener<T1, T2, TPlugOptions>();
            var setupSuccessful = Tweener.Setup(tweener, getter, setter, endValue, duration, plugin);
            if (!setupSuccessful) {
                TweenManager.Despawn(tweener);
                return null;
            }
            return tweener;
        }

        /// <summary>Tweens a property or field to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<float, float, NoOptions> To(DOGetter<float> getter, DOSetter<float> setter, float endValue, float duration)
        { return To(FloatPlugin.Instance, getter, setter, endValue, duration); }
        /// <summary>Tweens a property or field to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<int, int, NoOptions> To(DOGetter<int> getter, DOSetter<int> setter, int endValue,float duration)
        { return To(IntPlugin.Instance, getter, setter, endValue, duration); }
        /// <summary>Tweens a property or field to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<DOVector2, DOVector2, VectorOptions> To(DOGetter<DOVector2> getter, DOSetter<DOVector2> setter, Vector2 endValue, float duration)
        { return To(Vector2Plugin.Instance, getter, setter, endValue, duration); }
        /// <summary>Tweens a property or field to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<DOVector3, DOVector3, VectorOptions> To(DOGetter<DOVector3> getter, DOSetter<DOVector3> setter, Vector3 endValue, float duration)
        { return To(Vector3Plugin.Instance, getter, setter, endValue, duration); }
        /// <summary>Tweens a property or field to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<DOColor, DOColor, NoOptions> To(DOGetter<DOColor> getter, DOSetter<DOColor> setter, Color endValue, float duration)
        { return To(ColorPlugin.Instance, getter, setter, endValue, duration); }

        /// <summary>Tweens a property or field to the given value using a custom plugin</summary>
        /// <param name="plugin">The plugin to use. Each custom plugin implements a static <code>Get()</code> method
        /// you'll need to call to assign the correct plugin in the correct way, like this:
        /// <para><code>CustomPlugin.Get()</code></para></param>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<T1, T2, TPlugOptions> To<T1, T2, TPlugOptions>(
            ABSTweenPlugin<T1, T2, TPlugOptions> plugin, DOGetter<T1> getter, DOSetter<T1> setter, T2 endValue, float duration
        )
            where TPlugOptions : struct, IPlugOptions
        { return ApplyTo(getter, setter, endValue, duration, plugin); }

        /// <summary>Tweens only the alpha of a Color to the given value using default plugins</summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValue">The end value to reach</param><param name="duration">The tween's duration</param>
        public static TweenerCore<float, float, NoOptions> ToAlpha(DOGetter<Color> getter, DOSetter<Color> setter, float endValue, float duration)
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
        public static TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> Punch(DOGetter<Vector3> getter, DOSetter<Vector3> setter, Vector3 direction, float duration, int vibrato = 10, float elasticity = 1)
        {
            if (elasticity > 1) elasticity = 1;
            else if (elasticity < 0) elasticity = 0;
            float strength = direction.magnitude;
            int totIterations = (int)(vibrato * duration);
            if (totIterations < 2) totIterations = 2;
            float decayXTween = strength / totIterations;
            // Calculate and store the duration of each tween
            float[] tDurations = new float[totIterations];
            float sum = 0;
            for (int i = 0; i < totIterations; ++i) {
                float iterationPerc = (i + 1) / (float)totIterations;
                float tDuration = duration * iterationPerc;
                sum += tDuration;
                tDurations[i] = tDuration;
            }
            float tDurationMultiplier = duration / sum; // Multiplier that allows the sum of tDurations to equal the set duration
            for (int i = 0; i < totIterations; ++i) tDurations[i] = tDurations[i] * tDurationMultiplier;
            // Create the tween
            Vector3[] tos = new Vector3[totIterations];
            for (int i = 0; i < totIterations; ++i) {
                if (i < totIterations - 1) {
                    if (i == 0) tos[i] = direction;
                    else if (i % 2 != 0) tos[i] = -Vector3.ClampMagnitude(direction, strength * elasticity);
                    else tos[i] = Vector3.ClampMagnitude(direction, strength);
                    strength -= decayXTween;
                } else tos[i] = Vector3.zero;
            }
            return ToArray(getter, setter, tos, tDurations)
                .NoFrom()
                .SetSpecialStartupMode(SpecialStartupMode.SetPunch);
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
        public static TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> Shake(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration,
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
        public static TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> Shake(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration,
            Vector3 strength, int vibrato = 10, float randomness = 90,
            bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full
        )
        {
            return Shake(getter, setter, duration, strength, vibrato, randomness, false, true, fadeOut, randomnessMode);
        }
        static TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> Shake(DOGetter<Vector3> getter, DOSetter<Vector3> setter, float duration,
            Vector3 strength, int vibrato, float randomness, bool ignoreZAxis, bool vectorBased,
            bool fadeOut, ShakeRandomnessMode randomnessMode
        )
        {
            float shakeMagnitude = vectorBased ? strength.magnitude : strength.x;
            int totIterations = (int)(vibrato * duration);
            if (totIterations < 2) totIterations = 2;
            float decayXTween = shakeMagnitude / totIterations;
            // Calculate and store the duration of each tween
            float[] tDurations = new float[totIterations];
            float sum = 0;
            for (int i = 0; i < totIterations; ++i) {
                float iterationPerc = (i + 1) / (float)totIterations;
                float tDuration = fadeOut ? duration * iterationPerc : duration / totIterations;
                sum += tDuration;
                tDurations[i] = tDuration;
            }
            float tDurationMultiplier = duration / sum; // Multiplier that allows the sum of tDurations to equal the set duration
            for (int i = 0; i < totIterations; ++i) tDurations[i] = tDurations[i] * tDurationMultiplier;
            // Create the tween
            float ang = Random.Range(0f, 360f);
            Vector3[] tos = new Vector3[totIterations];
            for (int i = 0; i < totIterations; ++i) {
                if (i < totIterations - 1) {
                    Quaternion rndQuaternion = Quaternion.identity;
                    switch (randomnessMode) {
                    case ShakeRandomnessMode.Harmonic:
                        if (i > 0) ang = ang - 180 + Random.Range(0, randomness);
                        if (vectorBased || !ignoreZAxis) {
                            rndQuaternion = Quaternion.AngleAxis(Random.Range(0, randomness), Vector3.up);
                        }
                        break;
                    default: // Full
                        if (i > 0) ang = ang - 180 + Random.Range(-randomness, randomness);
                        if (vectorBased || !ignoreZAxis) {
                            rndQuaternion = Quaternion.AngleAxis(Random.Range(-randomness, randomness), Vector3.up);
                        }
                        break;
                    }
                    if (vectorBased) {
                        Vector3 to = rndQuaternion * DOTweenUtils.Vector3FromAngle(ang, shakeMagnitude);
                        to.x = Vector3.ClampMagnitude(to, strength.x).x;
                        to.y = Vector3.ClampMagnitude(to, strength.y).y;
                        to.z = Vector3.ClampMagnitude(to, strength.z).z;
                        to = to.normalized * shakeMagnitude; // Make sure first shake uses max magnitude
                        tos[i] = to;
                        if (fadeOut) shakeMagnitude -= decayXTween;
                        strength = Vector3.ClampMagnitude(strength, shakeMagnitude);
                    } else {
                        if (ignoreZAxis) {
                            tos[i] = DOTweenUtils.Vector3FromAngle(ang, shakeMagnitude);
                        } else {
                            tos[i] = rndQuaternion * DOTweenUtils.Vector3FromAngle(ang, shakeMagnitude);
                        }
                        if (fadeOut) shakeMagnitude -= decayXTween;
                    }
                } else tos[i] = Vector3.zero;
            }
            return ToArray(getter, setter, tos, tDurations)
                .NoFrom().SetSpecialStartupMode(SpecialStartupMode.SetShake);
        }

        /// <summary>Tweens a property or field to the given values using default plugins.
        /// Ease is applied between each segment and not as a whole.
        /// <para>This tween type generates some GC allocations at startup</para></summary>
        /// <param name="getter">A getter for the field or property to tween.
        /// <para>Example usage with lambda:</para><code>()=> myProperty</code></param>
        /// <param name="setter">A setter for the field or property to tween
        /// <para>Example usage with lambda:</para><code>x=> myProperty = x</code></param>
        /// <param name="endValues">The end values to reach for each segment. This array must have the same length as <code>durations</code></param>
        /// <param name="durations">The duration of each segment. This array must have the same length as <code>endValues</code></param>
        static TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> ToArray(DOGetter<Vector3> getter, DOSetter<Vector3> setter, Vector3[] endValues, float[] durations)
        {
            int len = durations.Length;
            if (len != endValues.Length) {
                Debugger.LogError("To Vector3 array tween: endValues and durations arrays must have the same length");
                return null;
            }

            // Clone the arrays
            Vector3[] endValuesClone = new Vector3[len];
            float[] durationsClone = new float[len];
            for (int i = 0; i < len; i++) {
                endValuesClone[i] = endValues[i];
                durationsClone[i] = durations[i];
            }

            float totDuration = 0;
            for (int i = 0; i < len; ++i) totDuration += durationsClone[i];
            var t = ApplyTo<Vector3, Vector3[], Vector3ArrayOptions>(getter, setter, endValuesClone, totDuration).NoFrom();
            t.plugOptions.durations = durationsClone;
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

        /// <summary>Completes all tweens with the given ID or target and returns the number of actual tweens completed
        /// (meaning the tweens that don't have infinite loops and were not already complete)</summary>
        /// <param name="withCallbacks">For Sequences only: if TRUE internal Sequence callbacks will be fired,
        /// otherwise they will be ignored</param>
        public static int Complete(object targetOrId, bool withCallbacks = false)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Complete, targetOrId, false, withCallbacks ? 1 : 0);
        }
        internal static int CompleteAndReturnKilledTot(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Complete, targetOrId, true, 0);
        }

        /// <summary>Flips the tweens with the given ID or target (changing their direction to forward if it was backwards and viceversa),
        /// then returns the number of actual tweens flipped</summary>
        public static int Flip(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Flip, targetOrId, false, 0);
        }

        /// <summary>Sends all tweens with the given ID or target to the given position (calculating also eventual loop cycles)
        /// and returns the actual tweens involved</summary>
        public static int Goto(object targetOrId, float to, bool andPlay = false)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Goto, targetOrId, andPlay, to);
        }

        /// <summary>Kills all tweens with the given ID or target and returns the number of actual tweens killed</summary>
        /// <param name="complete">If TRUE completes the tweens before killing them</param>
        public static int Kill(object targetOrId, bool complete = false)
        {
            if (targetOrId == null) return 0;
            int tot = complete ? CompleteAndReturnKilledTot(targetOrId) : 0;
            return tot + TweenManager.FilteredOperation(OperationType.Despawn, targetOrId, false, 0);
        }

        /// <summary>Pauses all tweens with the given ID or target and returns the number of actual tweens paused
        /// (meaning the tweens that were actually playing and have been paused)</summary>
        public static int Pause(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Pause, targetOrId, false, 0);
        }

        /// <summary>Plays all tweens with the given ID or target and returns the number of actual tweens played
        /// (meaning the tweens that were not already playing or complete)</summary>
        public static int Play(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Play, targetOrId, false, 0);
        }

        /// <summary>Plays backwards all tweens with the given ID or target and returns the number of actual tweens played
        /// (meaning the tweens that were not already started, playing backwards or rewinded)</summary>
        public static int PlayBackwards(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.PlayBackwards, targetOrId, false, 0);
        }

        /// <summary>Plays forward all tweens with the given ID or target and returns the number of actual tweens played
        /// (meaning the tweens that were not already playing forward or complete)</summary>
        public static int PlayForward(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.PlayForward, targetOrId, false, 0);
        }

        /// <summary>Restarts all tweens with the given ID or target, then returns the number of actual tweens restarted</summary>
        /// <param name="includeDelay">If TRUE includes the eventual tweens delays, otherwise skips them</param>
        /// <param name="changeDelayTo">If >= 0 changes the startup delay of all involved tweens to this value, otherwise doesn't touch it</param>
        public static int Restart(object targetOrId, bool includeDelay = true, float changeDelayTo = -1)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Restart, targetOrId, includeDelay, changeDelayTo);
        }

        /// <summary>Rewinds and pauses all tweens with the given ID or target, then returns the number of actual tweens rewinded
        /// (meaning the tweens that were not already rewinded)</summary>
        public static int Rewind(object targetOrId, bool includeDelay = true)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.Rewind, targetOrId, includeDelay, 0);
        }

        /// <summary>Smoothly rewinds all tweens (delays excluded) with the given ID or target, then returns the number of actual tweens rewinding/rewinded
        /// (meaning the tweens that were not already rewinded).
        /// A "smooth rewind" animates the tween to its start position,
        /// skipping all elapsed loops (except in case of LoopType.Incremental) while keeping the animation fluent.
        /// <para>Note that a tween that was smoothly rewinded will have its play direction flipped</para></summary>
        public static int SmoothRewind(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.SmoothRewind, targetOrId, false, 0);
        }

        /// <summary>Toggles the play state of all tweens with the given ID or target and returns the number of actual tweens toggled
        /// (meaning the tweens that could be played or paused, depending on the toggle state)</summary>
        public static int TogglePause(object targetOrId)
        {
            if (targetOrId == null) return 0;
            return TweenManager.FilteredOperation(OperationType.TogglePause, targetOrId, false, 0);
        }
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