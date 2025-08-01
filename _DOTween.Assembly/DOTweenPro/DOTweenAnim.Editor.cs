#if UNITY_EDITOR
// ReSharper disable InconsistentNaming

#nullable enable
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace DG.Tweening
{
    /// <summary>
    /// Attach this to a GameObject to create a tween
    /// </summary>
    [ExecuteAlways]
    public sealed partial class DOTweenAnim : ISelfValidator
    {
        private void OnValidate()
        {
            var dirty = false;

            /*
            if (animationType is DOTweenAnimType.Rotate or DOTweenAnimType.Scale
                && optionalBool1)
            {
                optionalBool1 = false; // optionalBool1 is not valid for Rotate or Scale.
                dirty = true;
            }
            */

            /*
            if (animationType is DOTweenAnimType.Fade
                && endValueFloat != 0)
            {
                endValueV3 = new Vector3(endValueFloat, 0, 0);
                endValueFloat = 0;
                dirty = true;
            }

            if (animationType is DOTweenAnimType.Scale
                && optionalBool0
                && endValueFloat != 0)
            {
                endValueV3 = new Vector3(endValueFloat, endValueFloat, endValueFloat);
                endValueFloat = 0;
                dirty = true;
            }

            if (animationType is DOTweenAnimType.Scale
                && !optionalBool0) // non-uniform scale, unused.
            {
                endValueFloat = 0;
                dirty = true;
            }
            */

            if (dirty)
                EditorUtility.SetDirty(this);
        }

        void ISelfValidator.Validate(SelfValidationResult result)
        {
            if (animationType is DOTweenAnimType.None)
                result.AddError("AnimationType must be set to a valid value");

            if (animationType
                is DOTweenAnimType.Move
                or DOTweenAnimType.PunchPos
                or DOTweenAnimType.ShakePos)
            {
                if (optionalBool0)
                    result.AddError("Snapping is not supported anymore.");
            }

            if (animationType is DOTweenAnimType.Rotate)
            {
                if (endValueV3.x != 0 || endValueV3.y != 0)
                    result.AddError("Rotate can only rotate on the Z axis");
                if (isRelative is false)
                    result.AddError("Rotate must be relative. Otherwise, it would result unexpected rotation.");
            }

            if (animationType
                is DOTweenAnimType.PunchPos
                or DOTweenAnimType.PunchRot
                or DOTweenAnimType.PunchScale)
            {
                if (easeType is not Ease.OutQuad)
                    result.AddError("Punch must use OutQuad ease type.");
                if (isFrom)
                    result.AddError("Punch cannot be from.");
            }

            if (animationType
                is DOTweenAnimType.ShakePos
                or DOTweenAnimType.ShakeRot
                or DOTweenAnimType.ShakeScale)
            {
                if (easeType is not Ease.Linear)
                    result.AddError("Shake must use Linear ease type.");
                if (isFrom)
                    result.AddError("Shake cannot be from.");
            }

            if (animationType
                is DOTweenAnimType.Fade
                or DOTweenAnimType.Color
                or DOTweenAnimType.PunchPos
                or DOTweenAnimType.PunchRot
                or DOTweenAnimType.PunchScale
                or DOTweenAnimType.ShakePos
                or DOTweenAnimType.ShakeRot
                or DOTweenAnimType.ShakeScale
                or DOTweenAnimType.UIAnchors)
            {
                if (isRelative)
                    result.AddError(animationType + " cannot be relative.");
            }
        }
    }
}
#endif