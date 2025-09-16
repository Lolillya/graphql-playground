using FirebaseAdmin;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using graphql_playground.DataLoaders;
using graphql_playground.GraphQL.Mutations;
using graphql_playground.GraphQL.Queries;
using graphql_playground.GraphQL.Subscriptions;
using graphql_playground.Services;
using graphql_playground.Services.Courses;
using graphql_playground.Services.Instructors;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using graphql_playground.Validators;
using FluentValidation;
using AppAny.HotChocolate.FluentValidation;
using Google.Apis.Auth.OAuth2;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Services
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CourseTypeInputValidator>();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<CourseType>()
    .AddType<InstructorType>()
    .AddTypeExtension<CourseQuery>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddAuthorization()
    .AddFluentValidation(o =>
    {
        o.UseDefaultErrorMapper();
    })
    .AddInMemorySubscriptions()
    .AddAuthorization(o => o.AddPolicy("IsAdmin", p => p.RequireClaim("email", "someonesusernam@gmail.com")));

var firebaseCredPath = configuration.GetValue<string>("FIREBASE_AUTH_PATH")
    ?? Environment.GetEnvironmentVariable("FIREBASE_AUTH_PATH");
if (!string.IsNullOrWhiteSpace(firebaseCredPath) && File.Exists(firebaseCredPath))
{
    builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseCredPath)
    }));
    builder.Services.AddFirebaseAuthentication();
}
else
{
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Services.AddLogging();
    builder.Services.AddSingleton<FirebaseApp?>(sp => null);
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
    builder.Services.BuildServiceProvider()
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("graphql-playground")
        .LogWarning("FIREBASE_AUTH_PATH not set or file missing; skipping Firebase initialization.");
}

var connectionString = configuration.GetConnectionString("Default")
    ?? configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPooledDbContextFactory<SchoolDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddDbContextPool<SchoolDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<CoursesRepository>();
builder.Services.AddScoped<InstructorsRepository>();
builder.Services.AddScoped<InstructorDataLoader>();
builder.Services.AddScoped<UserDataLoader>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Disposition", "Content-Length");
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();

app.UseWebSockets();

app.MapControllers();

app.MapGraphQL("/graphql").RequireCors("AllowAll");
using (IServiceScope scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SchoolDbContext>>();
    using var dbContext = dbContextFactory.CreateDbContext();
    dbContext.Database.Migrate();
}


app.Run();
