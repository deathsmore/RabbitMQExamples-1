using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    private static ConnectionFactory _factory;
    private static IConnection _connection;
    private static IModel _channel;
    private const string QueueName = "SalonQueue";
    //private static bool endApp = false;
    public static void Main()
    {
        
        Console.WriteLine("Publisher started");

        Console.Write("Type a number, and then press Enter: ");
        int num = int.Parse(Console.ReadLine());

        Publish(num);
        Console.ReadLine();
    }

    private static void Publish(int num)
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        //Sent request
        var replyQueueName = _channel.QueueDeclare();
        var correlationId = Guid.NewGuid().ToString();



        var messageProperty = _channel.CreateBasicProperties();
        messageProperty.ReplyTo = replyQueueName.QueueName;
        messageProperty.CorrelationId = correlationId;

        _channel.BasicPublish("", "square_numb_req", messageProperty, Encoding.UTF8.GetBytes(num.ToString()));

        //Receive response from server

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                Console.WriteLine($"The square of {num} is {response}");
            }



            //Thread.Sleep(5000);
            //Console.Write("Press 'x' and Enter to close the app, or press any other key and Enter to continue: ");
            //if (Console.ReadLine() == "x") endApp = true;
            //Console.WriteLine("\n");
        };

        _channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true);
    }
}