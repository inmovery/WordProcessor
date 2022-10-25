using Microsoft.EntityFrameworkCore;
using WordProcessor.Data.Entities;

namespace WordProcessor.Data
{
	public sealed class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{
			//ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		}

		public DbSet<Word> Words { get; set; } = default!;
	}
}
