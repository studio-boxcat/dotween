namespace DG.Tweening
{
    class Const
    {
        /// <summary>Default ease applied to all new Tweeners (not to Sequences which always have Ease.Linear as default).</summary>
        public const Ease defaultEaseType = Ease.OutQuad;
        /// <summary>Default overshoot/amplitude used for eases.</summary>
        public const float defaultEaseOvershootOrAmplitude = 1.70158f;
    }
}