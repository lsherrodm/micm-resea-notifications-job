using System.Net;

public interface IMessageQueueHandler
{
    Task HandleMessagesAsync(int? count);
}