namespace Shared
{
    public record MovementItem
    {
        public string Type { get; init; }
        public string To { get; init; }
        public string From { get; init; }
    }
}