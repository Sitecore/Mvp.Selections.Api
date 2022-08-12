namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IBaseRepository
    {
        public void SaveChanges();

        public Task SaveChangesAsync();
    }
}
