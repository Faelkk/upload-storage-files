using Microsoft.EntityFrameworkCore;

public class SeedDataBase
{
    public static void ApplyMigrationsOnDataBase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
    }
}
