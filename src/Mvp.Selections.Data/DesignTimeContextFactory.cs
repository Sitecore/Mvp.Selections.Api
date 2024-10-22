using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mvp.Selections.Data
{
#if DEBUG
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<Context> optionsBuilder = new();
            optionsBuilder.UseSqlServer(
                "Data Source=(local);Initial Catalog=mvp-selections-prod-2023-10-25-18-25;Integrated Security=True;Trust Server Certificate=True");

            return new Context(optionsBuilder.Options);
        }
    }
#endif
}
