using System.Collections.Generic;
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

        public TweenEnumerable StartIterate()
        {
            if (_iterateDepth is not 0)
                L.W("[DOTween] Iteration started while already iterating: " + _iterateDepth);
            _iterateDepth++;
            return new TweenEnumerable(_list, (TweenUpdateId) _list.Count);
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

#if DEBUG
            // Validate integrity.
            foreach (var i in _reservedToRemove)
                Assert.IsTrue(_list[i].updateId.IsInvalid(), "updateId is valid");
            foreach (var tween in _list)
            {
                if (tween.updateId.IsValid()) continue;
                Assert.IsFalse(_reservedToRemove.Contains((int) tween.updateId),
                    "Tween is not reserved to be removed");
            }
#endif

            // L.I($"[DOTween] Will remove tweens: {string.Join(", ", _reservedToRemove)}");
            // L.I($"[DOTween] Update list: {string.Join(", ", _list.Select(x => (int) x.updateId))}");

            // Reorder list.
            Tween lastTween = null;
            var lastIndex = listCount;
            foreach (var removeIndex in _reservedToRemove)
            {
                if (lastTween is null)
                {
                    --lastIndex;
                    Assert.IsTrue(lastIndex >= 0, "Index is below 0");
                    lastTween = SearchEligibleTweenBackward(_list, ref lastIndex);
                    Assert.AreEqual(lastTween, _list[lastIndex], "Tween is not equal to last tween");
                    Assert.AreNotEqual(removeIndex, lastIndex, "removeIndex is equal to lastIndex");
                }

                if (removeIndex > lastIndex) continue; // Skip if the tween is already removed.
                Assert.IsNotNull(lastTween, "lastTween is null");
                Assert.IsTrue(lastTween.updateId.IsValid(), "updateId is invalid");
                Assert.IsTrue(_list[removeIndex].updateId.IsInvalid(), "updateId is valid");

                // L.I("[DOTween] Swap: " + lastTween.updateId + " <-> " + removeIndex);
                lastTween.updateId = (TweenUpdateId) removeIndex;
                _list[removeIndex] = lastTween;
                lastTween = null;
                // L.I($"[DOTween] Update list: {string.Join(", ", _list.Select(x => (int) x.updateId))}");
            }

            // L.I($"[DOTween] Update list: {string.Join(", ", _list.Select(x => (int) x.updateId))}");

            // Validate list.
            var newCount = listCount - removeCount;
#if DEBUG
            for (var i = 0; i < newCount; i++)
            {
                var tween = _list[i];
                Assert.AreEqual((TweenUpdateId) i, tween.updateId,
                    $"updateId is not equal to index: expected={i}, actual={tween.updateId}, listCount={listCount}, newCount={newCount}");
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
                    Assert.IsTrue(index >= 0, "Can't find an eligible tween");
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

        public struct TweenEnumerable
        {
            readonly List<Tween> _list;
            readonly TweenUpdateId _lastUpdateId;

            public TweenEnumerable(List<Tween> list, TweenUpdateId lastUpdateId)
            {
                _list = list;
                _lastUpdateId = lastUpdateId;
            }

            public TweenEnumerator GetEnumerator() => new TweenEnumerator(_list, _lastUpdateId);

            public struct TweenEnumerator
            {
                readonly List<Tween> _list;
                readonly TweenUpdateId _lastUpdateId;
                int _index;

                public TweenEnumerator(List<Tween> list, TweenUpdateId lastUpdateId)
                {
                    _list = list;
                    _lastUpdateId = lastUpdateId;
                    _index = -1;
                }

                public Tween Current => _list[_index];

                public bool MoveNext()
                {
                    while (++_index < _list.Count)
                    {
                        var t = _list[_index];
                        if (t.updateId.IsInvalid()) continue;
                        if (t.updateId > _lastUpdateId) break;
                        Assert.IsTrue(t.active, "Tween is not active");
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}