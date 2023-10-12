using Amazon;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class SqsMessageHandler : IMessageQueueHandler
{
    private readonly ILogger<IMessageQueueHandler> _logger;
    private readonly IMessageProcessor _messageProcessor;
    private readonly string _sqsUrl;
    private readonly int _defaultNumberOfMessagesToGet = 10;
    private readonly int _defaultWaitTimeInSeconds = 10;

    public SqsMessageHandler(ILogger<IMessageQueueHandler> logger, IMessageProcessor messageProcessor)
    {
        _logger = logger;
        _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQS_URL")))
        {
            _sqsUrl = Environment.GetEnvironmentVariable("SQS_URL");
        }

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NUMBER_OF_MESSAGES")))
        {
            int.TryParse(Environment.GetEnvironmentVariable("NUMBER_OF_MESSAGES"), out _defaultNumberOfMessagesToGet);
        }


        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAIT_TIME_IN_SECONDS")))
        {
            int.TryParse(Environment.GetEnvironmentVariable("WAIT_TIME_IN_SECONDS"), out _defaultWaitTimeInSeconds);
        }
    }

    public async Task HandleMessagesAsync(int? count)
    {
        try
        {
            _logger.LogInformation("Start processing...");
            var amazonSQSConfig = new AmazonSQSConfig()
            {
                ServiceURL = _sqsUrl,
                RegionEndpoint = RegionEndpoint.USEast1
            };

#if (DEBUG)
            using (var sqsClient = new AmazonSQSClient(amazonSQSConfig))
#elif (RELEASE)
            var credentials = AssumeRoleWithWebIdentityCredentials.FromEnvironmentVariables();
            using (var sqsClient = new amazonSQSConfig(credentials, amazonSQSConfig))
#endif


            // _logger.LogInformation($"credentials {JsonSerializer.Serialize(sqsClient)}");
            // using (var sqsClient = new AmazonSQSClient(credentials, amazonSQSConfig))
            {
                _logger.LogInformation($"Job {JsonSerializer.Serialize(sqsClient)}");
                ReceiveMessageResponse messageResponse;
                while ((messageResponse = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest{
                    QueueUrl = _sqsUrl,
                    MaxNumberOfMessages = _defaultNumberOfMessagesToGet,
                    WaitTimeSeconds = _defaultWaitTimeInSeconds
                })) != null)
                {
                    await HandleResponse(sqsClient, messageResponse);
                }
            }
            _logger.LogInformation($"Job ended successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Job ended with exception: {ex}");
        }
    }

    private async Task HandleResponse(AmazonSQSClient sqsClient, ReceiveMessageResponse messageResponse)
    {
        foreach (var message in messageResponse.Messages)
        {
            var result =  _messageProcessor.Process(message.Body);
            if (result == ProcessingStatus.Success)
            {
                await sqsClient.DeleteMessageAsync(_sqsUrl, message.ReceiptHandle);
            }
            _logger.LogInformation($"Message: {message.Body}");
        }
    }
}