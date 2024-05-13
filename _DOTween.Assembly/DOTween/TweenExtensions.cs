// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/05 18:31
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using UnityEngine;

#pragma warning disable 1573
namespace DG.Tweening
{
    /// <summary>
    /// Methods that extend Tween objects and allow to control or get data from them
    /// </summary>
    public static class TweenExtensions
    {
        // ===================================================================================
        // TWEENERS + SEQUENCES --------------------------------------------------------------

        #region Runtime Operations

//        /// <summary>Completes the tween</summary>
//        /// <param name="withCallbacks">For Sequences only: if TRUE also internal Sequence callbacks will be fired,
//        /// otherwise they will be ignored</param>
//        public static void Complete(this Tween t, bool withCallbacks = false)
//        {
//            if (t == null) {
//                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
//            } else if (!t.active) {
//                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
//            } else if (t.isSequenced) {
//                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
//            }
//
//            TweenManager.Complete(t, true, withCallbacks ? UpdateMode.Update : UpdateMode.Goto);
//        }
        /// <summary>Completes the tween</summary>
        public static void Complete(this Tween t)
        { Complete(t, false); }
        /// <summary>Completes the tween</summary>
        /// <param name="withCallbacks">For Sequences only: if TRUE also internal Sequence callbacks will be fired,
        /// otherwise they will be ignored</param>
        public static void Complete(this Tween t, bool withCallbacks)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            // Previously disused in favor of bottom code because otherwise OnComplete was called twice when fired inside am OnUpdate call,
            // but that created another recent issue where events (OnComplete and any other) weren't called anymore
            // if called from another tween's internal callbacks.
            // Reinstated because thanks to other fixes this now works correctly
            TweenManager.Complete(t, true, withCallbacks ? UpdateMode.Update : UpdateMode.Goto);
            // See above note for reason why this was commented
            // UpdateMode updateMode = TweenManager.isUpdateLoop ? UpdateMode.IgnoreOnComplete
            //     : withCallbacks ? UpdateMode.Update : UpdateMode.Goto;
            // TweenManager.Complete(t, true, updateMode);
        }
        
        /// <summary>Optional: indicates that the tween creation has ended, to be used (optionally) as the last element of tween chaining creation.<br/>
        /// This method won't do anything except in case of 0-duration tweens,
        /// where it will complete them immediately instead of waiting for the next internal update routine
        /// (unless they're nested in a Sequence, in which case the Sequence will still be the one in control and this method will be ignored)</summary>
        public static void Done(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }
            
            if (t.duration <= 0) TweenManager.Complete(t);
        }

        /// <summary>Flips the direction of this tween (backwards if it was going forward or viceversa)</summary>
        public static void Flip(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.Flip(t);
        }

        /// <summary>Forces the tween to initialize its settings immediately</summary>
        public static void ForceInit(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.ForceInit(t);
        }

        /// <summary>Send the tween to the given position in time</summary>
        /// <param name="to">Time position to reach
        /// (if higher than the whole tween duration the tween will simply reach its end)</param>
        /// <param name="andPlay">If TRUE will play the tween after reaching the given position, otherwise it will pause it</param>
        public static void Goto(this Tween t, float to, bool andPlay = false)
        { DoGoto(t, to, andPlay, false); }
        /// <summary>Send the tween to the given position in time while also executing any callback between the previous time position and the new one</summary>
        /// <param name="to">Time position to reach
        /// (if higher than the whole tween duration the tween will simply reach its end)</param>
        /// <param name="andPlay">If TRUE will play the tween after reaching the given position, otherwise it will pause it</param>
        public static void GotoWithCallbacks(this Tween t, float to, bool andPlay = false)
        { DoGoto(t, to, andPlay, true); }
        static void DoGoto(Tween t, float to, bool andPlay, bool withCallbacks)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            if (to < 0) to = 0;
            if (!t.startupDone) TweenManager.ForceInit(t); // Initialize the tween if it's not initialized already (required)
            TweenManager.Goto(t, to, andPlay, withCallbacks ? UpdateMode.Update : UpdateMode.Goto);
        }

        /// <summary>Kills the tween</summary>
        /// <param name="complete">If TRUE completes the tween before killing it</param>
        public static void Kill(this Tween t, bool complete = false)
        {
            if (!DOTween.initialized) return;
            if (t == null || !t.active) {
                return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            if (complete) {
                TweenManager.Complete(t);
                if (t.autoKill && t.loops >= 0) return; // Already killed by Complete, so no need to go on
            }

            if (TweenManager.isUpdateLoop) {
                // Just mark it for killing, so the update loop will take care of it
                t.active = false;
            } else TweenManager.Despawn(t);
        }

        public static void KillRewind(this Tweener t)
        {
            if (!DOTween.initialized) return;
            if (t == null || !t.active) return;

            TweenManager.RestoreToOriginal(t);

            if (TweenManager.isUpdateLoop) {
                // Just mark it for killing, so the update loop will take care of it
                t.active = false;
            } else TweenManager.Despawn(t);
        }

        /// <summary>Pauses the tween</summary>
        public static T Pause<T>(this T t) where T : Tween
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return t;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return t;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return t;
            }

            TweenManager.Pause(t);
            return t;
        }

        /// <summary>Plays the tween</summary>
        public static T Play<T>(this T t) where T : Tween
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return t;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return t;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return t;
            }

            TweenManager.Play(t);
            return t;
        }

        /// <summary>Sets the tween in a backwards direction and plays it</summary>
        public static void PlayBackwards(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.PlayBackwards(t);
        }

        /// <summary>Sets the tween in a forward direction and plays it</summary>
        public static void PlayForward(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.PlayForward(t);
        }

        /// <summary>Restarts the tween from the beginning</summary>
        /// <param name="includeDelay">Ignored in case of Sequences. If TRUE includes the eventual tween delay, otherwise skips it</param>
        /// <param name="changeDelayTo">Ignored in case of Sequences. If >= 0 changes the startup delay to this value, otherwise doesn't touch it</param>
        public static void Restart(this Tween t, bool includeDelay = true, float changeDelayTo = -1)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.Restart(t, includeDelay, changeDelayTo);
        }

        /// <summary>Rewinds and pauses the tween</summary>
        /// <param name="includeDelay">Ignored in case of Sequences. If TRUE includes the eventual tween delay, otherwise skips it</param>
        public static void Rewind(this Tween t, bool includeDelay = true)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.Rewind(t, includeDelay);
        }

        /// <summary>Smoothly rewinds the tween (delays excluded).
        /// A "smooth rewind" animates the tween to its start position,
        /// skipping all elapsed loops (except in case of LoopType.Incremental) while keeping the animation fluent.
        /// If called on a tween who is still waiting for its delay to happen, it will simply set the delay to 0 and pause the tween.
        /// <para>Note that a tween that was smoothly rewinded will have its play direction flipped</para></summary>
        public static void SmoothRewind(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.SmoothRewind(t);
        }

        /// <summary>Plays the tween if it was paused, pauses it if it was playing</summary>
        public static void TogglePause(this Tween t)
        {
            if (t == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                if (Debugger.logPriority > 1) Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                if (Debugger.logPriority > 1) Debugger.LogNestedTween(t); return;
            }

            TweenManager.TogglePause(t);
        }

        #endregion

        #region Info Getters

        /// <summary>Returns the total number of loops completed by this tween</summary>
        public static int CompletedLoops(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.completedLoops;
        }

        /// <summary>Returns the eventual delay set for this tween</summary>
        public static float Delay(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.delay;
        }

        /// <summary>Returns the eventual elapsed delay set for this tween</summary>
        public static float ElapsedDelay(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.elapsedDelay;
        }

        /// <summary>Returns the duration of this tween (delays excluded).
        /// <para>NOTE: when using settings like SpeedBased, the duration will be recalculated when the tween starts</para></summary>
        /// <param name="includeLoops">If TRUE returns the full duration loops included,
        ///  otherwise the duration of a single loop cycle</param>
        public static float Duration(this Tween t, bool includeLoops = true)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            // Calculate duration here instead than getting fullDuration because fullDuration might
            // not be set yet, since it's set inside DoStartup
            if (includeLoops) return t.loops == -1 ? Mathf.Infinity : t.duration * t.loops;
            return t.duration;
        }

        /// <summary>Returns the elapsed percentage (0 to 1) of this tween (delays exluded)</summary>
        /// <param name="includeLoops">If TRUE returns the elapsed percentage since startup loops included,
        /// otherwise the elapsed percentage within the current loop cycle</param>
        public static float ElapsedPercentage(this Tween t, bool includeLoops = true)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            if (includeLoops) {
                if (t.fullDuration <= 0) return 0;
                int loopsToCount = t.position >= t.duration ? t.completedLoops - 1 : t.completedLoops;
                return ((loopsToCount * t.duration) + t.position) / t.fullDuration;
            }
            return t.position / t.duration;
        }
        /// <summary>Returns the elapsed percentage (0 to 1) of this tween (delays exluded),
        /// based on a single loop, and calculating eventual backwards Yoyo loops as 1 to 0 instead of 0 to 1</summary>
        public static float ElapsedDirectionalPercentage(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            float perc = t.position / t.duration;
            bool isInverse = t.completedLoops > 0 && t.hasLoops && t.loopType == LoopType.Yoyo
                             && (!t.isComplete && t.completedLoops % 2 != 0 || t.isComplete && t.completedLoops % 2 == 0);
            return isInverse ? 1 - perc : perc;
        }

        /// <summary>Returns FALSE if this tween has been killed or is NULL, TRUE otherwise.
        /// <para>BEWARE: if this tween is recyclable it might have been spawned again for another use and thus return TRUE anyway.</para>
        /// When working with recyclable tweens you should take care to know when a tween has been killed and manually set your references to NULL.
        /// If you want to be sure your references are set to NULL when a tween is killed you can use the <code>OnKill</code> callback like this:
        /// <para><code>.OnKill(()=> myTweenReference = null)</code></para></summary>
        public static bool IsActive(this Tween t)
        {
            return t != null && t.active;
        }

        /// <summary>Returns TRUE if this tween was reversed and is set to go backwards</summary>
        public static bool IsBackwards(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return false;
            }

            return t.isBackwards;
        }

        /// <summary>NOTE: To check if a tween was simply set to go backwards see <see cref="IsBackwards"/>.<para/>
        /// Returns TRUE if this tween is going backwards for any of these reasons:<para/>
        /// - The tween was reversed and is going backwards on a straight loop<para/>
        /// - The tween was reversed and is going backwards on an odd Yoyo loop<para/>
        /// - The tween is going forward but on an even Yoyo loop<para/>
        /// IMPORTANT: if used inside a tween's callback, this will return a result concerning the exact frame when it's asked,
        /// so for example in a callback at the end of a Yoyo loop step this method will never return FALSE
        /// because the frame will never end exactly there and the tween will already be going backwards when the callback is fired</summary>
        public static bool IsLoopingOrExecutingBackwards(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return false;
            }

            if (t.isBackwards) {
                return t.completedLoops < 1 || t.loopType != LoopType.Yoyo || t.completedLoops % 2 == 0;
            } else {
                return t.completedLoops >= 1 && t.loopType == LoopType.Yoyo && t.completedLoops % 2 != 0;
            }
        }

        /// <summary>Returns TRUE if the tween is complete
        /// (silently fails and returns FALSE if the tween has been killed)</summary>
        public static bool IsComplete(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return false;
            }

            return t.isComplete;
        }

        /// <summary>Returns TRUE if this tween has been initialized</summary>
        public static bool IsInitialized(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return false;
            }

            return t.startupDone;
        }

        /// <summary>Returns TRUE if this tween is playing</summary>
        public static bool IsPlaying(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return false;
            }

            return t.isPlaying;
        }

        /// <summary>Returns the total number of loops set for this tween
        /// (returns -1 if the loops are infinite)</summary>
        public static int Loops(this Tween t)
        {
            if (!t.active) {
                if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.loops;
        }

        #endregion
    }
}