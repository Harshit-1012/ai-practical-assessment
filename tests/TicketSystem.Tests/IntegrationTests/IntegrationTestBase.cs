namespace TicketSystem.Tests.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    public async Task InitializeAsync() => await Factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
