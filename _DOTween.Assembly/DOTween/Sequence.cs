﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/15 17:50
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    /// <summary>
    /// Controls other tweens as a group
    /// </summary>
    public sealed class Sequence : Tween
    {
        // SETUP DATA ////////////////////////////////////////////////

        internal readonly List<Tween> sequencedTweens = new(); // Only Tweens (used for despawning and validation)
        private readonly List<ABSSequentiable> _sequencedObjs = new(); // Tweens plus SequenceCallbacks

        #region Constructor

        internal Sequence()
        {
            Reset();
        }

        // Called by DOTween when spawning/creating a new Sequence.
        internal void Setup()
        {
            autoKill = true;
            isPlaying = true;
            loopType = LoopType.Restart;
            easeType = Ease.Linear;
            easeOvershootOrAmplitude = Config.defaultEaseOvershootOrAmplitude;
            easePeriod = 0;
        }

        #endregion

        #region Creation Methods

        internal static Sequence DoPrepend(Sequence inSequence, Tween t)
        {
            if (t.loops == -1) {
                t.loops = int.MaxValue;
                Debugger.LogWarning("Infinite loops aren't allowed inside a Sequence (only on the Sequence itself) and will be changed to int.MaxValue", t);
            }
            float tFullTime = t.delay + (t.duration * t.loops);
//            float tFullTime = t.duration * t.loops;
            inSequence.duration += tFullTime;
            int len = inSequence._sequencedObjs.Count;
            for (int i = 0; i < len; ++i) {
                var sequentiable = inSequence._sequencedObjs[i];
                sequentiable.sequencedPosition += tFullTime;
                sequentiable.sequencedEndPosition += tFullTime;
            }

            return DoInsert(inSequence, t, 0);
        }

        internal static Sequence DoInsert(Sequence inSequence, Tween t, float atPosition)
        {
            Assert.IsTrue(t.updateId.IsValid(), "Tween has an invalid updateId");
            TweenManager.DetachTween(t);

            // If t has a delay add it as an interval
            atPosition += t.delay;

            t.isSequenced = t.creationLocked = true;
            if (t.loops == -1) {
                t.loops = int.MaxValue;
                Debugger.LogWarning("Infinite loops aren't allowed inside a Sequence (only on the Sequence itself) and will be changed to int.MaxValue", t);
            }
            float tFullTime = t.duration * t.loops;
            t.autoKill = false;
            t.delay = t.elapsedDelay = 0;
            t.delayComplete = true;
            t.sequencedPosition = atPosition;
            t.sequencedEndPosition = atPosition + tFullTime;

            if (t.sequencedEndPosition > inSequence.duration) inSequence.duration = t.sequencedEndPosition;
            inSequence._sequencedObjs.Add(t);
            inSequence.sequencedTweens.Add(t);

            return inSequence;
        }

        internal static Sequence DoAppendInterval(Sequence inSequence, float interval)
        {
            inSequence.duration += interval;
            return inSequence;
        }

        internal static Sequence DoPrependInterval(Sequence inSequence, float interval)
        {
            inSequence.duration += interval;
            int len = inSequence._sequencedObjs.Count;
            for (int i = 0; i < len; ++i) {
                var sequentiable = inSequence._sequencedObjs[i];
                sequentiable.sequencedPosition += interval;
                sequentiable.sequencedEndPosition += interval;
            }

            return inSequence;
        }

        internal static Sequence DoInsertCallback(Sequence inSequence, TweenCallback callback, float atPosition)
        {
            var c = new SequenceCallback(atPosition, callback);
            c.sequencedPosition = c.sequencedEndPosition = atPosition;
            inSequence._sequencedObjs.Add(c);
            if (inSequence.duration < atPosition) inSequence.duration = atPosition;
            return inSequence;
        }

        #endregion

        internal override void Reset()
        {
            base.Reset();

#if DEBUG
            foreach (var t in sequencedTweens)
            {
                Assert.IsFalse(t.active, "Tween is still active");
                Assert.IsTrue(t.updateId.IsInvalid(), "Tween has a valid updateId");
            }
#endif

            sequencedTweens.Clear();
            _sequencedObjs.Clear();
        }

        // CALLED BY Tween the moment the tween starts.
        // Returns TRUE in case of success
        internal override bool Startup()
        {
            if (sequencedTweens.Count == 0 && _sequencedObjs.Count == 0 && !IsAnyCallbackSet(this)) {
                return false; // Empty Sequence without any callback set
            }

            startupDone = true;
            fullDuration = loops > -1 ? duration * loops : Mathf.Infinity;
            // Order sequencedObjs by start position
            StableSortSequencedObjs(_sequencedObjs);

            // Set relative nested tweens
            if (isRelative) {
                for (int len = sequencedTweens.Count, i = 0; i < len; ++i) {
                    sequencedTweens[i].isRelative = true;
                }
            }
            return true;

            static void StableSortSequencedObjs(List<ABSSequentiable> list)
            {
                var len = list.Count;
                for (var i = 1; i < len; i++) {
                    var j = i;
                    var temp = list[i];
                    while (j > 0 && list[j - 1].sequencedPosition > temp.sequencedPosition) {
                        list[j] = list[j - 1];
                        j -= 1;
                    }
                    list[j] = temp;
                }
            }
        }

        internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode)
        {
            return DoApplyTween(this, prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition, updateMode);
        }

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed
        internal static bool DoApplyTween(Sequence s, float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode)
        {
            // Adapt to eventual ease position
            float prevPos = prevPosition;
            float newPos = s.position;
            if (s.easeType != Ease.Linear) {
                prevPos = s.duration * EaseManager.Evaluate(s.easeType, s.customEase, prevPos, s.duration, s.easeOvershootOrAmplitude, s.easePeriod);
                newPos = s.duration * EaseManager.Evaluate(s.easeType, s.customEase, newPos, s.duration, s.easeOvershootOrAmplitude, s.easePeriod);
            }

            float from, to = 0;
            // Determine if prevPos was inverse.
            // Used to calculate correct "from" value when applying internal cycle
            // and also in case of multiple loops within a single update
            bool prevPosIsInverse = s.hasLoops && s.loopType == LoopType.Yoyo
                && (prevPos < s.duration ? prevCompletedLoops % 2 != 0 : prevCompletedLoops % 2 == 0);
            if (s.isBackwards) prevPosIsInverse = !prevPosIsInverse;
            // Update multiple loop cycles within the same update
            if (newCompletedSteps > 0) {
                // Store expected completedLoops and position, in order to check them after the update cycles.
                int expectedCompletedLoops = s.completedLoops;
                float expectedPosition = s.position;
                //
                int cycles = newCompletedSteps;
                int cyclesDone = 0;
                from = prevPos;
                if (updateMode == UpdateMode.Update) {
                    // Run all cycles elapsed since last update
                    while (cyclesDone < cycles) {
                        if (cyclesDone > 0) from = to;
                        else if (prevPosIsInverse && !s.isBackwards) from = s.duration - from;
                        to = prevPosIsInverse ? 0 : s.duration;
                        if (ApplyInternalCycle(s, from, to, updateMode, useInversePosition, prevPosIsInverse, true)) return true;
                        cyclesDone++;
                        if (s.hasLoops && s.loopType == LoopType.Yoyo) prevPosIsInverse = !prevPosIsInverse;
                    }
                    // If completedLoops or position were changed by some callback, exit here
                    if (expectedCompletedLoops != s.completedLoops || Math.Abs(expectedPosition - s.position) > Single.Epsilon) return !s.active;
                } else {
                    // Simply determine correct prevPosition after steps
                    if (s.hasLoops && s.loopType == LoopType.Yoyo && newCompletedSteps % 2 != 0) {
                        prevPosIsInverse = !prevPosIsInverse;
                        prevPos = s.duration - prevPos;
                    }
                    newCompletedSteps = 0;
                }
            }
            // Run current cycle
            if (newCompletedSteps == 1 && s.isComplete) return false; // Skip update if complete because multicycle took care of it
            if (newCompletedSteps > 0 && !s.isComplete) {
                from = useInversePosition ? s.duration : 0;
                // In case of Restart loop rewind all tweens (keep "to > 0" or remove it?)
                if (s.loopType == LoopType.Restart && to > 0) ApplyInternalCycle(s, s.duration, 0, UpdateMode.Goto, false, false, false);
            } else from = useInversePosition ? s.duration - prevPos : prevPos;
            return ApplyInternalCycle(s, from, useInversePosition ? s.duration - newPos : newPos, updateMode, useInversePosition, prevPosIsInverse);
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        // Returns TRUE if the tween needs to be killed
        private static bool ApplyInternalCycle(Sequence s, float fromPos, float toPos, UpdateMode updateMode, bool useInverse, bool prevPosIsInverse, bool multiCycleStep = false)
        {
            bool wasPlaying = s.isPlaying; // Used to interrupt for loops in case a callback pauses a running Sequence
            bool isBackwardsUpdate = toPos < fromPos;
            if (isBackwardsUpdate) {
                int len = s._sequencedObjs.Count - 1;
                for (int i = len; i > -1; --i) {
                    if (!s.active) return true; // Killed by some internal callback
                    if (!s.isPlaying && wasPlaying) return false; // Paused by internal callback
                    ABSSequentiable sequentiable = s._sequencedObjs[i];
                    if (sequentiable.sequencedEndPosition < toPos || sequentiable.sequencedPosition > fromPos) continue;
                    if (sequentiable is SequenceCallback) {
                        if (updateMode == UpdateMode.Update && prevPosIsInverse) {
                            OnTweenCallback(sequentiable.onStart, s);
                        }
                    } else {
                        // Nested Tweener/Sequence
                        float gotoPos = toPos - sequentiable.sequencedPosition;
                        if (gotoPos < 0) gotoPos = 0;
                        Tween t = (Tween)sequentiable;
                        if (!t.startupDone) continue; // since we're going backwards and this tween never started just ignore it
                        t.isBackwards = true;
                        if (TweenManager.Goto(t, gotoPos, false, updateMode)) {
                            // Nested tween failed. If it's the only tween and there's no callbacks mark for killing the whole sequence
                            // (default behaviour in any case prior to v1.2.060)...
                            return true;
                        }

                        // Fixes nested callbacks not being called correctly if main sequence has loops and nested ones don't
                        if (multiCycleStep && t is Sequence) {
                            if (s.position <= 0 && s.completedLoops == 0) t.position = 0;
                            else {
                                bool toZero = s.completedLoops == 0 || s.isBackwards && (s.completedLoops < s.loops || s.loops == -1);
                                if (t.isBackwards) toZero = !toZero;
                                if (useInverse) toZero = !toZero;
                                if (s.isBackwards && !useInverse && !prevPosIsInverse) toZero = !toZero;
                                t.position = toZero ? 0 : t.duration;
                            }
                        }
                    }
                }
            } else {
                int len = s._sequencedObjs.Count;
                for (int i = 0; i < len; ++i) {
                    if (!s.active) return true; // Killed by some internal callback
                    if (!s.isPlaying && wasPlaying) return false; // Paused by internal callback
                    ABSSequentiable sequentiable = s._sequencedObjs[i];
                    // Fix rare case with high FPS when a tween/callback might happen in same exact time as it's set
                    // This fixes it but should check for backwards tweens and loops
                    if (
                        sequentiable.sequencedPosition > toPos
                        || sequentiable.sequencedPosition > 0 && sequentiable.sequencedEndPosition <= fromPos
                        || sequentiable.sequencedPosition <= 0 && sequentiable.sequencedEndPosition < fromPos
                    ) continue;
                    if (sequentiable is SequenceCallback) {
                        if (updateMode == UpdateMode.Update) {
                            bool fire = !s.isBackwards && !useInverse && !prevPosIsInverse
                                || s.isBackwards && useInverse && !prevPosIsInverse;
                            if (fire) OnTweenCallback(sequentiable.onStart, s);
                        }
                    } else {
                        // Nested Tweener/Sequence
                        float gotoPos = toPos - sequentiable.sequencedPosition;
                        if (gotoPos < 0) gotoPos = 0;
                        Tween t = (Tween)sequentiable;
                        // Fix for final nested tween not calling OnComplete in some cases
                        if (toPos >= sequentiable.sequencedEndPosition) {
                            if (!t.startupDone) TweenManager.ForceInit(t, true);
                            if (gotoPos < t.fullDuration) gotoPos = t.fullDuration;
                        }
                        //
                        t.isBackwards = false;
                        if (TweenManager.Goto(t, gotoPos, false, updateMode)) {
                            // Nested tween failed. If it's the only tween and there's no callbacks mark for killing the whole sequence
                            // (default behaviour in any case prior to v1.2.060)...
                            return true;
                        }

                        // Fixes nested callbacks not being called correctly if main sequence has loops and nested ones don't
                        if (multiCycleStep && t is Sequence) {
                            if (s.position <= 0 && s.completedLoops == 0) t.position = 0;
                            else {
                                bool toZero = s.completedLoops == 0 || !s.isBackwards && (s.completedLoops < s.loops || s.loops == -1);
                                if (t.isBackwards) toZero = !toZero;
                                if (useInverse) toZero = !toZero;
                                if (s.isBackwards && !useInverse && !prevPosIsInverse) toZero = !toZero;
                                t.position = toZero ? 0 : t.duration;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsAnyCallbackSet(Sequence s)
        {
            return s.onComplete != null || s.onKill != null
                   || s.onStart != null || s.onUpdate != null;
        }
    }
}