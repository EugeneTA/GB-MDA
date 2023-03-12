using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    /// <summary>
    /// Отправитель сообщений в очередь
    /// </summary>
    public class Producer : IDisposable
    {
        private readonly string _queueName;
        private readonly string _hostName;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="queueName">имя очереди</param>
        /// <param name="hostName">имя хоста</param>
        /// <param name="virtualHost">имя виртуального хоста</param>
        /// <param name="userName">логин</param>
        /// <param name="password">пароль</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Producer(string queueName, string hostName, string virtualHost, string userName, string password)
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
        /// Метод отправки сообщения в очередь
        /// </summary>
        /// <param name="exchangeName">название биржи</param>
        /// <param name="exchangeType">тип биржи</param>
        /// <param name="message">сообщение</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Send(string exchangeName, string exchangeType, string message)
        {
            if (string.IsNullOrEmpty(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrEmpty(exchangeType)) throw new ArgumentNullException(nameof(exchangeType));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if (ExchangeType.All().Contains(exchangeType) == false) throw new ArgumentException($"No such Exchange Type {exchangeType}");

            var body = Encoding.UTF8.GetBytes(message);

            // Declare Exchange
            //_channel?.ExchangeDeclare(exchangeName, exchangeType, false, false, null);

            // Starting message consume
            //_channel.BasicPublish(exchangeName, _queueName, null, body);

            //Publish/Subscriber
            _channel?.ExchangeDeclare(exchangeName, exchangeType);
            _channel.BasicPublish(exchangeName, String.Empty, null, body);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}
