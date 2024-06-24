using Azure.Search.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Azure;
using CloudDevPOE.Services;

namespace CloudDevPOE
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Add session services.
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			// Add IHttpContextAccessor to the DI container using the built-in method.
			builder.Services.AddHttpContextAccessor();

			// Explicitly add IConfiguration to the DI container.
			builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

			// Add the SearchClient
			builder.Services.AddSingleton(serviceProvider =>
			{
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();
				var searchServiceName = configuration["SearchServiceName"];
				var apiKey = new AzureKeyCredential(configuration["SearchApiKey"]);
				var indexName = configuration["SearchIndexName"];

				var endpoint = $"https://{searchServiceName}.search.windows.net";

				return new SearchClient(new Uri(endpoint), indexName, apiKey);
			});

			// Add the SearchService
			builder.Services.AddSingleton<SearchService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			// Use session middle ware.
			app.UseSession();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}