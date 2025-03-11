using Microsoft.EntityFrameworkCore;

public class PasswordDbContext : DbContext
{
    public DbSet<PasswordEntry> passwordentry { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=./database.sqlite");
    }
}
