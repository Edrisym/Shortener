using Microsoft.EntityFrameworkCore;
using blink.Persistence;
using Testcontainers.MongoDb;

namespace Tests;

public class BaseFixture : IAsyncLifetime
{
    private MongoDbContainer _mongoContainer;
    private ShortenerDbContext _dbContext;

    public async Task InitializeAsync()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("docker.arvancloud.ir/mongo:latest")
            .WithCleanUp(true)
            .Build();
        await _mongoContainer.StartAsync();
    }

    public ShortenerDbContext CreateDatabase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShortenerDbContext>();
        var connectionString = _mongoContainer.GetConnectionString();
        optionsBuilder.UseMongoDB(connectionString, "Test_Db");

        return new ShortenerDbContext(optionsBuilder.Options);
    }

    public async Task DisposeAsync()
    {
        if (_mongoContainer != null)
        {
            await _mongoContainer.DisposeAsync();
        }

        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
    }
}