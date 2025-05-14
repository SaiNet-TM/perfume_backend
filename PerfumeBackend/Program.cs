using Microsoft.EntityFrameworkCore;
using PerfumeBackend.Data;
using PerfumeBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PerfumeContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "PerfumeBackend API", 
        Version = "v1",
        Description = "API для управления каталогом парфюмов"
    });
});

var app = builder.Build();

await InitializeDatabase(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PerfumeBackend v1");
        c.RoutePrefix = string.Empty; // Делаем Swagger UI доступным по корневому URL
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PerfumeContext>();
    db.Database.EnsureCreated(); // Создаст таблицы, если их нет
}

app.Run();

async Task InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<PerfumeContext>();
        await context.Database.MigrateAsync();
        
        if (!context.Portfolio.Any())
        {
            context.Portfolio.Add(new Portfolio 
            {
                Title = "DIOR SAUVAGE EDT",
                Description = "The strong gust of Citrus in Sauvage Eau de Toilette is powerfully anchored by the ambery nobleness of Ambroxan, resinous Elemi and Woods that bind the whole. <br/> Like a deep breath of fresh air, Sauvage Eau de Toilette is a bold composition for a man who is true to himself.",
                Notes = new[] { "Ambroxan", "Bergamot", "Amberwood" },
                Price = 175,
                Volume = 100,
                ImageUrl = "/images/dior-sauvage.jpg"
            });
        }

        if (!context.Services.Any())
        {
            context.Services.Add(new Service 
            {
                Title = "Персональный подбор ароматов",
                Description = "Анализ ваших предпочтений и стиля жизни",
                Features = new[] 
                {
                    "Дегустация 5 эксклюзивных композиций",
                    "Рекомендации по сезонному использованию",
                    "Фирменный набор пробников"
                },
                Price = 200,
                ImageUrl = "/images/service.jpg"
            });
        }

        await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации БД");
    }
}