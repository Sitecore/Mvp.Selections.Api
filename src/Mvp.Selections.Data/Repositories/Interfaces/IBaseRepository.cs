namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IBaseRepository
    {
        void SaveChanges();

        Task SaveChangesAsync();
    }
}
