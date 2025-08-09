#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public readonly struct TweenTracker
    {
        private readonly List<(Tweener, TweenId)>? _tweens;

        private TweenTracker(List<(Tweener, TweenId)>? tweens) =>
            _tweens = tweens;

        public void KillAll()
        {
            if (_tweens == null) return;

            var count = _tweens.Count; // OnKill callback could be invoked.
            var killed = 0;
            for (var i = 0; i < count; i++)
            {
                var (tween, id) = _tweens[i];
                if (tween.id != id) continue; // tween has been modified. (mostly by auto kill)
                tween.Kill();
                killed++;
            }

            if (count == _tweens.Count)
            {
                _tweens.Clear();
            }
            else
            {
                L.W("[DOTween] TweenTracker.KillAll(): some tweens have been added while killing.");
                _tweens.RemoveRange(0, count); // rare case.
            }

            if (killed is not 0)
                L.I("[DOTween] TweenTracker.KillAll(): killed=" + killed.Strm());
        }

        [MustUseReturnValue]
        public static TweenTracker operator +(TweenTracker tracker, Tweener tweener)
        {
            Assert.IsTrue(tweener.id.IsValid(), "Tweener must have a valid ID before adding to TweenTracker.");
            var list = tracker._tweens ?? new List<(Tweener, TweenId)>();
            list.Add((tweener, tweener.id));
            return new TweenTracker(list);
        }
    }
}