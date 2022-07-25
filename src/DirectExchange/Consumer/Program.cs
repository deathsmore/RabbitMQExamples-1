
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    private static ConnectionFactory _factory;
    private static IConnection _connection;
    private static IModel _channel;

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


        _channel.ExchangeDeclare("direct-exchange", ExchangeType.Direct);

        _channel.QueueDeclare("even-queue", true, false, false);
        _channel.QueueDeclare("odd-queue", true, false, false);

        _channel.QueueBind("even-queue", "direct-exchange", "id-even");
        _channel.QueueBind("odd-queue", "direct-exchange", "id-odd");

        //Consume for queue "even-queue"
        //var consumerEven = new EventingBasicConsumer(_channel);
        //consumerEven.Received += (model, eventArgs) =>
        //{
        //    var body = eventArgs.Body.ToArray();
        //    var message = Encoding.UTF8.GetString(body);
        //    Console.WriteLine($" [x] Message received from queue \"even-queue\" : {message}");
        //};

        //_channel.BasicConsume("even-queue", true, consumerEven);

        //Consume for queue "odd-queue"
        var consumerOdd = new EventingBasicConsumer(_channel);
        consumerOdd.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Message received from queue \"odd-queue\" : {message}");
        };

        _channel.BasicConsume("odd-queue", true, consumerOdd);
    }
}