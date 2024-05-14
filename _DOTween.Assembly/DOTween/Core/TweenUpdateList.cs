using System.Collections.Generic;
using DG.Tweening.Core.Enums;
using UnityEngine.Assertions;

namespace DG.Tweening.Core
{
    public struct TweenUpdateList
    {
        readonly List<Tween> _list;
        readonly List<int> _reservedToRemove;
        int _iterateDepth;

        public TweenUpdateList(int capacity)
        {
            _list = new List<Tween>(capacity);
            _reservedToRemove = new List<int>();
            _iterateDepth = 0;
        }

        public List<Tween> StartIterate(out TweenUpdateId count)
        {
            if (_iterateDepth is not 0)
                L.W("[DOTween] Iteration started while already iterating: " + _iterateDepth);
            _iterateDepth++;
            count = (TweenUpdateId) _list.Count;
            return _list;
        }

        public void EndIterate()
        {
            _iterateDepth--;
            Assert.IsTrue(_iterateDepth >= 0, "Iterate depth is below 0");

            if (_iterateDepth is not 0)
                return;

            // Skip if there are no tweens to remove.
            var removeCount = _reservedToRemove.Count;
            if (removeCount is 0)
                return;

            // Clear list if all tweens are removed.
            var listCount = _list.Count;
            if (listCount == removeCount)
            {
#if DEBUG
                foreach (var tween in _list)
                    Assert.IsTrue(tween.updateId.IsInvalid(), "updateId is valid");
#endif
                _list.Clear();
                _reservedToRemove.Clear();
                return;
            }

            // Reorder list.
            var lastIndex = listCount;
            foreach (var removeIndex in _reservedToRemove)
            {
                --lastIndex;
                Assert.IsTrue(lastIndex >= 0, "Index is below 0");
                var lastTween = SearchEligibleTweenBackward(_list, ref lastIndex);
                if (removeIndex > lastIndex) continue; // Skip if the tween is already removed.

                Assert.AreNotEqual(removeIndex, lastIndex, "removeIndex is equal to lastIndex");
                Assert.IsTrue(lastTween.updateId.IsValid(), "updateId is invalid");

                lastTween.updateId = (TweenUpdateId) removeIndex;
                _list[removeIndex] = lastTween;
            }

            // Validate list.
            var newCount = listCount - removeCount;
#if DEBUG
            for (var i = 0; i < newCount; i++)
            {
                var tween = _list[i];
                Assert.AreEqual((TweenUpdateId) i, tween.updateId, "updateId is not equal to index");
            }
#endif

            // Shrink list.
            _list.RemoveRange(newCount, removeCount);
            Assert.AreEqual(newCount, _list.Count, "List count is not equal to new count");
            _reservedToRemove.Clear();
            return;

            static Tween SearchEligibleTweenBackward(List<Tween> list, ref int index)
            {
                while (true)
                {
                    var tween = list[index];
                    if (tween.updateId.IsValid())
                        return tween;
                    index--;
                    Assert.AreNotEqual(0, index, "Can't find an eligible tween");
                }
            }
        }

        public void Add(Tween tween)
        {
            Assert.IsTrue(tween.active, "You can't add a tween that is not active");
            Assert.IsTrue(tween.updateId.IsInvalid(), "updateId is valid");
            tween.updateId = (TweenUpdateId) _list.Count;
            _list.Add(tween);
        }

        public void Remove(Tween tween)
        {
            Assert.IsTrue(tween.active, "You can't remove a tween that is not active");
            Assert.IsTrue(tween.updateId.IsValid(), "updateId is invalid");
            Assert.IsFalse(_reservedToRemove.Contains((int) tween.updateId), "Tween is already reserved to be removed");
            // L.I($"[DOTween] Will be removed: {tween}", tween);
            _reservedToRemove.Add((int) tween.updateId);
            tween.updateId = TweenUpdateId.Invalid;
        }
    }
}