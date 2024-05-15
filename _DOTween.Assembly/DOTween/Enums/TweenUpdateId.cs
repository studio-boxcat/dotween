namespace DG.Tweening
{
    public enum TweenUpdateId
    {
        Invalid = -1,
    }

    public static class TweenUpdateIdExtensions
    {
        public static bool IsValid(this TweenUpdateId id)
        {
            return id is not TweenUpdateId.Invalid;
        }

        public static bool IsInvalid(this TweenUpdateId id)
        {
            return id is TweenUpdateId.Invalid;
        }
    }
}