using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace AdminPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Intialize DB Context.
            builder.Services.AddDbContext<Server.OnboardingBotContext>(options =>
               options.UseNpgsql("Host=localhost;Port=5432;Database=OnboardingBot;Username=onboardingbot;Password=onboardingbot"));

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<TelegramBotClient>(sp =>
            {
                var botToken = "5654249846:AAHU6xVbyc8vGMpODDM9h9kAfS4khjBUgig";
                return new TelegramBotClient(botToken);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}