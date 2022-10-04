﻿using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role, Guid>
    {
        Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role;

        Task<T?> GetAsync<T>(Guid id)
            where T : Role;
    }
}
