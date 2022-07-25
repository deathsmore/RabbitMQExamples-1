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
        while (count <= 10)
        {
            var salon = new Salon { Id = count, Name = $"Salon {count}" };
            SendNotifyToUser1(salon);
            //if (salon.Id % 2 == 0)
            //{
            //    SendNotifyToUser1(salon);
            //}
            //else
            //{
            //    SendNotifyToUser2(salon);
            //}
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

        _channel.ExchangeDeclare("fanout-exchange", ExchangeType.Fanout, true, false, null);

        //_channel.QueueDeclare("user-1", true, false, false, null);
        //_channel.QueueBind("user-1", "fanout-exchange", "to-user-1");

        //_channel.QueueDeclare("user-2", true, false, false, null);
        //_channel.QueueBind("user-2", "fanout-exchange", "to-user-2");
    }

    private static void SendNotifyToUser1(Salon message)
    {
        _channel.BasicPublish("fanout-exchange", "to-user-1", null, message.Serialize());
        Console.WriteLine($" [x] Notify sent to queue with routing key \"to-user-1\" : {message.Id} - {message.Name}");
    }

    private static void SendNotifyToUser2(Salon message)
    {
        _channel.BasicPublish("fanout-exchange", "to-user-2", null, message.Serialize());
        Console.WriteLine($" [x] Notify sent to queue with routing key \"to-user-2\" : {message.Id} - {message.Name}");
    }
}