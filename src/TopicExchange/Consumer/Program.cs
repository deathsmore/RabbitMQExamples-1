using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    private static ConnectionFactory _factory;
    private static IConnection _connection;
    private static IModel _channel;
    private const string QueueName = "SalonQueue";

    public static void Main()
    {
        Console.WriteLine("Consumer started");
        Recieve();

        Console.ReadLine();
    }
    public static void Recieve()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        //_channel.ExchangeDeclare("topic-exchange", ExchangeType.Topic, true, false, null);

        //_channel.QueueDeclare("noti-all", true, false, false, null);
        //_channel.QueueBind("noti-all", "topic-exchange", "dvg.dept.*");

        //_channel.QueueDeclare("noti-it", true, false, false, null);
        //_channel.QueueBind("noti-it", "topic-exchange", "dvg.dept.it");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };

        _channel.BasicConsume(QueueName, true, consumer);
    }
}