using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesStorage.Models;
using FilesStorage.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestDatabaseContext : DbContext, IDatabaseContext
{
    public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options)
        : base(options) { }

    public DbSet<FileRecord> FileRecords { get; set; } = null!;

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}

public class FileRepositoryTests
{
    private DbContextOptions<TestDatabaseContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<TestDatabaseContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddSync_ShouldAddFileRecord()
    {
        var options = CreateNewContextOptions();

        using (var context = new TestDatabaseContext(options))
        {
            var repository = new FileRepository(context);
            var file = new FileRecord { FileName = "Teste.png", ContentType = "jpeg" };

            await repository.AddAsync(file);

            Assert.Equal(1, await context.FileRecords.CountAsync());
            Assert.Equal("Teste.png", (await context.FileRecords.FirstAsync()).FileName);
            Assert.Equal("jpeg", (await context.FileRecords.FirstAsync()).ContentType);
        }
    }

    [Fact]
    public async Task GetByName_ShouldAsyncReturnCorrectFile()
    {
        var options = CreateNewContextOptions();

        using (var context = new TestDatabaseContext(options))
        {
            context.FileRecords.Add(
                new FileRecord { FileName = "Teste1.png", ContentType = "jpeg" }
            );
            context.FileRecords.Add(
                new FileRecord { FileName = "Teste2.png", ContentType = "img" }
            );

            await context.SaveChangesAsync();
        }

        using (var context = new TestDatabaseContext(options))
        {
            var repository = new FileRepository(context);
            var file = await repository.GetByNameAsync("Teste2.png");

            Assert.NotNull(file);
            Assert.Equal("Teste2.png", file!.FileName);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnFiles()
    {
        var options = CreateNewContextOptions();

        using (var context = new TestDatabaseContext(options))
        {
            context.FileRecords.Add(
                new FileRecord { FileName = "Teste1.png", ContentType = "jpeg" }
            );
            context.FileRecords.Add(
                new FileRecord { FileName = "Teste2.png", ContentType = "img" }
            );

            await context.SaveChangesAsync();
        }

        using (var context = new TestDatabaseContext(options))
        {
            var repository = new FileRepository(context);
            var files = await repository.GetAllAsync();

            Assert.Equal(2, files.Count);
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveFileRecord()
    {
        var options = CreateNewContextOptions();

        using (var context = new TestDatabaseContext(options))
        {
            var file = new FileRecord { FileName = "file1.txt", ContentType = "text/plain" };
            context.FileRecords.Add(file);
            await context.SaveChangesAsync();

            var repository = new FileRepository(context);
            await repository.DeleteAsync(file);

            Assert.Equal(0, await context.FileRecords.CountAsync());
        }
    }
}
