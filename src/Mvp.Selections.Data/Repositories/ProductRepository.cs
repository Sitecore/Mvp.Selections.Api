﻿using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class ProductRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Product, int>(context, currentUserNameProvider), IProductRepository
{
}