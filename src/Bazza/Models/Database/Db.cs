using Microsoft.EntityFrameworkCore;

namespace Bazza.Models.Database;

public sealed class Db(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Sale> Sales => Set<Sale>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Person>(b =>
        {
            b.HasKey(x => x.PersonId);
        });
        builder.Entity<Article>(b =>
        {
            b.HasKey(x => new { x.PersonId, x.ArticleId });
            b.HasOne(x => x.Sale).WithMany(x => x.Articles).HasForeignKey(x => x.SaleId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<Sale>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
        });
        builder.Entity<User>(b =>
        {
            b.HasKey(x => x.Username);
        });
        builder.Entity<Setting>(b =>
        {
            b.HasKey(x => x.Key);
        });

        base.OnModelCreating(builder);
    }
}