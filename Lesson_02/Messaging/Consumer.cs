using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Messaging
{
    /// <summary>
    /// Потребитель из очереди сообщений
    /// </summary>
    public class Consumer : IDisposable
    {
        private readonly string _queueName;
        private readonly string _hostName;

        private readonly IConnection _connection;
        private readonly IModel _channel;


        /// <summary>
        //  Конструктор
        /// </summary>
        /// <param name="queueName">название очереди</param>
        /// <param name="hostName">название хоста</param>
        /// <param name="virtualHost"></param>
        /// <param name="userName">логин</param>
        /// <param name="password">парль</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Consumer(string queueName, string hostName, string virtualHost, string userName, string password)
        {
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _hostName = hostName ?? throw new ArgumentNullException(nameof(hostName));
            _ = virtualHost ?? throw new ArgumentNullException(nameof(virtualHost));
            _ = userName ?? throw new ArgumentNullException(nameof(userName));
            _ = password ?? throw new ArgumentNullException(nameof(password));

            // создаем подключение
            _connection = new ConnectionFactory()
            {
                HostName = _hostName,
                Port = 5672,
                UserName = userName,
                Password = password,
                VirtualHost = virtualHost
            }.CreateConnection();

            _channel = _connection.CreateModel();
        }


        /// <summary>
        /// Метод получения сообщения из очереди
        /// </summary>
        /// <param name="exchangeName">название биржи</param>
        /// <param name="exchangeType">тип биржи</param>
        /// <param name="recieveCallback">метод для обработки сообщений</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Recieve(string exchangeName, string exchangeType, EventHandler<BasicDeliverEventArgs> recieveCallback)
        {
            if (string.IsNullOrEmpty(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrEmpty(exchangeType)) throw new ArgumentNullException(nameof(exchangeType));
            if (ExchangeType.All().Contains(exchangeType) == false) throw new ArgumentException($"No such Exchange Type {exchangeType}");
            if (recieveCallback == null) throw new ArgumentException($"No callback defined");

            // Declare Exchange
            _channel?.ExchangeDeclare(exchangeName, exchangeType);

            // Declare Queue
            //_channel?.QueueDeclare(_queueName, false, false, false, null);
            // Bind
            //_channel?.QueueBind(_queueName, exchangeName, _queueName);

            // Bind. Publisher/Subscriber
            var queueName = _channel.QueueDeclare().QueueName;
            _channel?.QueueBind(queueName, exchangeName, String.Empty);

            // Create consumer for the channel
            var consumer = new EventingBasicConsumer(_channel);

            if (consumer == null) throw new InvalidOperationException("Can't create EventingBasicConsumer");

            // Add message recieve callback
            consumer.Received += recieveCallback;

            // Starting message consume
            //_channel.BasicConsume(_queueName, true, consumer);

            // Starting message consume Publish/Subscriber
            _channel.BasicConsume(queueName, true, consumer);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}
