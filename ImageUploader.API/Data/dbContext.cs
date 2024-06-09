using Microsoft.EntityFrameworkCore;
public class dbContext : DbContext
{
    public dbContext(DbContextOptions<dbContext> options)
        : base(options)
    {
    }

    public DbSet<Entidades.Image> Image { get; set; } = default!;
}
