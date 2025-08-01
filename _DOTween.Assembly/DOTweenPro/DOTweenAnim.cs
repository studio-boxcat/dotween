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
        UIAnchors = 22,
    }

    /// <summary>
    /// Attach this to a GameObject to create a tween
    /// </summary>
    public sealed partial class DOTweenAnim : MonoBehaviour
    {
        [NonSerialized]
        public Tweener? tween;

        public float delay;
        [MinValue(0.0001f)]
        public float duration = 1;
        public Ease easeType = Ease.OutQuad;
        public AnimationCurve easeCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public LoopType loopType = LoopType.Restart;
        public int loops = 1;
        public bool isRelative = true;
        public bool isFrom;
        public bool autoKill = true;
        public bool autoGenerate = true; // If TRUE automatically creates the tween at startup

        [Required, ChildGameObjectsOnly]
        public Component target = null!;
        public DOTweenAnimType animationType;
        public bool autoPlay = true;

        public Vector3 endValueV3;
#if UNITY_EDITOR // uniform scale is editor only property.
        public bool optionalBool0;
#endif
        public float optionalFloat0;
        public int optionalInt0;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Editing.Yes(this)) return; // [ExecuteAlways] by DOTweenAnim.Editor.cs
#endif

            if (autoGenerate)
                PopulateTween(play: autoPlay);
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (Editing.Yes(this)) return; // [ExecuteAlways] by DOTweenAnim.Editor.cs
#endif

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
            Assert.AreNotEqual(DOTweenAnimType.None, animationType, "AnimationType is None");
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
            L.I($"[DOTweenAnim] CreateTween: {animationType} - {target}", this);

            // Create tween.
            var t = CreateTween(
                target, transform, animationType, duration,
                endValueV3, optionalFloat0, optionalInt0);

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
            Vector3 endValueV3,
            float optionalFloat0,
            int optionalInt0)
        {
            Assert.IsTrue(animType != DOTweenAnimType.None, "Animation type cannot be None");
            Assert.IsTrue(duration > 0, "Duration must be greater than 0");

            return animType switch
            {
                DOTweenAnimType.Move => transform.DOLocalMove(endValueV3, duration),
                DOTweenAnimType.MoveY => transform.DOLocalMoveY(endValueV3.y, duration),
                DOTweenAnimType.Rotate => transform.DOLocalRotateZ(endValueV3.z, duration),
                DOTweenAnimType.Scale => transform.DOScale(endValueV3, duration),
                DOTweenAnimType.Fade => target switch
                {
                    CanvasGroup t => t.DOFade(endValueV3.x, duration),
                    Graphic t => t.DOFade(endValueV3.x, duration),
                    SpriteRenderer t => t.DOFade(endValueV3.x, duration),
                    Renderer t => t.material.DOFade(endValueV3.x, duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.PunchPos => target switch
                {
                    RectTransform t => t.DOPunchAnchorPos(endValueV3, duration, vibrato: optionalInt0, elasticity: optionalFloat0),
                    Transform t => t.DOPunchPosition(endValueV3, duration, vibrato: optionalInt0, elasticity: optionalFloat0),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.PunchScale => transform.DOPunchScale(endValueV3, duration, vibrato: optionalInt0, elasticity: optionalFloat0),
                DOTweenAnimType.PunchRot => transform.DOPunchRotation(endValueV3, duration, vibrato: optionalInt0, elasticity: optionalFloat0),
                DOTweenAnimType.ShakePos => target switch
                {
                    RectTransform t => t.DOShakeAnchorPos(duration, endValueV3, vibrato: optionalInt0, randomness: optionalFloat0, fadeOut: false),
                    Transform t => t.DOShakePosition(duration, endValueV3, vibrato: optionalInt0, randomness: optionalFloat0, fadeOut: false),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                DOTweenAnimType.ShakeScale => transform.DOShakeScale(duration, endValueV3, vibrato: optionalInt0, randomness: optionalFloat0, fadeOut: false),
                DOTweenAnimType.ShakeRot => transform.DOShakeRotation(duration, endValueV3, vibrato: optionalInt0, randomness: optionalFloat0, fadeOut: false),
                DOTweenAnimType.UIAnchors => DOTween.To(
                    () => ((RectTransform) target).anchorMin,
                    x => ((RectTransform) target).anchorMin = ((RectTransform) target).anchorMax = x,
                    (Vector2) endValueV3, duration),
                _ => throw new ArgumentOutOfRangeException(nameof(animType), animType, null)
            };
        }
    }
}