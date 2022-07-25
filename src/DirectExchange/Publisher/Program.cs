using Common;
using RabbitMQ.Client;

class Program
{
    private static ConnectionFactory _factory;
    private static IConnection _connection;
    private static IModel _channel;
    private const string QueueName = "SalonQueue";

    public static void Main()
    {
        CreateQueue();

        int count = 1;
        while (count <= 30)
        {
            var salon = new Salon { Id = count, Name = $"Salon {count}" };
            if(salon.Id % 2 == 0)
            {
                SendMessageWithEvenId(salon);
            }
            else
            {
                SendMessageWithOddId(salon);
            }
            count++;
            //Thread.Sleep(1000);
            
        }

        Console.ReadLine();
    }

    private static void CreateQueue()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("direct-exchange", ExchangeType.Direct);

    }

    private static void SendMessageWithEvenId(Salon message)
    {
        _channel.BasicPublish("direct-exchange", "id-even", null, message.Serialize());
        Console.WriteLine($" [x] Salon Message Sent to queue with routing key \"id-even\" : {message.Id} - {message.Name}");
    }

    private static void SendMessageWithOddId(Salon message)
    {
        _channel.BasicPublish("direct-exchange", "id-odd", null, message.Serialize());
        Console.WriteLine($" [x] Salon Message Sent to queue with routing key \"id-odd\" : {message.Id} - {message.Name}");
    }
}