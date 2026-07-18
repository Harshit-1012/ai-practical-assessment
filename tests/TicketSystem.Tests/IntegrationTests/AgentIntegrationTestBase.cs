using TicketSystem.Tests.Helpers;

namespace TicketSystem.Tests.IntegrationTests;

public abstract class AgentIntegrationTestBase : IntegrationTestBase
{
    protected AgentIntegrationTestBase(CustomWebApplicationFactory factory)
        : base(factory)
    {
        AuthTestHelper.AuthenticateAsAgentAsync(Client).GetAwaiter().GetResult();
    }
}
