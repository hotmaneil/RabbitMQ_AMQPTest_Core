using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/**
 *  Work queues
 *  參考 https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html
 **/

//初始化連線資訊
var factory = new ConnectionFactory { HostName = "localhost" };

//設定 RabbitMQ port（可略過）
//factory.Port = 5672;

//開啟連線
using var connection = factory.CreateConnection();

//開啟 channel
using var channel = connection.CreateModel();

//宣告 queues
channel.QueueDeclare(queue: "hello",
					 durable: true,
					 exclusive: false,
					 autoDelete: false,
					 arguments: null);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine(" [*] Waiting for messages.");

//建立 consumer
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
	var body = ea.Body.ToArray();
	var message = Encoding.UTF8.GetString(body);
	Console.WriteLine($" [x] Received {message}");

	int dots = message.Split('.').Length - 1;
	Thread.Sleep(dots * 1000);

	Console.WriteLine(" [x] Done");

	// here channel could also be accessed as ((EventingBasicConsumer)sender).Model
	channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

channel.BasicConsume(queue: "hello",
					 autoAck: false,
					 consumer: consumer);


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();