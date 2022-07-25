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

        var listProduct = new List<Product>()
        {
            new Product(){ Material = "wood", Color = "black"},
            new Product(){ Material = "steel", Color = "red"},
            new Product(){ Material = "aluminum", Color = "white"},
        };

        foreach(var product in listProduct)
        {
            Publish(product);
            //Thread.Sleep(1000);
        }
        Console.ReadLine();
    }

    private static void CreateQueue()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("header-exchange", ExchangeType.Headers, true, false, null);

        _channel.QueueDeclare("demo-header-queue-1", true, false, false, null);
        _channel.QueueBind("demo-header-queue-1", "header-exchange", string.Empty, new Dictionary<string, object>()
        {
            { "material", "wood"},
            { "color", "black"}
        });
    }

    private static void Publish(Product message)
    {
        var property = _channel.CreateBasicProperties();
        property.Headers = new Dictionary<string, object>()
        {
            { "material", message.Material},
            { "color", message.Color}
        };
        property.Persistent = true;
        _channel.BasicPublish("header-exchange", String.Empty, property, message.Serialize());
        Console.WriteLine($" [x] Notify sent to exchange \"header-exchange\" : {message.Material} - {message.Color}");
    }
}