#nullable enable

using System;

#if UNITY_EDITOR
using DG.Tweening;
using Sirenix.OdinInspector.Editor.Validation;

[assembly: RegisterValidator(typeof(DOTweenAnimManualValidator))]
#endif

namespace DG.Tweening
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DOTweenAnimManualAttribute : Attribute { }

#if UNITY_EDITOR
    public class DOTweenAnimManualValidator : AttributeValidator<DOTweenAnimManualAttribute, DOTweenAnim>
    {
        protected override void Validate(ValidationResult result)
        {
            var value = ValueEntry.SmartValue;
            if ((value.autoGenerate && value.autoPlay) || value.autoKill)
                result.AddError("DOTweenAnim should not be set to autoGenerate and autoPlay at the same time, or autoKill.");
        }
    }
#endif
}