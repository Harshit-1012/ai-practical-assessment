namespace TicketSystem.Tests.IntegrationTests;

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "Integration";
}
