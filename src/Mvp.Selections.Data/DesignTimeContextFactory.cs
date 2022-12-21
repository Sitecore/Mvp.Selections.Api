using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mvp.Selections.Data
{
#if DEBUG
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<Context> optionsBuilder = new ();
            optionsBuilder.UseSqlServer(
                "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=C:\\Code\\Mvp.Selections\\data\\Temp.mdf");

            return new Context(optionsBuilder.Options);
        }
    }
#endif
}
