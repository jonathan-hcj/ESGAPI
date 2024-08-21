using ESGAPI;
using ESGAPI.Contexts;
using ESGAPI.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                ("BasicAuthentication", null);

builder.Services.AddDbContext<CustomerDbContext>(option => 
    option.UseSqlServer(builder.Configuration.GetConnectionString(nameof(Customer))));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.MapGet("/customer", () => "This endpoint requires authorization")
        .RequireAuthorization();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
