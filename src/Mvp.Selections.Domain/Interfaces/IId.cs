namespace Mvp.Selections.Domain.Interfaces
{
    public interface IId<out T>
    {
        T Id { get; }
    }
}
