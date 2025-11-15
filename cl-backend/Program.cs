using cl_backend;
using cl_backend.DbContexts;
using cl_backend.Models;
using cl_backend.Services;
using cl_backend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// ������������ ApplicationContext
builder.Services.AddDbContext<ApplicationContext>();
// ����������� ������� ��������
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddAuthentication();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c=>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
        {
            Name= "Authorization",
            Type= SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter token in format 'bearer <space> token'"
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {{
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
     });
});

var app = builder.Build();

// Автоматическое применение миграций и инициализация данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Применение миграций...");
        context.Database.Migrate();
        logger.LogInformation("Миграции успешно применены");

        // Инициализация пользователей по умолчанию
        if (!context.Users.Any())
        {
            logger.LogInformation("Создание пользователей по умолчанию...");

            var adminUser = new User
            {
                Login = "admin@admin.admin",
                Password = AuthUtils.HashPassword("admin@admin.admin"),
                Role = "admin"
            };

            var testUser = new User
            {
                Login = "test@test.test",
                Password = AuthUtils.HashPassword("test@test.test"),
                Role = "user"
            };

            context.Users.AddRange(adminUser, testUser);
            context.SaveChanges();

            logger.LogInformation("Пользователи созданы: admin@admin.admin (admin), test@test.test (user)");
        }
        else
        {
            logger.LogInformation("Пользователи уже существуют, пропуск инициализации");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных");
        throw;
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
