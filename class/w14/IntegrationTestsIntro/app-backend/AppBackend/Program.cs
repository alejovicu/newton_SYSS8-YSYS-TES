using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Services
// ---------------------------

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // JWT Bearer scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid JWT token."
    });

    // Require JWT for all endpoints in Swagger UI
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc, null)
            {
                Reference = new OpenApiReferenceWithDescription
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                    HostDocument = doc
                }
            },
            new List<string>()
        }
    });
});

// ---------------------------
// JWT Authentication
// ---------------------------

var jwtSecret = "your-very-strong-secret-key-1234567890"; // replace in production
var jwtIssuer = "AppBackend";
var jwtAudience = "AppBackendUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ---------------------------
// Build App
// ---------------------------

var app = builder.Build();

// ---------------------------
// Middleware
// ---------------------------

// Swagger
app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/swagger.json");
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "docs";
    c.SwaggerEndpoint("/docs/v1/swagger.json", "AppBackend v1");
});

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run
app.Run();