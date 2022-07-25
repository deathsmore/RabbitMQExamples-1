using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    private static ConnectionFactory _factory;
    private static IConnection _connection;
    private static IModel _channel;
    private const string QueueName = "demo-header-queue-1";

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

        _channel.ExchangeDeclare("fanout-exchange", ExchangeType.Fanout, true, false, null);


        var queue = _channel.QueueDeclare("", true, true);

        Console.WriteLine($"Queue name: {queue.QueueName}");
        _channel.QueueBind(queue.QueueName, "fanout-exchange", string.Empty);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };

        _channel.BasicConsume(queue.QueueName, true, consumer);
    }
}