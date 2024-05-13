// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 13:03
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;

namespace DG.Tweening
{
    /// <summary>
    /// Indicates either a Tweener or a Sequence
    /// </summary>
    public abstract class Tween : ABSSequentiable
    {
        public const int invalidId = -999;

        // OPTIONS ///////////////////////////////////////////////////

        // Modifiable at runtime
        /// <summary>If TRUE the tween will play backwards</summary>
        public bool isBackwards;
        /// <summary>Int ID (usable for filtering with DOTween static methods). 4X faster than using an object id, 2X faster than using a string id.
        /// Default is -999 so avoid using an ID like that or it will capture all unset intIds</summary>
        public int id = invalidId;
        /// <summary>Tween target (usable for filtering with DOTween static methods). Automatically set by tween creation shortcuts</summary>
        public object target; // Automatically set by DO shortcuts using SetTarget extension. Also used during Tweener.DoStartup in some special cases
        // Update type and eventual independence (changed via TweenManager.SetUpdateType)
        internal UpdateType updateType;
        internal bool isIndependentUpdate;
//        public TweenCallback onStart; // (in ABSSequentiable) When the tween is set in a PLAY state the first time, AFTER any eventual delay
        /// <summary>Called each time the tween updates</summary>
        public TweenCallback onUpdate;
        /// <summary>Called the moment the tween reaches completion (loops included)</summary>
        public TweenCallback onComplete;
        /// <summary>Called the moment the tween is killed</summary>
        public TweenCallback onKill;

        // Fixed after creation
        internal bool isFrom; // Used to prevent settings like isRelative from being applied on From tweens
        internal bool isBlendable; // Set by blendable tweens, prevents isRelative to be applied
        internal bool isRecyclable;
        internal bool autoKill;
        internal float duration;
        internal int loops;
        internal LoopType loopType;
        // NOW USED BY SEQUENCES TOO (since v1.2.340)
        internal float delay;
        /// <summary>Tweeners-only (ignored by Sequences), returns TRUE if the tween was set as relative</summary>
        public bool isRelative { get; internal set; } // Required by Modules
        internal Ease easeType;
        internal EaseFunction customEase; // Used both for AnimationCurve and custom eases
#pragma warning disable 1591
        public float easeOvershootOrAmplitude; // Public so it can be used with custom plugins
        public float easePeriod; // Public so it can be used with custom plugins
#pragma warning restore 1591

        // SPECIAL DEBUG DATA ////////////////////////////////////////////////
        /// <summary>
        /// Set by SetTarget if DOTween's Debug Mode is on (see DOTween Utility Panel -> "Store GameObject's ID" debug option
        /// </summary>
        public string debugTargetId;

        // SETUP DATA ////////////////////////////////////////////////

        internal Type typeofT1; // Only used by Tweeners
        internal Type typeofT2; // Only used by Tweeners
        internal Type typeofTPlugOptions; // Only used by Tweeners
        /// <summary>FALSE when tween is (or should be) despawned - set only by TweenManager</summary>
        public bool active { get; internal set; } // Required by Modules
        internal bool isSequenced; // Set by Sequence when adding a Tween to it
        internal Sequence sequenceParent;  // Set by Sequence when adding a Tween to it
        internal int activeId = -1; // Index inside its active list (touched only by TweenManager)
        internal SpecialStartupMode specialStartupMode;

        // PLAY DATA /////////////////////////////////////////////////

        /// <summary>Returns TRUE if the tween is set to loop (either a set number of times or infinitely)</summary>
        public bool hasLoops => loops is -1 or > 1;

        internal bool creationLocked; // TRUE after the tween was updated the first time (even if it was delayed), or when added to a Sequence
        internal bool startupDone; // TRUE the first time the actual tween starts, AFTER any delay has elapsed (unless it's a FROM tween)
        /// <summary>TRUE after the tween was set in a play state at least once, AFTER any delay is elapsed</summary>
        public bool playedOnce { get; private set; } // Required by Modules
        /// <summary>Time position within a single loop cycle</summary>
        public float position { get; internal set; } // Required by Modules
        internal float fullDuration; // Total duration loops included
        internal int completedLoops;
        internal bool isPlaying; // Set by TweenManager when getting a new tween
        internal bool isComplete;
        internal float elapsedDelay; // Amount of eventual delay elapsed (shared by Sequences only for compatibility reasons, otherwise not used)
        internal bool delayComplete = true; // TRUE when the delay has elapsed or isn't set, also set by Delay extension method (shared by Sequences only for compatibility reasons, otherwise not used)
        

        #region Abstracts + Overrideables

        // Doesn't reset active state, activeId and despawned, since those are only touched by TweenManager
        // Doesn't reset default values since those are set when Tweener.Setup is called
        internal virtual void Reset()
        {
            isBackwards = false;
            id = invalidId;
            isIndependentUpdate = false;
            onStart = onUpdate = onComplete = onKill = null;

            debugTargetId = null;

            target = null;
            isFrom = false;
            isBlendable = false;
            duration = 0;
            loops = 1;
            delay = 0;
            isRelative = false;
            customEase = null;
            isSequenced = false;
            sequenceParent = null;
            specialStartupMode = SpecialStartupMode.None;
            creationLocked = startupDone = playedOnce = false;
            position = fullDuration = completedLoops = 0;
            isPlaying = isComplete = false;
            elapsedDelay = 0;
            delayComplete = true;
        }

        // Called by TweenManager.Validate.
        // Returns TRUE if the tween is valid
        internal abstract bool Validate();

        // Called by TweenManager in case a tween has a delay that needs to be updated.
        // Returns the eventual time in excess compared to the tween's delay time.
        // Previously unused by Sequences but now implemented.
        // NOT TRUE ANYMORE: Shared also by Sequences even if they don't use it, in order to make it compatible with Tween.
        internal virtual float UpdateDelay(float elapsed) { return 0; }

        // Called the moment the tween starts.
        // For tweeners, that means AFTER any delay has elapsed
        // (unless it's a FROM tween, in which case it will be called BEFORE any eventual delay).
        // Returns TRUE in case of success,
        // FALSE if there are missing references and the tween needs to be killed
        internal abstract bool Startup();

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed.
        // UpdateNotice is only used by Tweeners, since Sequences re-evaluate for it
        internal abstract bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode);

        #endregion

        #region Goto and Callbacks

        // Instead of advancing the tween from the previous position each time,
        // uses the given position to calculate running time since startup, and places the tween there like a Goto.
        // Executes regardless of whether the tween is playing.
        // Returns TRUE if the tween needs to be killed
        internal static bool DoGoto(Tween t, float toPosition, int toCompletedLoops, UpdateMode updateMode)
        {
            // Startup
            if (!t.startupDone) {
                if (!t.Startup()) return true;
            }
            // OnStart and first OnPlay callbacks
            if (!t.playedOnce && updateMode == UpdateMode.Update) {
                t.playedOnce = true;
                if (t.onStart != null) {
                    OnTweenCallback(t.onStart, t);
                    if (!t.active) return true; // Tween might have been killed by onStart callback
                }
            }

            float prevPosition = t.position;
            int prevCompletedLoops = t.completedLoops;
            t.completedLoops = toCompletedLoops;
            bool wasRewinded = t.position <= 0 && prevCompletedLoops <= 0;
            bool wasComplete = t.isComplete;
            // Determine if it will be complete after update
            if (t.loops != -1) t.isComplete = t.completedLoops == t.loops;
            // Calculate newCompletedSteps (always useful with Sequences)
            int newCompletedSteps = 0;
            if (updateMode == UpdateMode.Update) {
                if (t.isBackwards) {
                    newCompletedSteps = t.completedLoops < prevCompletedLoops ? prevCompletedLoops - t.completedLoops : (toPosition <= 0 && !wasRewinded ? 1 : 0);
                    if (wasComplete) newCompletedSteps--;
                } else newCompletedSteps = t.completedLoops > prevCompletedLoops ? t.completedLoops - prevCompletedLoops : 0;
            } else if (t.tweenType == TweenType.Sequence) {
                newCompletedSteps = prevCompletedLoops - toCompletedLoops;
                if (newCompletedSteps < 0) newCompletedSteps = -newCompletedSteps;
            }

            // Set position (makes position 0 equal to position "end" when looping)
            t.position = toPosition;
            if (t.position > t.duration) t.position = t.duration;
            else if (t.position <= 0) {
                if (t.completedLoops > 0 || t.isComplete) t.position = t.duration;
                else t.position = 0;
            }
            // Set playing state after update
            if (t.isPlaying) {
                if (!t.isBackwards) t.isPlaying = !t.isComplete; // Reached the end
                else t.isPlaying = !(t.completedLoops == 0 && t.position <= 0); // Rewinded
            }

            // updatePosition is different in case of Yoyo loop under certain circumstances
            bool useInversePosition = t.hasLoops && t.loopType == LoopType.Yoyo
                && (t.position < t.duration ? t.completedLoops % 2 != 0 : t.completedLoops % 2 == 0);

            // Get values from plugin and set them
            if (t.ApplyTween(prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition, updateMode)) return true;

            // Additional callbacks
            if (t.onUpdate != null && updateMode != UpdateMode.IgnoreOnUpdate) {
                OnTweenCallback(t.onUpdate, t);
            }
            if (t.isComplete && !wasComplete && updateMode != UpdateMode.IgnoreOnComplete && t.onComplete != null) {
                OnTweenCallback(t.onComplete, t);
            }

            // Return
            return t.autoKill && t.isComplete;
        }

        // Assumes that the callback exists (because it was previously checked).
        // Returns TRUE in case of success, FALSE in case of error (if safeMode is on)
        internal static bool OnTweenCallback(TweenCallback callback, Tween t)
        {
            if (DOTween.useSafeMode) {
                try {
                    callback();
                } catch (Exception e) {
                    Debugger.LogSafeModeCapturedError(string.Format(
                        "An error inside a tween callback was taken care of ({0}) ► {1}\n\n{2}\n\n", e.TargetSite, e.Message, e.StackTrace
                    ), t);
                    return false; // Callback error
                }
            } else callback();
            return true;
        }

        #endregion
    }
}