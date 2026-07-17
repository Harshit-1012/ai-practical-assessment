using TicketSystem.Tests.Helpers;

namespace TicketSystem.Tests.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase
{
    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        ResetAndAuthenticateAsync().GetAwaiter().GetResult();
    }

    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    private async Task ResetAndAuthenticateAsync()
    {
        await Factory.ResetDatabaseAsync();
        await AuthTestHelper.AuthenticateAsUserAsync(Client);
    }
}
