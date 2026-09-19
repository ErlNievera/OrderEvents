using System.Text.Json;
using RabbitMQ.Client;
using Contracts;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "order.placed",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var order = new OrderPlaced(
    Guid.NewGuid(),
    "STUDENT-001",
    1250.50m,
    DateTime.UtcNow);

var body = JsonSerializer.SerializeToUtf8Bytes(order);

var properties = new BasicProperties
{
    Persistent = true,
    MessageId = order.OrderId.ToString()
};

await channel.BasicPublishAsync(
    exchange: "",
    routingKey: "order.placed",
    mandatory: true,
    basicProperties: properties,
    body: body);

Console.WriteLine($"Published OrderPlaced {order.OrderId}");
