using Microsoft.Extensions.Configuration;
using NewTask;
using RabbitMQ.Client;
using System.Text;

/**
 *  Work queues
 *  參考 https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html
 **/

IConfiguration config = new ConfigurationBuilder()
		   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		   .Build();

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
if(_userName!=null)
	userName = _userName;

factory.UserName = userName;

//設定 RabbitMQ password
string password = "";
var _password = config["ConnectionSettings:Password"];
if(_password!=null)
	password = _password;

factory.Password = password;

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