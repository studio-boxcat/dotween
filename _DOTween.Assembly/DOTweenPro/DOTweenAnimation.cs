using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#pragma warning disable 1591
namespace DG.Tweening
{
    /// <summary>
    /// Attach this to a GameObject to create a tween
    /// </summary>
    [AddComponentMenu("DOTween/DOTween Animation")]
    public sealed class DOTweenAnimation : MonoBehaviour
#if UNITY_EDITOR
        , ISelfValidator
#endif
    {
        public enum AnimationType : byte
        {
            None = 0,
            // Unused_8 = 1,
            LocalMove = 2,
            // Unused_9 = 3,
            LocalRotateZ = 4,
            Scale = 5,
            Color = 6,
            Fade = 7,
            // Unused_0 = 8,
            PunchPosition = 9,
            PunchRotation = 10,
            PunchScale = 11,
            ShakePosition = 12,
            ShakeRotation = 13,
            ShakeScale = 14,
            // Unused_1 = 15,
            // Unused_2 = 16,
            // Unused_3 = 17,
            // Unused_4 = 18,
            // Unused_5 = 19,
            // Unused_6 = 20,
            // Unused_7 = 21,
            UIAnchors = 22,
        }

        [NonSerialized]
        public Tweener tween;

        public float delay;
        public float duration = 1;
        public Ease easeType = Ease.OutQuad;
        public AnimationCurve easeCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public LoopType loopType = LoopType.Restart;
        public int loops = 1;
        public bool isRelative;
        public bool isFrom;
        public bool autoKill = true;
        public bool autoGenerate = true; // If TRUE automatically creates the tween at startup

        [Required, ChildGameObjectsOnly]
        public Component target;
        public AnimationType animationType;
        public bool autoPlay = true;

        public float endValueFloat;
        public Vector3 endValueV3;
        public Color endValueColor = new Color(1, 1, 1, 1);

        public bool optionalBool0, optionalBool1;
        public float optionalFloat0;
        public int optionalInt0;

        bool _tweenAutoGenerationCalled; // TRUE after the tweens have been autoGenerated

        #region Unity Methods

        void Awake()
        {
            if (!autoGenerate) return;

            CreateTween(false, autoPlay);
            _tweenAutoGenerationCalled = true;
        }

        void Start()
        {
            if (_tweenAutoGenerationCalled || !autoGenerate) return;

            CreateTween(false, autoPlay);
            _tweenAutoGenerationCalled = true;
        }

        void OnDestroy()
        {
            if (tween != null)
            {
                if (tween.active)
                    tween.Kill();
                tween = null;
            }
        }

        // Used also by DOTweenAnimationInspector when applying runtime changes and restarting
        /// <summary>
        /// Creates the tween manually (called automatically if AutoGenerate is set in the Inspector)
        /// from its target's current value.
        /// </summary>
        /// <param name="regenerateIfExists">If TRUE and an existing tween was already created (and not killed), kills it and recreates it with the current
        /// parameters. Otherwise, if a tween already exists, does nothing.</param>
        /// <param name="andPlay">If TRUE also plays the tween, otherwise only creates it</param>
        public void CreateTween(bool regenerateIfExists = false, bool andPlay = true)
        {
            Assert.AreNotEqual(AnimationType.None, animationType, "AnimationType is None");
            Assert.IsNotNull(target, "Target is null");

            if (tween != null)
            {
                if (tween.active)
                {
                    if (regenerateIfExists) tween.Kill();
                    else return;
                }
                tween = null;
            }

            tween = CreateTweenInstance();
            tween.OnKill(() => tween = null);
            if (andPlay is false)
                tween.Pause();
        }

        [MustUseReturnValue]
        public Tweener CreateTweenInstance()
        {
            // Create tween.
            var t = CreateTween(
                target, transform, animationType, duration,
                endValueFloat, endValueV3, endValueColor,
                optionalBool0, optionalBool1, optionalFloat0, optionalInt0);

            // Set from or relative.
            if (isFrom) t.From(isRelative);
            else t.SetRelative(isRelative);

            // Set basic tween settings.
            t.SetTarget(gameObject)
                .SetDelay(delay).SetLoops(loops, loopType).SetAutoKill(autoKill);

            // Set easeType.
            if (easeType is Ease.INTERNAL_Custom) t.SetEase(easeCurve);
            else t.SetEase(easeType);

            return t;
        }

        #endregion

        [NotNull]
        static Tweener CreateTween(
            Object target, Transform transform,
            AnimationType animationType,
            float duration,
            float endValueFloat,
            Vector3 endValueV3,
            Color endValueColor,
            bool optionalBool0,
            bool optionalBool1,
            float optionalFloat0,
            int optionalInt0)
        {
            return animationType switch
            {
                AnimationType.LocalMove => transform.DOLocalMove(endValueV3, duration),
                AnimationType.LocalRotateZ => transform.DOLocalRotateZ(endValueV3.z, duration),
                AnimationType.Scale => transform.DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration),
                AnimationType.Color => target switch
                {
                    Graphic t => t.DOColor(endValueColor, duration),
                    SpriteRenderer t => t.DOColor(endValueColor, duration),
                    Renderer t => t.material.DOColor(endValueColor, duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                AnimationType.Fade => target switch
                {
                    CanvasGroup t => t.DOFade(endValueFloat, duration),
                    Graphic t => t.DOFade(endValueFloat, duration),
                    SpriteRenderer t => t.DOFade(endValueFloat, duration),
                    Renderer t => t.material.DOFade(endValueFloat, duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                AnimationType.PunchPosition => target switch
                {
                    RectTransform t => t.DOPunchAnchorPos(endValueV3, duration, optionalInt0, optionalFloat0),
                    Transform t => t.DOPunchPosition(endValueV3, duration, optionalInt0, optionalFloat0),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                AnimationType.PunchScale => transform.DOPunchScale(endValueV3, duration, optionalInt0, optionalFloat0),
                AnimationType.PunchRotation => transform.DOPunchRotation(endValueV3, duration, optionalInt0, optionalFloat0),
                AnimationType.ShakePosition => target switch
                {
                    RectTransform t => t.DOShakeAnchorPos(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool1),
                    Transform t => t.DOShakePosition(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool1),
                    _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                },
                AnimationType.ShakeScale => transform.DOShakeScale(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool1),
                AnimationType.ShakeRotation => transform.DOShakeRotation(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool1),
                AnimationType.UIAnchors => DOTween.To(() => ((RectTransform) target).anchorMin, x => ((RectTransform) target).anchorMin = ((RectTransform) target).anchorMax = x, (Vector2) endValueV3, duration),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null)
            };
        }

#if UNITY_EDITOR
        void ISelfValidator.Validate(SelfValidationResult result)
        {
            if (animationType is AnimationType.None)
                result.AddError("AnimationType must be set to a valid value");

            if (animationType
                is AnimationType.LocalMove
                or AnimationType.PunchPosition
                or AnimationType.ShakePosition)
            {
                if (optionalBool0)
                    result.AddError("Punch and Shake animations cannot have snapping enabled");
            }

            if (animationType is AnimationType.LocalRotateZ)
            {
                if (endValueV3.x != 0 || endValueV3.y != 0)
                    result.AddError("LocalRotate animation can only rotate on the Z axis");
            }

            if (animationType
                is AnimationType.PunchPosition
                or AnimationType.PunchRotation
                or AnimationType.PunchScale)
            {
                if (easeType is not Ease.OutQuad)
                    result.AddError("Punch animations must use OutQuad ease type");
                if (isRelative)
                    result.AddError("Punch animations cannot be relative");
                if (isFrom)
                    result.AddError("Punch animations cannot be from");
            }

            if (animationType
                is AnimationType.ShakePosition
                or AnimationType.ShakeRotation
                or AnimationType.ShakeScale)
            {
                if (easeType is not Ease.Linear)
                    result.AddError("Shake animations must use Linear ease type");
                if (isRelative)
                    result.AddError("Shake animations cannot be relative");
                if (isFrom)
                    result.AddError("Shake animations cannot be from");
            }

            if (animationType
                is AnimationType.Fade
                or AnimationType.Color
                or AnimationType.PunchPosition
                or AnimationType.PunchRotation
                or AnimationType.PunchScale
                or AnimationType.ShakePosition
                or AnimationType.ShakeRotation
                or AnimationType.ShakeScale
                or AnimationType.UIAnchors)
            {
                if (isRelative)
                    result.AddError("Fade and Color animations cannot be relative");
            }
        }
#endif
    }
}