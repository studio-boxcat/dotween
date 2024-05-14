using System;
using System.Collections.Generic;
using UnityEngine;

namespace DG.Tweening.Core
{
    static class TweenPool
    {
        static readonly List<Tweener> _float = new();
        static readonly List<Tweener> _int = new();
        static readonly List<Tweener> _color = new();
        static readonly List<Tweener> _vector2 = new();
        static readonly List<Tweener> _vector3 = new();
        static readonly List<Sequence> _sequence = new();

        public static TweenerCore<T> RentTweener<T>()
        {
            var list = GetTweenerList(typeof(TweenerCore<T>));

            var count = list.Count;
            if (count is 0)
                return new TweenerCore<T>();

            var t = list[count - 1];
            list.RemoveAt(count - 1);
            return (TweenerCore<T>) t;
        }

        public static void ReturnTweener(Tweener tweener)
        {
            GetTweenerList(tweener.GetType()).Add(tweener);
        }

        public static Sequence RentSequence()
        {
            var count = _sequence.Count;
            if (count is 0)
                return new Sequence();

            var sequence = _sequence[count - 1];
            _sequence.RemoveAt(count - 1);
            return sequence;
        }

        public static void ReturnSequence(Sequence sequence)
        {
            _sequence.Add(sequence);
        }

        static List<Tweener> GetTweenerList(Type tweenerType)
        {
            if (tweenerType == typeof(TweenerCore<float>)) return _float;
            if (tweenerType == typeof(TweenerCore<int>)) return _int;
            if (tweenerType == typeof(TweenerCore<Color>)) return _color;
            if (tweenerType == typeof(TweenerCore<Vector2>)) return _vector2;
            if (tweenerType == typeof(TweenerCore<Vector3>)) return _vector3;
            throw new ArgumentException($"Unsupported tweener type: {tweenerType}");
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
#endif
    }
}