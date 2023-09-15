using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/**
 *  Work queues
 *  參考 https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html
 **/

IConfiguration config = new ConfigurationBuilder()
		   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		   .Build();

//初始化連線資訊

//hostName
string hostName = "127.0.0.1";
var _hostName = config["ConnectionSettings:HostName"];
if (_hostName != null)
	hostName = _hostName;

var factory = new ConnectionFactory { HostName = hostName };

//設定 RabbitMQ port（可略過）
int port = 5672;
var _port = config["ConnectionSettings:port"];
if (_port != null)
	int.TryParse(_port, out port);

factory.Port = port;

//設定連線 RabbitMQ username
string userName = "";
var _userName = config["ConnectionSettings:UserName"];
if (_userName != null)
	userName = _userName;

factory.UserName = userName;

//設定 RabbitMQ password
string password = "";
var _password = config["ConnectionSettings:Password"];
if (_password != null)
	password = _password;

factory.Password = password;

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