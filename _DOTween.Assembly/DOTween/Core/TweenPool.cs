using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening.Core
{
    internal static class TweenPool
    {
        private const int _recycleThreshold = 4;

        private static readonly List<Tweener> _float = new();
        private static readonly List<Tweener> _int = new();
        private static readonly List<Tweener> _color = new();
        private static readonly List<Tweener> _vector2 = new();
        private static readonly List<Tweener> _vector3 = new();
        private static readonly List<Sequence> _sequence = new();

        private static readonly List<Tweener> _recyclableTweens = new();
        private static readonly List<Sequence> _recyclableSequences = new();

#if DEBUG
        private static readonly Dictionary<Type, int> _debugCreateCount = new();
#endif

        public static TweenerCore<T> RentTweener<T>()
        {
            var list = GetTweenerList(typeof(TweenerCore<T>));

            Tweener tweener = null;

            var count = list.Count;
            if (count is 0)
            {
                RecordCreate(typeof(T));
                tweener = new TweenerCore<T>();
            }
            else
            {
                tweener = list[count - 1];
                list.RemoveAt(count - 1);
            }

            Assert.IsFalse(tweener.active, "Polled tweener is still active");
            Assert.IsTrue(tweener.updateId.IsInvalid(), "Polled tweener has a valid updateId");
            tweener.active = true;
            return (TweenerCore<T>) tweener;
        }

        public static void ReturnTweener(Tweener tweener)
        {
            Assert.IsTrue(tweener.active, "Returned tweener is not active");
            Assert.IsTrue(tweener.updateId.IsInvalid(), "Returned tweener has a valid updateId");
            tweener.active = false;
            tweener.Reset();
            _recyclableTweens.Add(tweener);
        }

        public static Sequence RentSequence()
        {
            Sequence sequence = null;

            var count = _sequence.Count;
            if (count is 0)
            {
                RecordCreate(typeof(Sequence));
                sequence = new Sequence();
            }
            else
            {
                sequence = _sequence[count - 1];
                _sequence.RemoveAt(count - 1);
            }

            Assert.IsFalse(sequence.active, "Polled tweener is still active");
            Assert.IsTrue(sequence.updateId.IsInvalid(), "Polled tweener has a valid updateId");
            sequence.active = true;
            return sequence;
        }

        public static void ReturnSequence(Sequence sequence)
        {
            Assert.IsTrue(sequence.active, "Returned tweener is not active");
            sequence.active = false;
            sequence.Reset();
            _recyclableSequences.Add(sequence);
        }

        public static void Recycle()
        {
#if DEBUG
            foreach (var tweener in _recyclableTweens)
            {
                Assert.IsFalse(tweener.active, "Tween to recycle is still active");
                Assert.IsTrue(tweener.updateId.IsInvalid(), "Tween to recycle has a valid updateId");
            }

            foreach (var sequence in _recyclableSequences)
            {
                Assert.IsFalse(sequence.active, "Sequence to recycle is still active");
                Assert.IsTrue(sequence.updateId.IsInvalid(), "Tween to recycle has a valid updateId");
            }
#endif

            if (_recyclableTweens.Count >= _recycleThreshold)
            {
                // L.I("[DOTween] Recycling tweens: " + _recyclableTweens.Count);
                foreach (var tweener in _recyclableTweens)
                    GetTweenerList(tweener.GetType()).Add(tweener);
                _recyclableTweens.Clear();
            }

            if (_recyclableSequences.Count >= _recycleThreshold)
            {
                // L.I("[DOTween] Recycling sequences: " + _recyclableSequences.Count);
                _sequence.AddRange(_recyclableSequences);
                _recyclableSequences.Clear();
            }
        }

        private static List<Tweener> GetTweenerList(Type tweenerType)
        {
            if (tweenerType == typeof(TweenerCore<float>)) return _float;
            if (tweenerType == typeof(TweenerCore<int>)) return _int;
            if (tweenerType == typeof(TweenerCore<Color>)) return _color;
            if (tweenerType == typeof(TweenerCore<Vector2>)) return _vector2;
            if (tweenerType == typeof(TweenerCore<Vector3>)) return _vector3;
            throw new ArgumentException($"Unsupported tweener type: {tweenerType}");
        }

        [Conditional("DEBUG")]
        private static void RecordCreate(Type type)
        {
#if DEBUG
            var count = _debugCreateCount.GetValueOrDefault(type, 0);
            _debugCreateCount[type] = ++count;

            if (count > 32)
                L.W($"[DOTween] {type.Name} created {count} instances. This is probably a leak.");
#endif
        }

#if DEBUG
        public static int SumPooledTweeners()
        {
            return _float.Count + _int.Count + _color.Count + _vector2.Count + _vector3.Count;
        }

        public static int SumPooledSequences()
        {
            return _sequence.Count;
        }

        public static void Editor_Clear()
        {
            _float.Clear();
            _int.Clear();
            _color.Clear();
            _vector2.Clear();
            _vector3.Clear();
            _sequence.Clear();
            _recyclableTweens.Clear();
            _recyclableSequences.Clear();
            _debugCreateCount.Clear();
        }
#endif
    }
}