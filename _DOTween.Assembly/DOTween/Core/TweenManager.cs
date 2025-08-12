// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 13:00
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#nullable enable
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    internal static class TweenManager
    {
        internal static TweenUpdateList Tweens = new(32);

        #region Main

        // Returns a new Tweener, from the pool if there's one available,
        // otherwise by instantiating a new one
        internal static Tweener<T> GetTweener<T>() where T : struct
        {
            var t = TweenPool.RentTweener<T>();
            AttachTween(t);
            return t;
        }

        // Returns a new Sequence, from the pool if there's one available,
        // otherwise by instantiating a new one
        internal static Sequence GetSequence()
        {
            var s = TweenPool.RentSequence();
            AttachTween(s);
            return s;
        }

        // Adds the given tween to the active tweens list (updateType is always Normal, but can be changed by SetUpdateType)
        private static void AttachTween(Tween t) => Tweens.Add(t);

        // Removes a tween from the active list, then reorganizes said list and decreases the given total.
        // Also removes any TweenLinks associated to this tween.
        internal static void DetachTween(Tween t) => Tweens.Remove(t);

        internal static void KillTween(Tween t)
        {
            Assert.IsTrue(t.active, "Tween is not active");

            if (t.onKill != null)
            {
                t.onKill.OnTweenCallback(t);
                t.onKill = null;
            }

            if (t is Tweener tweener)
            {
                if (tweener.updateId.IsValid()) DetachTween(t);
                // returned tween could be added to TweenUpdateList again,
                // but it's safe since we are tracking _lastUpdateId. (count of tweens at the beginning of the update)
                // the new tween will have a updateId greater than the _lastUpdateId.
                TweenPool.ReturnTweener(tweener);
                Assert.IsFalse(tweener.active, "Tween is still active after being killed");
                return;
            }

            {
                var s = (Sequence) t;
                var len = s.sequencedTweens.Count;
                for (var i = len - 1; i >= 0; --i)
                    KillTween(s.sequencedTweens[i]);
                if (s.updateId.IsValid()) DetachTween(t);
                TweenPool.ReturnSequence(s);
                Assert.IsFalse(s.active, "Tween is still active after being killed");
            }
        }

        internal static void Update(float deltaTime)
        {
            TweenPool.Recycle();

            var tweens = Tweens.StartIterate();
            foreach (var t in tweens)
            {
                // The tween is marked for killing when Kill() is called inside a tween's callback.
                if (t.ForceUpdate(deltaTime))
                    KillTween(t);
            }
            Tweens.EndIterate();
        }

        internal static bool IsTweening(Object target)
        {
            var tweens = Tweens.StartIterate();
            foreach (var t in tweens)
            {
                if (ReferenceEquals(target, t.target) is false)
                    continue;
                if (t is { isComplete: true, autoKill: true })
                    continue;

                Tweens.EndIterate(); // exit early.
                return true;
            }

            Tweens.EndIterate();
            return false;
        }

        internal static void KillAllTweensWithTarget(Object? target, bool complete)
        {
            if (target is null)
            {
                L.E("[TweenManager] KillAllTweensWithTarget called with a null target. This is not allowed.");
                return;
            }

            var tweens = Tweens.StartIterate();
            foreach (var t in tweens)
            {
                // skip if the target is not the same as the tween's target
                if (target.RefNq(t.target))
                    continue;

                if (complete)
                {
                    if (!t.startupDone) ForceInit(t); // necessary?
                    Complete(t, updateMode: UpdateMode.Goto);
                    if (t.active is false) continue; // already killed by Complete()
                }

                KillTween(t);
            }

            Tweens.EndIterate();
        }

        #endregion

        #region Play Operations

        // Forces the tween to startup and initialize all its data
        internal static void ForceInit(Tween t, bool isSequenced = false)
        {
            if (t.startupDone) return;

            if (!t.Startup() && !isSequenced)
                KillTween(t);
        }

        internal static void Complete(Tween t, UpdateMode updateMode = UpdateMode.Goto)
        {
            Assert.IsTrue(t.active, "Tween is not active");

            if (t.loops is -1)
            {
                L.W("[TweenManager] Complete called on a tween with infinite loops. This is not allowed.");
                return;
            }

            if (t.isComplete) return;

            t.ForceGoto(t.duration, t.loops, updateMode);
            t.isPlaying = false;
            // Despawn if needed (might have already been killed by the complete callback/operation)
            if (t.autoKill) KillTween(t);
        }

        internal static bool Flip(Tween t)
        {
            t.isBackwards = !t.isBackwards;
            return true;
        }

        // Returns TRUE if there was an error and the tween needs to be destroyed
        internal static bool Goto(Tween t, float to, bool andPlay = false, UpdateMode updateMode = UpdateMode.Goto)
        {
            t.isPlaying = andPlay;
            t.delayComplete = true;
            t.elapsedDelay = t.delay;
            int toCompletedLoops = t.duration <= 0 ? 1 : Mathf.FloorToInt(to / t.duration); // Still generates imprecision with some values (like 0.4)
            float toPosition = to % t.duration;
            if (t.loops != -1 && toCompletedLoops >= t.loops)
            {
                toCompletedLoops = t.loops;
                toPosition = t.duration;
            }
            else if (toPosition >= t.duration) toPosition = 0;
            // If andPlay is FALSE manage onPause from here because DoGoto won't detect it (since t.isPlaying was already set from here)
            return t.ForceGoto(toPosition, toCompletedLoops, updateMode);
        }

        // Returns TRUE if the given tween was not already paused
        internal static bool Pause(Tween t)
        {
            if (t.isPlaying)
            {
                t.isPlaying = false;
                return true;
            }
            return false;
        }

        // Returns TRUE if the given tween was not already playing and is not complete
        internal static bool Play(Tween t)
        {
            if (!t.isPlaying && (!t.isBackwards && !t.isComplete || t.isBackwards && (t.completedLoops > 0 || t.position > 0)))
            {
                t.isPlaying = true;
                return true;
            }
            return false;
        }

        internal static bool PlayBackwards(Tween t)
        {
            if (t.completedLoops == 0 && t.position <= 0)
            {
                t.isBackwards = true;
                t.isPlaying = false;
                return false;
            }
            if (!t.isBackwards)
            {
                t.isBackwards = true;
                Play(t);
                return true;
            }
            return Play(t);
        }

        internal static bool PlayForward(Tween t)
        {
            if (t.isComplete)
            {
                t.isBackwards = false;
                t.isPlaying = false;
                return false;
            }
            if (t.isBackwards)
            {
                t.isBackwards = false;
                Play(t);
                return true;
            }
            return Play(t);
        }

        internal static bool Restart(Tween t, bool includeDelay = true, float changeDelayTo = -1)
        {
            t.isBackwards = false;
            if (changeDelayTo >= 0 && t is Tweener) t.delay = changeDelayTo;
            Rewind(t, includeDelay);
            t.isPlaying = true;
            return true;
        }

        internal static bool Rewind(Tween t, bool includeDelay = true)
        {
            t.isPlaying = false;
            var rewinded = false;
            if (t.delay > 0)
            {
                if (includeDelay)
                {
                    rewinded = t.delay > 0 && t.elapsedDelay > 0;
                    t.elapsedDelay = 0;
                    t.delayComplete = false;
                }
                else
                {
                    rewinded = t.elapsedDelay < t.delay;
                    t.elapsedDelay = t.delay;
                    t.delayComplete = true;
                }
            }
            if (t.position > 0 || t.completedLoops > 0 || !t.startupDone)
            {
                rewinded = true;
                t.ForceGoto(0, 0, UpdateMode.Goto);
            }
            return rewinded;
        }

        internal static void RestoreToOriginal(Tweener t)
        {
            Assert.IsTrue(t.active);

            if (!t.startupDone)
            {
                Assert.IsFalse(t.playedOnce);
                Assert.AreEqual(0, t.position);
                if (t.isFrom) t.ApplyOriginal(); // When the tween is isFrom, the setter immediately sets the endValue to the original one.
                return;
            }

            t.elapsedDelay = 0;
            t.delayComplete = false;
            t.completedLoops = 0;
            t.isComplete = false;
            t.position = 0;
            t.ApplyOriginal();
        }

        internal static bool TogglePause(Tween t)
        {
            if (t.isPlaying) return Pause(t);
            return Play(t);
        }

        #endregion

#if UNITY_EDITOR
        [PlayModeGate]
        internal static void DetachAllTweens()
        {
            L.I("[DOTween] Detach All Tweens");
            var tweens = Tweens.StartIterate();
            foreach (var t in tweens) DetachTween(t); // no kill to avoid OnKill() callbacks.
            Tweens.EndIterate();
            TweenPool.Clear();
        }
#endif
    }
}