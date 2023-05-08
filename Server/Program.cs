using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Intialize DB Context.
            builder.Services.AddDbContext<OnboardingBotContext>(options =>
               options.UseNpgsql("Host=localhost;Port=5432;Database=OnboardingBot;Username=onboardingbot;Password=onboardingbot"));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

                //builder.Services.AddTransient<CodeService>();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }

                //builder.Services.AddAutoMapper(typeof(Mappings));

            builder.Services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
                options.JsonSerializerOptions.Converters.Add(new ObjectToInferredTypesConverter());

                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition
                    = JsonIgnoreCondition.WhenWritingNull;
            });
            var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            serializerOptions.PropertyNameCaseInsensitive = true;
            serializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            serializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
            serializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));

            var serializer = new SystemTextJsonContentSerializer(serializerOptions);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            //var db = app.Services.CreateScope().ServiceProvider.GetService<OnboardingBotContext>();
            //await db.Database.EnsureCreatedAsync();

            app.Run();
        }
    }
}