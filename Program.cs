using Application;
using System.Reflection;
using NLog.Web;
using Microsoft.OpenApi.Models;



public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        // Add services to the container.
        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();
        builder.Services.AddApplicationServices();
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Aggregator",
                Version = "v1",
                Description = "Supported sort options: date, date_desc, title, title_desc"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
        });

        builder.Services.AddLogging();
        builder.Services.AddSingleton<IApiStatsTracker, InMemoryApiStatsTracker>();
        builder.Services.AddMemoryCache();
        builder.Services.Configure<CacheSettings>(
            builder.Configuration.GetSection("CacheSettings"));


        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        // app.UseAuthentication(); // (optional)
        app.UseAuthorization();

        app.MapControllers();

        app.Run();


    }
}

