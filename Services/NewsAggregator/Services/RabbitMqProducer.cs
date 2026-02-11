using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace NewsAggregator.Services;

public class RabbitMqProducer
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqProducer(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
        };
        // Sync over async for constructor (not ideal but simple for this demo)
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(queue: "news.raw",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null).GetAwaiter().GetResult();
    }

    public async Task PublishNewsAsync(object message)
    {
        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(exchange: "",
                             routingKey: "news.raw",
                             body: body);
    }
}
