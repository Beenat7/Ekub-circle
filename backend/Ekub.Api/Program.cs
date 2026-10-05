using Microsoft.EntityFrameworkCore;
using Ekub.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);


// EF Core

builder.Services.AddDbContext<EkubDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("starterDatabase")
    )
);
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
