using System.Text;
using System.Text.Json;
using FitLife.Membership.Api.IntegrationEvents;
using RabbitMQ.Client;

namespace FitLife.Membership.Api.Messaging;

public class RabbitMqMemberEventPublisher : IMemberEventPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqMemberEventPublisher> _logger;

    public RabbitMqMemberEventPublisher(
        IConfiguration configuration,
        ILogger<RabbitMqMemberEventPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishMemberCreatedAsync(MemberCreatedEvent memberCreatedEvent)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"]
                       ?? throw new InvalidOperationException("RabbitMQ host is missing"),
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"]
                       ?? throw new InvalidOperationException("RabbitMQ username is missing"),
            Password = _configuration["RabbitMQ:Password"]
                       ?? throw new InvalidOperationException("RabbitMQ password is missing")
        };

        const string queueName = "membership.member.created";

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(memberCreatedEvent));

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body);

        _logger.LogInformation(
            "Published MemberCreated event for MemberId {MemberId}",
            memberCreatedEvent.MemberId);
    }
}