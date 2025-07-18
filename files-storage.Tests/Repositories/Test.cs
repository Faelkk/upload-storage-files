using System.Collections.Generic;
using System.Threading.Tasks;
using FilesStorage.Models;
using FilesStorage.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class FileRepositoryRealDbTests
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=files_db;Username=postgres;Password=yourpassword";

    private DbContextOptions<DatabaseContext> CreateRealDbOptions()
    {
        return new DbContextOptionsBuilder<DatabaseContext>().UseNpgsql(ConnectionString).Options;
    }

    private async Task PrepareDatabaseAsync()
    {
        var options = CreateRealDbOptions();
        using var context = new DatabaseContext(options);

        await context.Database.MigrateAsync();

        context.FileRecords.RemoveRange(context.FileRecords);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldAddFileRecord()
    {
        await PrepareDatabaseAsync();
        var options = CreateRealDbOptions();

        using var context = new DatabaseContext(options);
        var repo = new FileRepository(context);

        var file = new FileRecord { FileName = "file1.png", ContentType = "image/png" };
        await repo.AddAsync(file);
        await context.SaveChangesAsync();

        var count = await context.FileRecords.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnCorrectFile()
    {
        await PrepareDatabaseAsync();
        var options = CreateRealDbOptions();

        using (var context = new DatabaseContext(options))
        {
            context.FileRecords.AddRange(
                new[]
                {
                    new FileRecord { FileName = "file1.png", ContentType = "image/png" },
                    new FileRecord { FileName = "file2.png", ContentType = "image/jpeg" },
                }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new DatabaseContext(options))
        {
            var repo = new FileRepository(context);
            var file = await repo.GetByNameAsync("file2.png");

            Assert.NotNull(file);
            Assert.Equal("file2.png", file!.FileName);
            Assert.Equal("image/jpeg", file.ContentType);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFiles()
    {
        await PrepareDatabaseAsync();
        var options = CreateRealDbOptions();

        using (var context = new DatabaseContext(options))
        {
            context.FileRecords.AddRange(
                new[]
                {
                    new FileRecord { FileName = "file1.png", ContentType = "image/png" },
                    new FileRecord { FileName = "file2.png", ContentType = "image/jpeg" },
                }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new DatabaseContext(options))
        {
            var repo = new FileRepository(context);
            var files = await repo.GetAllAsync();

            Assert.Equal(2, files.Count);
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveFile()
    {
        await PrepareDatabaseAsync();
        var options = CreateRealDbOptions();

        using (var context = new DatabaseContext(options))
        {
            var file = new FileRecord
            {
                FileName = "file-to-delete.png",
                ContentType = "image/png",
            };
            context.FileRecords.Add(file);
            await context.SaveChangesAsync();

            var repo = new FileRepository(context);
            await repo.DeleteAsync(file);
            await context.SaveChangesAsync();

            var count = await context.FileRecords.CountAsync();
            Assert.Equal(0, count);
        }
    }
}
