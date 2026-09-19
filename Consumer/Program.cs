using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

await channel.BasicQosAsync(
    prefetchSize: 0,
    prefetchCount: 10,
    global: false);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (sender, ea) =>
{
    try
    {
        var order = JsonSerializer.Deserialize<OrderPlaced>(ea.Body.Span);

        if (order is null)
        {
            throw new InvalidOperationException("Could not deserialize OrderPlaced event.");
        }

        Console.WriteLine(
            $"Processing OrderPlaced {order.OrderId} | " +
            $"Student: {order.StudentId} | " +
            $"Total: {order.Total}");

        await Task.Delay(5000);

        Console.WriteLine($"Acknowledging OrderPlaced {order.OrderId}");

        await channel.BasicAckAsync(
            deliveryTag: ea.DeliveryTag,
            multiple: false);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing message: {ex.Message}");

        await channel.BasicNackAsync(
            deliveryTag: ea.DeliveryTag,
            multiple: false,
            requeue: false);
    }
};

await channel.BasicConsumeAsync(
    queue: "order.placed",
    autoAck: false,
    consumer: consumer);

Console.WriteLine("Consumer started. Waiting for OrderPlaced events...");
Console.WriteLine("Press Ctrl+C to stop.");

await Task.Delay(Timeout.Infinite);
