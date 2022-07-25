
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

        _channel.QueueDeclare(queue: "square_numb_req", durable: false, exclusive: true, autoDelete: false, arguments: null);
        _channel.BasicQos(0, 1, false);


        //Consume for queue "odd-queue"
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var props = eventArgs.BasicProperties;

            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            var message = Encoding.UTF8.GetString(body);
            int n = int.Parse(message);
            Console.WriteLine($" [x] Number: {n}");

            var result = n * n;

            _channel.BasicPublish("", props.ReplyTo, replyProps, Encoding.UTF8.GetBytes(result.ToString()));
            _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume("square_numb_req", false, consumer);
    }
}