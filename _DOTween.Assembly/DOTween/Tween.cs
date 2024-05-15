// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 13:03
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Core;
using UnityEngine.Assertions;

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
        /// <summary>Called each time the tween updates</summary>
        public TweenCallback onUpdate;
        /// <summary>Called the moment the tween reaches completion (loops included)</summary>
        public TweenCallback onComplete;
        /// <summary>Called the moment the tween is killed</summary>
        public TweenCallback onKill;

        // Fixed after creation
        internal bool isFrom; // Used to prevent settings like isRelative from being applied on From tweens
        internal bool isBlendable; // Set by blendable tweens, prevents isRelative to be applied
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

#if DEBUG
        public string debugHint;
#endif

        // SETUP DATA ////////////////////////////////////////////////

        /// <summary>FALSE when tween is (or should be) despawned - set only by TweenManager</summary>
        public bool active { get; internal set; } // Required by Modules
        internal bool isSequenced; // Set by Sequence when adding a Tween to it
        internal TweenUpdateId updateId = TweenUpdateId.Invalid; // Index inside its active list (touched only by TweenManager)

        // PLAY DATA /////////////////////////////////////////////////

        /// <summary>Returns TRUE if the tween is set to loop (either a set number of times or infinitely)</summary>
        public bool hasLoops => loops is -1 or > 1;
        internal int completedLoops;

        internal bool creationLocked; // TRUE after the tween was updated the first time (even if it was delayed), or when added to a Sequence
        internal bool startupDone; // TRUE the first time the actual tween starts, AFTER any delay has elapsed (unless it's a FROM tween)
        /// <summary>TRUE after the tween was set in a play state at least once, AFTER any delay is elapsed</summary>
        public bool playedOnce { get; private set; } // Required by Modules
        /// <summary>Time position within a single loop cycle</summary>
        public float position { get; internal set; } // Required by Modules
        internal float fullDuration; // Total duration loops included
        internal bool isPlaying; // Set by TweenManager when getting a new tween
        internal bool isComplete;
        internal float elapsedDelay; // Amount of eventual delay elapsed (shared by Sequences only for compatibility reasons, otherwise not used)
        internal bool delayComplete = true; // TRUE when the delay has elapsed or isn't set, also set by Delay extension method (shared by Sequences only for compatibility reasons, otherwise not used)


        public override string ToString()
        {
#if DEBUG
            return $"[debugHint={debugHint ?? ""}, target={target?.ToString() ?? ""}, targetType={target?.GetType().Name ?? ""}, id={id}, tweenType={GetType().Name}]";
#else
            return $"[target={target?.ToString() ?? ""}, targetType={target?.GetType().Name ?? ""}, id={id}, tweenType={GetType().Name}]";
#endif
        }

        #region Abstracts + Overrideables

        // Doesn't reset active state, activeId and despawned, since those are only touched by TweenManager
        // Doesn't reset default values since those are set when Tweener.Setup is called
        internal virtual void Reset()
        {
            isBackwards = false;
            id = invalidId;
            onStart = onUpdate = onComplete = onKill = null;

#if DEBUG
            debugHint = null;
#endif

            target = null;
            isFrom = false;
            isBlendable = false;
            duration = 0;
            loops = 1;
            delay = 0;
            isRelative = false;
            customEase = null;
            isSequenced = false;
            creationLocked = startupDone = playedOnce = false;
            position = fullDuration = completedLoops = 0;
            isPlaying = isComplete = false;
            elapsedDelay = 0;
            delayComplete = true;
        }

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
        internal bool ForceGoto(float toPosition, int toCompletedLoops, UpdateMode updateMode)
        {
            // Startup
            if (!startupDone) {
                if (!Startup()) return true;
            }
            // OnStart and first OnPlay callbacks
            if (!playedOnce && updateMode == UpdateMode.Update) {
                playedOnce = true;
                if (onStart != null) {
                    OnTweenCallback(onStart, this);
                    if (!active) return true; // Tween might have been killed by onStart callback
                }
            }

            float prevPosition = position;
            int prevCompletedLoops = completedLoops;
            completedLoops = toCompletedLoops;
            bool wasRewinded = position <= 0 && prevCompletedLoops <= 0;
            bool wasComplete = isComplete;
            // Determine if it will be complete after update
            if (loops != -1) isComplete = completedLoops == loops;
            // Calculate newCompletedSteps (always useful with Sequences)
            int newCompletedSteps = 0;
            if (updateMode == UpdateMode.Update) {
                if (isBackwards) {
                    newCompletedSteps = completedLoops < prevCompletedLoops ? prevCompletedLoops - completedLoops : (toPosition <= 0 && !wasRewinded ? 1 : 0);
                    if (wasComplete) newCompletedSteps--;
                } else newCompletedSteps = completedLoops > prevCompletedLoops ? completedLoops - prevCompletedLoops : 0;
            } else if (this is Sequence) {
                newCompletedSteps = prevCompletedLoops - toCompletedLoops;
                if (newCompletedSteps < 0) newCompletedSteps = -newCompletedSteps;
            }

            // Set position (makes position 0 equal to position "end" when looping)
            position = toPosition;
            if (position > duration) position = duration;
            else if (position <= 0) {
                if (completedLoops > 0 || isComplete) position = duration;
                else position = 0;
            }
            // Set playing state after update
            if (isPlaying) {
                if (!isBackwards) isPlaying = !isComplete; // Reached the end
                else isPlaying = !(completedLoops == 0 && position <= 0); // Rewinded
            }

            // updatePosition is different in case of Yoyo loop under certain circumstances
            var useInversePosition = hasLoops && loopType == LoopType.Yoyo
                                              && (position < duration ? completedLoops % 2 != 0 : completedLoops % 2 == 0);

            // Get values from plugin and set them
            if (ApplyTween(prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition, updateMode)) return true;

            // Additional callbacks
            if (onUpdate != null) {
                OnTweenCallback(onUpdate, this);
            }
            if (isComplete && !wasComplete && onComplete != null) {
                OnTweenCallback(onComplete, this);
            }

            // Return
            return autoKill && isComplete;
        }

        // deltaTime will be passed as fixedDeltaTime in case of UpdateType.Fixed
        // Returns TRUE if the tween should be killed
        internal bool ForceUpdate(float dt)
        {
            Assert.IsTrue(active, "Given tween is not valid");
            if (!isPlaying) return false;
            creationLocked = true; // Lock tween creation methods from now on

            // Skip update in case time is approximately 0
            const float epsilon = 0.000001f;
            if (dt is < epsilon and > -epsilon)
                return false;

            // Update delay.
            if (!delayComplete)
            {
                var elapsed = elapsedDelay + dt;
                if (elapsed <= delay)
                {
                    elapsedDelay = elapsed;
                    return false;
                }

                // Delay complete
                elapsedDelay = delay;
                delayComplete = true;
                dt = elapsed - delay;
            }

            // Startup (needs to be here other than in Tween.DoGoto in case of speed-based tweens, to calculate duration correctly)
            if (!startupDone) {
                if (!Startup()) {
                    // Startup failure: mark for killing
                    return true;
                }
            }

            // Find update data
            float toPosition = position;
            bool wasEndPosition = toPosition >= duration;
            int toCompletedLoops = completedLoops;
            if (duration <= 0) {
                toPosition = 0;
                toCompletedLoops = loops == -1 ? completedLoops + 1 : loops;
            } else {
                if (isBackwards) {
                    toPosition -= dt;
                    while (toPosition < 0 && toCompletedLoops > -1) {
                        toPosition += duration;
                        toCompletedLoops--;
                    }
                    if (toCompletedLoops < 0 || wasEndPosition && toCompletedLoops < 1) {
                        // Result is equivalent to a rewind, so set values according to it
                        toPosition = 0;
                        toCompletedLoops = wasEndPosition ? 1 : 0;
                    }
                } else {
                    toPosition += dt;
                    while (toPosition >= duration && (loops == -1 || toCompletedLoops < loops)) {
                        toPosition -= duration;
                        toCompletedLoops++;
                    }
                }
                if (wasEndPosition) toCompletedLoops--;
                if (loops != -1 && toCompletedLoops >= loops) toPosition = duration;
            }

            // Goto
            return ForceGoto(toPosition, toCompletedLoops, UpdateMode.Update);
        }

        // Assumes that the callback exists (because it was previously checked).
        // Returns TRUE in case of success, FALSE in case of error (if safeMode is on)
        internal static bool OnTweenCallback(TweenCallback callback, Tween t)
        {
            if (Config.useSafeMode) {
                try {
                    callback();
                } catch (Exception e) {
                    Debugger.LogSafeModeCapturedError(e, t);
                    return false; // Callback error
                }
            } else callback();
            return true;
        }

        #endregion
    }
}