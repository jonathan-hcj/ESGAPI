using ESGAPI;
using ESGAPI.Contexts;
using ESGAPI.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

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

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ESG Dummy API",
        Description = "An ASP.NET Core Web API for adding and querying customers",
        Contact = new OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("https://esgglobal.com/contact-2/")
        },
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


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
