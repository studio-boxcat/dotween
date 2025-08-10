namespace DG.Tweening
{
    public enum TweenId : uint
    {
        Invalid = 0,
    }

    internal enum TweenUpdateId
    {
        Invalid = -1,
    }

    public static class TweenIdIssuer
    {
        private static uint _next = 1; // 0 is reserved for Invalid
        internal static TweenId Issue() => (TweenId) _next++;
    }

    public static class TweenEnumExtensions
    {
        public static uint Val(this TweenId id) => (uint) id;
        public static string Str(this TweenId id) => id.Val().ToString();
        public static bool IsValid(this TweenId id) => id is not TweenId.Invalid;
        public static bool IsInvalid(this TweenId id) => id is TweenId.Invalid;

        internal static bool IsValid(this TweenUpdateId id) => id is not TweenUpdateId.Invalid;
        internal static bool IsInvalid(this TweenUpdateId id) => id is TweenUpdateId.Invalid;
    }
}