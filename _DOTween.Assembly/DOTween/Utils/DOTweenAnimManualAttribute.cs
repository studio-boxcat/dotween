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
    public sealed class DOTweenAnimManualAttribute : Attribute
    {
        public readonly bool AutoGen;

        public DOTweenAnimManualAttribute(bool autoGen = false) => AutoGen = autoGen;
    }

#if UNITY_EDITOR
    public class DOTweenAnimManualValidator : AttributeValidator<DOTweenAnimManualAttribute, DOTweenAnim>
    {
        protected override void Validate(ValidationResult result)
        {
            var value = ValueEntry.SmartValue;
            if (!value) return;

            if (value.autoGenerate != Attribute.AutoGen)
                result.AddError($"autoGenerate value must be '{Attribute.AutoGen.Literal()}'.");
            if (value.autoPlay)
                result.AddError("autoPlay should be false.");
            if (value.autoKill)
                result.AddError("autoKill should be false.");
        }
    }
#endif
}