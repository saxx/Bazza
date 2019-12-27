using Microsoft.EntityFrameworkCore;

namespace Bazza.Models.Database
{
    public sealed class Db : DbContext
    {
        public Db(DbContextOptions options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>().HasKey(x => x.PersonId);
            builder.Entity<Article>().HasKey(x => new { x.PersonId, x.ArticleId });

            base.OnModelCreating(builder);
        }
    }
}