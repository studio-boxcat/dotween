// ReSharper disable InconsistentNaming

#nullable enable
using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DG.Tweening
{
    public enum DOTweenAnimType : byte
    {
        None = 0,
        Move = 2,
        MoveY = 3,
        Rotate = 4,
        Scale = 5,
        Fade = 7,
        PunchPos = 9,
        PunchRot = 10,
        PunchScale = 11,
        ShakePos = 12,
        ShakeRot = 13,
        ShakeScale = 14,
        AnchorPos = 21,
        Anchor = 22,
    }

    /// <summary>
    /// Attach this to a GameObject to create a tween
    /// </summary>
    public sealed partial class DOTweenAnim : MonoBehaviour
    {
        [NonSerialized]
        public Tweener? tween;

        [Required, ChildGameObjectsOnly]
        public Component target = null!;

        public DOTweenAnimType animType;

        [MinValue(0.0001f)]
        public float duration = 1;
        public float delay;

        public bool isFrom;
        public bool isRelative = true;
        public Ease easeType = Ease.OutQuad;
        public AnimationCurve easeCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public int loops = 1;
        public LoopType loopType = LoopType.Restart;

        public bool autoGenerate = true; // If TRUE automatically creates the tween at startup
        public bool autoPlay = true;
        public bool autoKill = true;

        public Vector3 endValue;
        public float optionalFloat; // elasticity (Punch), randomness (Shake)
        public int optionalInt; // vibrato (Punch, Shake)

#if UNITY_EDITOR // uniform scale is editor only property.
        public bool uniformScale;
#endif

        private void OnEnable()
        {
            if (autoGenerate)
                PopulateTween(play: autoPlay);
        }

        private void OnDisable()
        {
            if (tween != null)
            {
                if (tween.active)
                    tween.KillRewind();
                tween = null;
            }
        }

        // Used also by DOTweenAnimInspector when applying runtime changes and restarting
        /// <summary>
        /// Creates the tween manually (called automatically if AutoGenerate is set in the Inspector)
        /// from its target's current value.
        /// </summary>
        /// <param name="play">If TRUE also plays the tween, otherwise only creates it</param>
        public Tweener PopulateTween(bool play)
        {
            Assert.AreNotEqual(DOTweenAnimType.None, animType, "AnimationType is None");
            Assert.IsNotNull(target, "Target is null");

            if (tween is not { active: true })
            {
                tween = CreateTween(play: play);
                tween.OnKill(() => tween = null); // automatically nullify tween when it is killed
            }

            return tween;
        }

        [MustUseReturnValue]
        internal Tweener CreateTween(bool play)
        {
            L.I($"[DOTweenAnim] CreateTween: {animType} - {target}", this);

            // Create tween.
            var t = CreateTween(
                target, transform, animType, duration,
                endValue, optionalFloat, optionalInt);

            // Set from or relative.
            if (isFrom) t.From(isRelative);
            else t.SetRelative(isRelative);

            // Set basic tween settings.
            t.SetTarget(null) // set target to null to prevent accidental tween kills by DOKill() (e.g. transform.DOKill())
                .SetDelay(delay).SetLoops(loops, loopType).SetAutoKill(autoKill);

            // Set easeType.
            if (easeType is Ease.INTERNAL_Custom) t.SetEase(easeCurve);
            else t.SetEase(easeType);

            if (play is false) t.Pause();
            return t;
        }

        private static Tweener CreateTween(
            Object target, Transform transform,
            DOTweenAnimType animType,
            float duration,
            Vector3 endValue,
            float optionalFloat,
            int optionalInt)
        {
            Assert.IsTrue(animType != DOTweenAnimType.None, "Animation type cannot be None");
            Assert.IsTrue(duration > 0, "Duration must be greater than 0");

            return animType switch
            {
                DOTweenAnimType.Move => transform.DOLocalMove(endValue, duration),
                DOTweenAnimType.MoveY => transform.DOLocalMoveY(endValue.y, duration),
                DOTweenAnimType.Rotate => transform.DOLocalRotateZ(endValue.z, duration),
                DOTweenAnimType.Scale => transform.DOScale(endValue, duration),
                DOTweenAnimType.Fade => target switch
                {
                    CanvasGroup t => t.DOFade(endValue.x, duration),
                    Graphic t => t.DOFade(endValue.x, duration),
                    SpriteRenderer t => t.DOFade(endValue.x, duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.PunchPos => target switch
                {
                    RectTransform t => t.DOPunchAnchorPos(endValue, duration, vibrato: optionalInt, elasticity: optionalFloat),
                    Transform t => t.DOPunchPosition(endValue, duration, vibrato: optionalInt, elasticity: optionalFloat),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.PunchScale => transform.DOPunchScale(endValue, duration, vibrato: optionalInt, elasticity: optionalFloat),
                DOTweenAnimType.PunchRot => transform.DOPunchRotation(endValue, duration, vibrato: optionalInt, elasticity: optionalFloat),
                DOTweenAnimType.ShakePos => target switch
                {
                    RectTransform t => t.DOShakeAnchorPos(duration, endValue, vibrato: optionalInt, randomness: optionalFloat, fadeOut: false),
                    Transform t => t.DOShakePosition(duration, endValue, vibrato: optionalInt, randomness: optionalFloat, fadeOut: false),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.ShakeScale => transform.DOShakeScale(duration, endValue, vibrato: optionalInt, randomness: optionalFloat, fadeOut: false),
                DOTweenAnimType.ShakeRot => transform.DOShakeRotation(duration, endValue, vibrato: optionalInt, randomness: optionalFloat, fadeOut: false),
                DOTweenAnimType.AnchorPos => target switch
                {
                    RectTransform t => t.DOAnchorPos(endValue, duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.Anchor => DOTween.To(
                    () => ((RectTransform) target).anchorMin,
                    x => ((RectTransform) target).anchorMin = ((RectTransform) target).anchorMax = x,
                    (Vector2) endValue, duration),
                _ => throw new ArgumentOutOfRangeException(nameof(animType), animType, null)
            };
        }
    }
}