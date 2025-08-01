#if UNITY_EDITOR
// ReSharper disable InconsistentNaming

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening
{
    public sealed partial class DOTweenAnim : ISelfValidator
    {
        private static readonly Dictionary<DOTweenAnimType, Type[]> _eligibleTargetTypes = new()
        {
            { DOTweenAnimType.None, new[] { typeof(Transform) } }, // placeholder.
            { DOTweenAnimType.Move, new[] { typeof(Transform) } },
            { DOTweenAnimType.MoveY, new[] { typeof(Transform) } },
            { DOTweenAnimType.Rotate, new[] { typeof(Transform) } },
            { DOTweenAnimType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimType.Fade, new[] { typeof(CanvasGroup), typeof(Graphic), typeof(SpriteRenderer) } },
            { DOTweenAnimType.PunchPos, new[] { typeof(Transform) } },
            { DOTweenAnimType.PunchRot, new[] { typeof(Transform) } },
            { DOTweenAnimType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimType.ShakePos, new[] { typeof(Transform) } },
            { DOTweenAnimType.ShakeRot, new[] { typeof(Transform) } },
            { DOTweenAnimType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimType.UIAnchors, new[] { typeof(RectTransform) } },
        };

        public static Type[] GetEligibleTargetTypes(DOTweenAnimType animType) => _eligibleTargetTypes[animType];

        void ISelfValidator.Validate(SelfValidationResult result)
        {
            if (target)
            {
                var eligibleTypes = GetEligibleTargetTypes(animType);
                if (eligibleTypes.Any(t => t.IsInstanceOfType(target)) is false)
                {
                    result.AddError($"Target type {target.GetType()} is not eligible for {animType} animation. " +
                                    $"Eligible types: {string.Join(", ", eligibleTypes.Select(t => t.Name))}");
                }
            }

            if (animType is DOTweenAnimType.None)
            {
                result.AddError("AnimationType must be set to a valid value");
            }
            else if (animType is DOTweenAnimType.Rotate)
            {
                if (endValue.x != 0 || endValue.y != 0)
                    result.AddError("Rotate can only rotate on the Z axis");
                if (isRelative is false)
                    result.AddError("Rotate must be relative. Otherwise, it would result unexpected rotation.");
            }
            else if (animType
                     is DOTweenAnimType.PunchPos
                     or DOTweenAnimType.PunchRot
                     or DOTweenAnimType.PunchScale)
            {
                if (easeType is not Ease.OutQuad)
                    result.AddError("Punch must use OutQuad ease type.");
                if (isFrom)
                    result.AddError("Punch cannot be from.");
            }
            else if (animType
                     is DOTweenAnimType.ShakePos
                     or DOTweenAnimType.ShakeRot
                     or DOTweenAnimType.ShakeScale)
            {
                if (easeType is not Ease.Linear)
                    result.AddError("Shake must use Linear ease type.");
                if (isFrom)
                    result.AddError("Shake cannot be from.");
            }
            else if (animType
                     is DOTweenAnimType.Fade
                     or DOTweenAnimType.PunchPos
                     or DOTweenAnimType.PunchRot
                     or DOTweenAnimType.PunchScale
                     or DOTweenAnimType.ShakePos
                     or DOTweenAnimType.ShakeRot
                     or DOTweenAnimType.ShakeScale
                     or DOTweenAnimType.UIAnchors)
            {
                if (isRelative)
                    result.AddError(animType + " cannot be relative.");
            }
        }
    }
}
#endif