namespace DG.Tweening
{
    internal enum TweenUpdateId
    {
        Invalid = -1,
    }

    internal static class TweenUpdateIdExtensions
    {
        public static bool IsValid(this TweenUpdateId id) => id is not TweenUpdateId.Invalid;
        public static bool IsInvalid(this TweenUpdateId id) => id is TweenUpdateId.Invalid;
    }
}