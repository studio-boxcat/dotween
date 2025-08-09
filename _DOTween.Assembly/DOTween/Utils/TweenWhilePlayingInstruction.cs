#nullable enable

using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    internal sealed class TweenWhilePlayingInstruction : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                if (_tween.active is false) return false; // typical case: tween goes back to pool
                if (_tween.id != _orgId) return false; // tween has been recycled (or forcefully changed its id...)
                if (_tween.isComplete) return false; // tween is completed
                if (_tween.isPlaying is false) return false; // tween is paused or killed or so.
                return true;
            }
        }

        private readonly Tweener _tween;
        private readonly TweenId _orgId;

        internal TweenWhilePlayingInstruction(Tweener tween)
        {
            _tween = tween;
            _orgId = tween.id;

            // those assertions will be checked by Utils.WhilePlaying(), but just in case.
            Assert.IsTrue(_tween.active, "TweenWhilePlayingInstruction can only be used with active tweens.");
            Assert.IsTrue(_orgId.IsValid(), "TweenWhilePlayingInstruction can only be used with valid tweens.");
        }
    }
}