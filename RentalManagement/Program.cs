using System;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;

using Microsoft.EntityFrameworkCore;
using RentalManagement.Contexts;

namespace RentalManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RentalManagement API", Version = "v1" });
                c.EnableAnnotations();
            });

            var app = builder.Build();

            // Check database connection
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    // Attempt to query the database
                    dbContext.Database.OpenConnection();
                    dbContext.Database.CloseConnection();
                    Console.WriteLine("Database connection successful");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database connection failed");
                    Console.WriteLine(ex.Message);
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentalManagement API v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}