// Decompiled with JetBrains decompiler
// Type: DG.Tweening.DOTweenVisualManager
// Assembly: DOTweenPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3BD1A734-E28E-44F1-8B34-16C95521FE16
// Assembly location: /Users/jameskim/Develop/meow-tower/Assets/Plugins/Demigiant/DOTweenPro/DOTweenPro.dll
// XML documentation location: /Users/jameskim/Develop/meow-tower/Assets/Plugins/Demigiant/DOTweenPro/DOTweenPro.xml

using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public class DOTweenVisualManager_Custom : MonoBehaviour
#if UNITY_EDITOR
        , ISelfValidator
#endif
    {
        static readonly Stack<List<DOTweenAnimation>> _animListPool = new();

        [CanBeNull] List<DOTweenAnimation> _animList;

        void OnEnable()
        {
            if (_animList == null)
            {
                _animList = _animListPool.Count > 0 ? _animListPool.Pop() : new List<DOTweenAnimation>();
            }
            else
            {
                Assert.AreEqual(0, _animList!.Count);
            }

            var id = GetInstanceID();
            GetComponents(_animList);
            foreach (var anim in _animList!)
            {
                anim.CreateTween(false, true);
                var tween = anim.tween;
                if (tween is null) continue;
                anim.tween.id = id;
            }
        }

        void OnDisable()
        {
            var id = GetInstanceID();

            foreach (var anim in _animList!)
            {
                var tween = anim.tween;
                // XXX: Even if AutoKill is set to false, tween can be killed accidentally, like transform.DOKill().
                if (tween is null || tween.id != id)
                    continue;
                tween.KillRewind();
            }

            _animList.Clear();
        }

        void OnDestroy()
        {
            if (_animList != null)
            {
                Assert.AreEqual(0, _animList.Count);
                _animListPool.Push(_animList);
                _animList = null;
            }
        }

#if UNITY_EDITOR
        void ISelfValidator.Validate(SelfValidationResult result)
        {
            var anims = GetComponents<DOTweenAnimation>();

            if (anims.Length == 0)
            {
                result.AddError("적어도 하나의 트윈 애니메이션이 존재해야합니다.");
                return;
            }

            foreach (var anim in anims)
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