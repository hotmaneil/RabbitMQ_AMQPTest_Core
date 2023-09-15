using System.Text;
using NewTask;
using RabbitMQ.Client;

/**
 *  Work queues
 *  參考 https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html
 **/

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
					 durable: true,
					 exclusive: false,
					 autoDelete: false,
					 arguments: null);

var message = Manager.GetMessage(args);

var body = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

channel.BasicPublish(exchange: string.Empty,
					 routingKey: "hello",
					 basicProperties: properties,
					 body: body);

Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");

Console.ReadLine();