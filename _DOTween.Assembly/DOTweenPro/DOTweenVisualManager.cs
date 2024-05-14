using System.Collections.Generic;
using DG.Tweening.Core;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public class DOTweenVisualManager : MonoBehaviour
#if UNITY_EDITOR
        , ISelfValidator
#endif
    {
        static readonly List<DOTweenAnimation> _animBuf = new();
        static readonly Stack<List<Tweener>> _tweenListPool = new();
        [CanBeNull] List<Tweener> _tweenList;

        void OnEnable()
        {
            if (_tweenList is null)
            {
                _tweenList = _tweenListPool.Count > 0 ? _tweenListPool.Pop() : new List<Tweener>();
            }
            else
            {
                Assert.AreEqual(0, _tweenList!.Count);
            }

            var id = GetInstanceID();
            GetComponents(_animBuf);
            foreach (var anim in _animBuf)
            {
                var tween = anim.CreateTweenInstance();
                tween.id = id;
                _tweenList!.Add(tween);
            }
        }

        void OnDisable()
        {
            var id = GetInstanceID();

            foreach (var tween in _tweenList!)
            {
                // XXX: Even if AutoKill is set to false, tween can be killed accidentally, like transform.DOKill().
                if (tween is null || tween.id != id)
                    continue;
                tween.KillRewind();
            }

            _tweenList.Clear();
        }

        void OnDestroy()
        {
            if (_tweenList is not null)
            {
                Assert.AreEqual(0, _tweenList.Count);
                _tweenListPool.Push(_tweenList);
                _tweenList = null;
            }
        }

#if UNITY_EDITOR
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
                {
                    result.AddError("매니저가 있는 경우 autoGenerate 가 비활성화되어있어야합니다.");
                    return;
                }

                if (anim.autoPlay)
                {
                    result.AddError("매니저가 있는 경우 autoPlay 가 비활성화되어있어야합니다.");
                    return;
                }

                if (anim.autoKill)
                {
                    result.AddError("매니저가 있는 경우 autoKill 이 비활성화되어있어야합니다.");
                    return;
                }
            }
        }
#endif
    }
}