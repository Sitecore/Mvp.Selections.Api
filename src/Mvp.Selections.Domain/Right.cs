namespace Mvp.Selections.Domain
{
    [Flags]
    public enum Right
    {
        #pragma warning disable SA1025 // Code should not contain multiple whitespace in a row - Nice readable formatting

        Any           = 0b_0000_0000,
        Admin         = 0b_0000_0010,
        Apply         = 0b_0000_0100,
        Review        = 0b_0000_1000

        #pragma warning restore SA1025 // Code should not contain multiple whitespace in a row - Nice readable formatting
    }
}
