using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WordProcessor.Data;
using WordProcessor.Data.Contracts;
using WordProcessor.Data.Repositories;

namespace WordProcessor
{
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var builder = CreateHostBuilder().Build();

			using var serviceScope = builder.Services.CreateScope();
			var databaseContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();

			var areThereAnyPendingMigrations = databaseContext.Database.GetPendingMigrations().Any();
			if (areThereAnyPendingMigrations)
				databaseContext.Database.Migrate();

			var serviceProvider = serviceScope.ServiceProvider;

			var form = serviceProvider.GetRequiredService<MainForm>();
			Application.Run(form);
		}

		private static IHostBuilder CreateHostBuilder()
		{
			return Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices);
		}

		private static void ConfigureServices(this IServiceCollection services)
		{
			services.AddScoped<MainForm>();

			// When need use EF tools via package manager
			//var connectionString = "Server=localhost;Database=WordsDatabase;MultipleActiveResultSets=True;Trusted_Connection=True;TrustServerCertificate=true;";

			var connectionString = ConfigurationManager.ConnectionStrings["ProductionDatabase"].ConnectionString;
			services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseSqlServer(connectionString, assembly =>
				{
					assembly.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName);
				});
				options.EnableSensitiveDataLogging(false);
			});

			services.AddTransient<IWordRepository, WordRepository>();
		}
	}
}