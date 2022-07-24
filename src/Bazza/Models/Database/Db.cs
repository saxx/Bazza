using Microsoft.EntityFrameworkCore;

namespace Bazza.Models.Database;

public sealed class Db : DbContext
{
    public Db(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Person>(b => { b.HasKey(x => x.PersonId); });
        builder.Entity<Article>(b => { b.HasKey(x => new { x.PersonId, x.ArticleId }); });
        builder.Entity<User>(b => { b.HasKey(x => x.Username); });
        builder.Entity<Setting>(b => { b.HasKey(x => x.Key); });

        base.OnModelCreating(builder);
    }
}