// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/05 18:31
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
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

        /// <summary>Completes the tween</summary>
        /// <param name="withCallbacks">For Sequences only: if TRUE also internal Sequence callbacks will be fired,
        /// otherwise they will be ignored</param>
        public static void Complete(this Tween t, bool withCallbacks)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.Complete(t, withCallbacks ? UpdateMode.Update : UpdateMode.Goto);
        }

        /// <summary>Flips the direction of this tween (backwards if it was going forward or viceversa)</summary>
        public static void Flip(this Tween t)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.Flip(t);
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
        private static void DoGoto(Tween t, float to, bool andPlay, bool withCallbacks)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            if (to < 0) to = 0;
            if (!t.startupDone) TweenManager.ForceInit(t); // Initialize the tween if it's not initialized already (required)
            TweenManager.Goto(t, to, andPlay, withCallbacks ? UpdateMode.Update : UpdateMode.Goto);
        }

        /// <summary>Kills the tween</summary>
        /// <param name="complete">If TRUE completes the tween before killing it</param>
        public static void Kill(this Tween t, bool complete = false)
        {
            if (t is not { active: true })
                return;
            if (t.isSequenced) {
                Debugger.LogNestedTween(t);
                return;
            }

            if (complete) {
                TweenManager.Complete(t);
                if (t.autoKill && t.loops >= 0) return; // Already killed by Complete, so no need to go on
            }

            TweenManager.KillTween(t);
        }

        public static void KillRewind(this Tweener t)
        {
            if (t is not { active: true })
            {
                L.E("Tween is not active.");
                return;
            }

            TweenManager.RestoreToOriginal(t);
            TweenManager.KillTween(t);
        }

        /// <summary>Pauses the tween</summary>
        public static T Pause<T>(this T t) where T : Tween
        {
            if (t == null) {
                Debugger.LogNullTween(t); return t;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return t;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return t;
            }

            TweenManager.Pause(t);
            return t;
        }

        /// <summary>Plays the tween</summary>
        public static T Play<T>(this T t) where T : Tween
        {
            if (t == null) {
                Debugger.LogNullTween(t); return t;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return t;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return t;
            }

            TweenManager.Play(t);
            return t;
        }

        /// <summary>Sets the tween in a backwards direction and plays it</summary>
        public static void PlayBackwards(this Tween t)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.PlayBackwards(t);
        }

        /// <summary>Sets the tween in a forward direction and plays it</summary>
        public static void PlayForward(this Tween t)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.PlayForward(t);
        }

        /// <summary>Restarts the tween from the beginning</summary>
        /// <param name="includeDelay">Ignored in case of Sequences. If TRUE includes the eventual tween delay, otherwise skips it</param>
        /// <param name="changeDelayTo">Ignored in case of Sequences. If >= 0 changes the startup delay to this value, otherwise doesn't touch it</param>
        public static void Restart(this Tween t, bool includeDelay = true, float changeDelayTo = -1)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.Restart(t, includeDelay, changeDelayTo);
        }

        /// <summary>Rewinds and pauses the tween</summary>
        /// <param name="includeDelay">Ignored in case of Sequences. If TRUE includes the eventual tween delay, otherwise skips it</param>
        public static void Rewind(this Tween t, bool includeDelay = true)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.Rewind(t, includeDelay);
        }

        /// <summary>Plays the tween if it was paused, pauses it if it was playing</summary>
        public static void TogglePause(this Tween t)
        {
            if (t == null) {
                Debugger.LogNullTween(t); return;
            } else if (!t.active) {
                Debugger.LogInvalidTween(t); return;
            } else if (t.isSequenced) {
                Debugger.LogNestedTween(t); return;
            }

            TweenManager.TogglePause(t);
        }

        #endregion

        #region Info Getters

        /// <summary>Returns the total number of loops completed by this tween</summary>
        public static int CompletedLoops(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.completedLoops;
        }

        /// <summary>Returns the eventual delay set for this tween</summary>
        public static float Delay(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.delay;
        }

        /// <summary>Returns the eventual elapsed delay set for this tween</summary>
        public static float ElapsedDelay(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
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
                Debugger.LogInvalidTween(t);
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
                Debugger.LogInvalidTween(t);
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
                Debugger.LogInvalidTween(t);
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
                Debugger.LogInvalidTween(t);
                return false;
            }

            return t.isBackwards;
        }

        /// <summary>Returns TRUE if the tween is complete
        /// (silently fails and returns FALSE if the tween has been killed)</summary>
        public static bool IsComplete(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
                return false;
            }

            return t.isComplete;
        }

        /// <summary>Returns TRUE if this tween has been initialized</summary>
        public static bool IsInitialized(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
                return false;
            }

            return t.startupDone;
        }

        /// <summary>Returns TRUE if this tween is playing</summary>
        public static bool IsPlaying(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
                return false;
            }

            return t.isPlaying;
        }

        /// <summary>Returns the total number of loops set for this tween
        /// (returns -1 if the loops are infinite)</summary>
        public static int Loops(this Tween t)
        {
            if (!t.active) {
                Debugger.LogInvalidTween(t);
                return 0;
            }

            return t.loops;
        }

        #endregion
    }
}