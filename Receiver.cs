using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

internal class Program
{
    private static void Main(string[] args)
    {
        //Create Factory - using RabbitMQDockerServer
        var factory = new ConnectionFactory() { HostName = "localhost" };
        //Create Connection
        using (var connection = factory.CreateConnection())
        {
            //Create channel to send/communicate
            using (var channel = connection.CreateModel())
            {
                //Create queue with Exchange if not exist or use it if exist 
                channel.ExchangeDeclare(exchange:"logs", type:ExchangeType.Fanout);

                //Create queue if not exist or use it if exist
                /*channel.QueueDeclare(queue: "hello", durable: false,
                    exclusive: false, autoDelete: false, arguments: null);*/

                //Getting queue name (random)
                var queueName = channel.QueueDeclare().QueueName;

                //Adding/Connecting 1 or N number of queues to the exchange channel
                channel.QueueBind(queue:queueName, exchange:"logs", routingKey:"");
                Console.WriteLine("waiting for logs..");
                
                //Listen to the Channel
                var consumer = new EventingBasicConsumer(channel);

                //Receive Event triggered
                consumer.Received += (model, ea) => {
                    //Getting body byte array
                    var body = ea.Body.ToArray();
                    //String encoding
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[x] Received {message}");
                };

                //Configuring auto knowledge from queue to consumer
                channel.BasicConsume(queue:queueName, autoAck:true, consumer:consumer);

                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }
    }
}