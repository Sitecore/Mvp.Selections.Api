namespace Mvp.Selections.Domain.Interfaces
{
    public interface IId<out T>
    {
        public T Id { get; }
    }
}
