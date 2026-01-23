using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mvp.Selections.Data;

#if DEBUG
public class DesignTimeContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<Context> optionsBuilder = new();
        optionsBuilder.UseSqlServer(
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Code\\Mvp.Selections\\data\\Temp.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True");

        return new Context(optionsBuilder.Options);
    }
}
#endif