#nullable enable
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public class DOTweenGroup : MonoBehaviour
#if UNITY_EDITOR
        , ISelfValidator
#endif
    {
        private List<Tweener>? _tweens;

        private static readonly List<DOTweenAnim> _animBuf = new();
        private static readonly Stack<List<Tweener>> _tweenPool = new();

        private void OnEnable()
        {
            if (_tweens is null)
            {
                _tweens = _tweenPool.Count > 0 ? _tweenPool.Pop() : new List<Tweener>();
            }
            else
            {
                Assert.AreEqual(0, _tweens!.Count);
            }

            var id = GetInstanceID();
            GetComponents(_animBuf);
            foreach (var anim in _animBuf)
            {
                var tween = anim.CreateTween(play: true);
                tween.id = id;
                _tweens!.Add(tween);
            }
        }

        private void OnDisable()
        {
            var id = GetInstanceID();

            foreach (var tween in _tweens!)
            {
                // XXX: Even if AutoKill is set to false, tween can be killed accidentally, like transform.DOKill().
                if (tween.id == id)
                    tween.KillRewind();
            }

            _tweens.Clear();
        }

        private void OnDestroy()
        {
            if (_tweens is not null)
            {
                Assert.AreEqual(0, _tweens.Count, "Should be cleared from OnDisable()");
                _tweenPool.Push(_tweens);
                _tweens = null;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Toggle Preview _p"), Button(DirtyOnClick = false)]
        private void TogglePreview()
        {
            GetComponents(_animBuf);

            // kill in reverse order to restore the original value correctly.
            var anyStopped = false;
            for (var i = _animBuf.Count - 1; i >= 0; i--)
            {
                var previewId = _animBuf[i].GetInstanceID();
                anyStopped |= DOTweenPreviewManager.TryStopPreview(previewId);
            }
            if (anyStopped) return; // if any stopped, then we were in preview mode.

            foreach (var anim in _animBuf)
            {
                var previewId = anim.GetInstanceID();
                DOTweenPreviewManager.StartPreview(
                    anim.CreateTween(play: false).SetId(previewId));
            }
        }

        void ISelfValidator.Validate(SelfValidationResult result)
        {
            GetComponents(_animBuf);

            if (_animBuf.Count is 0)
            {
                result.AddError("적어도 하나의 트윈 애니메이션이 존재해야합니다.");
                return;
            }

            foreach (var anim in _animBuf)
            {
                if (anim.autoGenerate)
                    result.AddError("매니저가 있는 경우 autoGenerate 가 비활성화되어있어야합니다.").WithFix(() => anim.autoGenerate = false);
                if (anim.autoPlay)
                    result.AddError("매니저가 있는 경우 autoPlay 가 비활성화되어있어야합니다.").WithFix(() => anim.autoPlay = false);
                if (anim.autoKill)
                    result.AddError("매니저가 있는 경우 autoKill 이 비활성화되어있어야합니다.").WithFix(() => anim.autoKill = false);
            }
        }
#endif
    }
}