#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public readonly struct TweenTracker
    {
        private readonly List<(Tweener, TweenId)>? _tweens;

        private TweenTracker(List<(Tweener, TweenId)>? tweens) =>
            _tweens = tweens;

        private TweenTracker Add(Tweener tweener)
        {
            Assert.IsTrue(tweener.id.IsValid(), "Tweener must have a valid ID before adding to TweenTracker.");
            var list = _tweens ?? new List<(Tweener, TweenId)>();
            list.Add((tweener, tweener.id));
            return new TweenTracker(list);
        }

        private void KillRange(int startIndex, int count, bool rewind)
        {
            Assert.IsNotNull(_tweens, "[TweenTracker] _tweens is null.");
            Assert.IsTrue(startIndex >= 0, "[TweenTracker] KillRange: count is larger than the number of tweens in the tracker.");
            Assert.IsTrue(startIndex + count <= _tweens!.Count, "[TweenTracker] KillRange: count is larger than the number of tweens in the tracker.");

            // as list could be appended while we are iterating,
            // we need to kill all tweens in the range first,
            // then remove them from the list.

            var killed = 0; // for logging.
            for (var i = startIndex; i < startIndex + count; i++)
            {
                Assert.IsNotNull(_tweens, "[TweenTracker] _tweens is null.");
                var (tween, id) = _tweens![i];
                if (tween.id != id) continue; // tween has been recycled or killed.
                KillTween(tween, rewind);
                killed++;
            }

            // remove killed tweens from the list.
            _tweens!.RemoveRange(startIndex, count);

            if (killed is not 0)
                L.I($"[TweenTracker] Killed {killed} tweens from the tracker.");
        }

        public void KillAll(bool rewind = false)
        {
            if (_tweens is not null)
                KillRange(0, _tweens.Count, rewind);
        }

        public void KillAll(Object target, bool rewind)
        {
            if (_tweens is null) return;

            var count = _tweens.Count;
            var swapPtr = count - 1;
            var tweensToKill = 0;

            // send tweens to kill to the end of the list first.
            // tween could be added to the tracker while we are iterating.
            for (var i = count - 1; i >= 0; i--)
            {
                var (tweener, id) = _tweens[i];
                if (tweener.id != id // prune recycled or killed tweens.
                    || tweener.target.RefEq(target)) // target matches, we will kill this tween.
                {
                    tweensToKill++; // we will kill this tween.

                    if (i == swapPtr) continue; // no need to swap.

                    // swap with the last element.
                    _tweens[i] = _tweens[swapPtr];
                    _tweens[swapPtr] = (tweener, id);
                    swapPtr--;
                }
            }

            if (tweensToKill is not 0)
                KillRange(swapPtr, tweensToKill, rewind);
        }

        private static void KillTween(Tweener tweener, bool rewind)
        {
            Assert.IsTrue(tweener.active && tweener.id.IsValid(),
                "TweenTracker.KillTween: Tweener must be active and have a valid ID before killing.");
            if (rewind) tweener.KillRewind();
            else tweener.Kill();
            Assert.IsTrue(tweener.id.IsInvalid(),
                "TweenTracker.KillTween: Tweener should have an invalid ID after being killed.");
        }

        [MustUseReturnValue]
        public static TweenTracker operator +(TweenTracker tracker, Tweener tweener) => tracker.Add(tweener);
    }
}