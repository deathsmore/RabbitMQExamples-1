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
            if (salon.Id % 2 == 0)
            {
                SendNotifyToIT(salon);
            }
            else
            {
                SendNotifyToHR(salon);
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

        _channel.ExchangeDeclare("topic-exchange", ExchangeType.Topic, true, false, null);

        _channel.QueueDeclare("noti-all", true, false, false, null);
        _channel.QueueBind("noti-all", "topic-exchange", "dvg.dept.*");

        _channel.QueueDeclare("noti-it", true, false, false, null);
        _channel.QueueBind("noti-it", "topic-exchange", "dvg.dept.it");
    }

    private static void SendNotifyToIT(Salon message)
    {
        _channel.BasicPublish("topic-exchange", "dvg.dept.it", null, message.Serialize());
        Console.WriteLine($" [x] Notify sent to queue with routing key \"dvg.dept.it\" : {message.Id} - {message.Name}");
    }

    private static void SendNotifyToHR(Salon message)
    {
        _channel.BasicPublish("topic-exchange", "dvg.dept.hr", null, message.Serialize());
        Console.WriteLine($" [x] Notify sent to queue with routing key \"dvg.dept.hr\" : {message.Id} - {message.Name}");
    }
}