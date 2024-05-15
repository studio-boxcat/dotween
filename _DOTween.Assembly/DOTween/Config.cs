using DG.Tweening.Core.Enums;

namespace DG.Tweening
{
    class Config
    {
        /// <summary>If TRUE (default) makes tweens slightly slower but safer, automatically taking care of a series of things
        /// (like targets becoming null while a tween is playing).
        /// <para>Default: TRUE</para></summary>
        public const bool useSafeMode = true;

        /// <summary>Default ease applied to all new Tweeners (not to Sequences which always have Ease.Linear as default).</summary>
        public const Ease defaultEaseType = Ease.OutQuad;
        /// <summary>Default overshoot/amplitude used for eases.</summary>
        public const float defaultEaseOvershootOrAmplitude = 1.70158f;

        public const NestedTweenFailureBehaviour nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.KillWholeSequence;
    }
}