using Common;
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
        CreateQueue();

        int count = 1;
        while(count < 30)
        {
            var salon = new Salon { Id = count, Name = $"Salon {count}" };
            count++;
            Thread.Sleep(1000);
            SendMessage(salon);
        }
        
        Console.ReadLine();
    }

    private static void CreateQueue()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(QueueName, true, false, false, null);
    }

    private static void SendMessage(Salon message)
    {
        _channel.BasicPublish("", QueueName, null, message.Serialize());
        Console.WriteLine($" [x] Salon Message Sent : {message.Id} - {message.Name}");
    }
}