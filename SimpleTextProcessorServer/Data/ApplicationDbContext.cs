using Microsoft.EntityFrameworkCore;

namespace SimpleTextProcessorServer.Data;

public sealed class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WordDictionary>().ToTable(nameof(WordDictionary));
        modelBuilder.Entity<WordDictionary>().HasKey(w => w.Id);
        modelBuilder.Entity<WordDictionary>().HasIndex(w => w.Word).IsUnique();
        modelBuilder.Entity<WordDictionary>().Property(w => w.Word).HasColumnType("varchar").HasMaxLength(15).IsRequired();
        modelBuilder.Entity<WordDictionary>().Property(w => w.Counter).HasColumnType("int").IsRequired();
    }

    public DbSet<WordDictionary> WordDictionary { get; set; } = null!;
}
