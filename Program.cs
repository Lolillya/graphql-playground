using FirebaseAdmin;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using graphql_playground.DataLoaders;
using graphql_playground.GraphQL.Mutations;
using graphql_playground.GraphQL.Queries;
using graphql_playground.GraphQL.Subscriptions;
using graphql_playground.Services;
using graphql_playground.Services.Courses;
using graphql_playground.Services.Instructors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddInMemorySubscriptions()
    .AddAuthorization(o => o.AddPolicy("IsAdmin", p => p.RequireClaim("email", "someonesusernam@gmail.com")));

builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddFirebaseAuthentication();

var connectionString = configuration.GetConnectionString("Default");
builder.Services.AddPooledDbContextFactory<SchoolDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddDbContextPool<SchoolDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<CoursesRepository>();
builder.Services.AddScoped<InstructorsRepository>();
builder.Services.AddScoped<InstructorDataLoader>();

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
