using FilesStorage.Models;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DbSet<FileRecord> FileRecords { get; set; }

    public override int SaveChanges() => base.SaveChanges();

    public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileRecord>(entity =>
        {
            entity.ToTable("file_records");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.ContentType).HasColumnName("content_type");
        });
    }
}
