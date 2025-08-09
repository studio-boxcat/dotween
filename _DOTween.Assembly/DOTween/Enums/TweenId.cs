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
        private static uint _front = 1; // 0 is reserved for Invalid
        private static uint _back = uint.MaxValue;

        internal static TweenId IssueActivationId() => (TweenId) _front++;
        public static TweenId IssueFixedId() => (TweenId) _back--;
        internal static bool IsValidFixedId(TweenId id) => id.Val() > _back;
    }

    internal static class TweenEnumExtensions
    {
        public static uint Val(this TweenId id) => (uint) id;
        public static string Str(this TweenId id) => id.Val().ToString();
        public static bool IsValid(this TweenId id) => id is not TweenId.Invalid;
        public static bool IsInvalid(this TweenId id) => id is TweenId.Invalid;

        public static bool IsValid(this TweenUpdateId id) => id is not TweenUpdateId.Invalid;
        public static bool IsInvalid(this TweenUpdateId id) => id is TweenUpdateId.Invalid;
    }
}