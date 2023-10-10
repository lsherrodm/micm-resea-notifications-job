
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class ApplicationService
{
public readonly IMessageQueueHandler _handler;

    public ApplicationService(IMessageQueueHandler handler)
    {
        _handler = handler;
    }

public async Task Run(string[] args)
{
    await _handler.HandleMessagesAsync(10);
}
}