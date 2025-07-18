using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FilesStorage.Persistence;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

        // substitua pela sua connection string correta
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=files_db;Username=postgres;Password=yourpassword"
        );

        return new DatabaseContext(optionsBuilder.Options);
    }
}
