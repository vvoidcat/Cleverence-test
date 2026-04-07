using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LogFormatter.Options;
using LogFormatter.Services;
using LogFormatter.Parsers.LogEntry;
using LogFormatter.Providers.Factories;
using LogFormatter.Parsers;

namespace LogFormatter;

public class Program
{
	public static async Task Main()
	{
		var services = new ServiceCollection();
		var options = new ApplicationOptions();

		try
		{
			var configuration =
			new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.Build();

			configuration.GetSection("ApplicationOptions").Bind(options);
			options.FileSystemOptions.Validate();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"CONFIGURATION ERROR: {ex.Message}");
			return;
		}

		services.AddTransient(provider => LogEntryParser.Create());
		services.AddTransient(provider => DataReaderFactory.Create(options.FileSystemOptions));
		services.AddTransient(provider => DataWriterFactory.Create(options.FileSystemOptions));
		services.AddSingleton<LogNormalizerService>();

		var serviceProvider = services.BuildServiceProvider();

		try
		{
			var cts = new CancellationTokenSource();
			Console.CancelKeyPress += (sender, e) =>
			{
				Console.WriteLine("Отмена операции...");
				cts.Cancel();
			};

			var app = serviceProvider.GetRequiredService<LogNormalizerService>();
			await app.RunAsync(cts.Token);
		}
		catch (OperationCanceledException) { }
		catch (Exception ex)
		{
			Console.WriteLine($"RUNTIME ERROR: {ex.Message}");
		}
	}
}
