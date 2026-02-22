using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Movies.Services;
using System.Text;

// Setup to build the application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MongoDB
var connectionString = builder.Configuration.GetConnectionString("MongoDB");
var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("MoviesDB");

builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<IAuth, Auth>();

// Add API controllers
builder.Services.AddControllers();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();

// Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter a valid token",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Movies/Index");       // redirect to Index page if error occurs
    app.UseHsts();
}

app.UseHttpsRedirection();                          // Redirect HTTP requests to HTTPS
app.UseStaticFiles();                               // Serve static files (e.g., CSS, JS, images)
app.UseRouting();                                   // Enable routing
app.UseAuthentication();                            // Enable authentication
app.UseAuthorization();                             // Enable authorization

// MVC route configuration
app.MapControllerRoute(                             // Define default route
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");       // Default to Movies controller and Index action

// API route configuration
app.MapControllers();                              // Map API controllers

app.Run();                                          // Run the application
