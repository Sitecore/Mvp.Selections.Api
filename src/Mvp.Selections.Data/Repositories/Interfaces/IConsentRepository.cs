using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IConsentRepository : IBaseRepository<Consent, Guid>
    {
        Task<IList<Consent>> GetAllForUserAsync(Guid userId);

        Task<Consent?> GetForUserAsync(Guid userId, ConsentType type);
    }
}
