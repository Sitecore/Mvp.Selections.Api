﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ReviewRepository : BaseRepository<Review, Guid>, IReviewRepository
    {
        public ReviewRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<IList<Review>> GetAllAsync(Guid applicationId, int page = 1, short pageSize = 100, params Expression<Func<Review, object>>[] includes)
        {
            page--;
            return await Context.Reviews
                .Where(r => r.Application.Id == applicationId)
                .OrderByDescending(r => r.CreatedOn)
                .ThenBy(r => r.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }
    }
}
