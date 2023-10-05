using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PetShop.Api.Database;
using System.Reflection;

namespace PetShop.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var defaultProblemDetailsFactory = context.HttpContext.RequestServices.GetService<ProblemDetailsFactory>();
                        var problemDetails = defaultProblemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                        problemDetails.Title = "Ocorreram erros de validação";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;

                        return new UnprocessableEntityObjectResult(problemDetails);
                    };
                });


            builder.Services.AddFluentValidation();

            var connectionString = builder.Configuration.GetConnectionString("Default");
            var dbPassword = builder.Configuration["DatabasePassword"];
            var dbServerName = builder.Configuration["DatabaseServerName"];

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.Password = dbPassword;
            connectionStringBuilder.DataSource = dbServerName;

            builder.Services.AddDbContext<IPetShopDbContext, PetShopDbContext>(options =>
            {
                options.UseSqlServer(connectionStringBuilder.ConnectionString);
            });

            builder.Services.AddSwaggerGen(options =>
            {
                var openApiInfo = new OpenApiInfo();

                openApiInfo.Title = "Documentação de WebApi Petshop";
                openApiInfo.Description = "A documentação relata os métodos e utilizações desta API";
                openApiInfo.License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri(@"http://www.mit.com/license")
                };
                openApiInfo.Contact = new OpenApiContact()
                {
                    Name = "Marianna Correa",
                    Email = "mahvalenterj@gmail.com"
                };

                options.SwaggerDoc("v1", openApiInfo);

                var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var path = Path.Combine(AppContext.BaseDirectory, fileName);
                options.IncludeXmlComments(path, true);
            });

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

            app.Run();
        }
    }
}