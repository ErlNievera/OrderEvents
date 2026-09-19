OrderEvents — RabbitMQ Event-Driven Order System
Overview

This project demonstrates a small event-driven order system using .NET 9 and RabbitMQ. A Producer publishes an OrderPlaced event to RabbitMQ, the order.placed queue stores the event, and a Consumer receives, processes, and manually acknowledges the event.

Requirements

.NET 9 SDK

Docker Desktop

RabbitMQ running in Docker

1. Start RabbitMQ

Open a terminal and run:

docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management


Verify that RabbitMQ is running:

docker ps


Open the RabbitMQ Management UI in a browser:

http://localhost:15672


Login:

Username: guest
Password: guest


Create a durable queue named:

order.placed

2. Build the Solution

From the solution directory:

dotnet build

3. Run the Consumer

Open Terminal 1:

cd C:\Users\COLLEGELAB-02\Consumer
dotnet run


The Consumer waits for OrderPlaced events and manually acknowledges successfully processed messages.

4. Run the Producer

Open Terminal 2:

cd C:\Users\COLLEGELAB-02\Producer
dotnet run


The Producer publishes one OrderPlaced event each time it is run.

Example output:

Published OrderPlaced <OrderId>


Run the Producer multiple times to publish multiple orders.

5. Observe Processing

The Consumer displays each processed event:

Processed OrderPlaced <OrderId> | Student: STUDENT-001 | Total: 1250.50


RabbitMQ should eventually show:

Ready: 0
Unacked: 0


after all messages have been acknowledged.

6. Stop/Restart Test

The Consumer uses manual acknowledgments with:

autoAck: false


A message is acknowledged only after successful processing.

During the stop/restart test, the Consumer was stopped while a message was being processed but before its acknowledgment was sent. RabbitMQ returned the unacknowledged message to the queue after the Consumer disconnected. When the Consumer was restarted, the message was delivered again and processed successfully. This demonstrated that an unacknowledged message was not lost and showed the at-least-once delivery behavior of RabbitMQ with manual acknowledgments.

Project Structure
OrderEvents/
├── Contracts/
│   ├── OrderPlaced.cs
│   └── Contracts.csproj
├── Producer/
│   ├── Program.cs
│   └── Producer.csproj
├── Consumer/
│   ├── Program.cs
│   └── Consumer.csproj
├── OrderEvents.sln
└── README.md
