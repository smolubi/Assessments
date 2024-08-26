using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OBDC.Core.Interfaces;
using OBDC.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OBDC.API.Configurations;

namespace OBDC.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;

        public Worker(ILogger<Worker> logger, IPlayerRepository playerRepository, IOptions<RabbitMQSettings> options)
        {
            _logger = logger;
            _playerRepository = playerRepository;

            var settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = settings.Hostname,
                UserName = settings.Username,
                Password = settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = settings.QueueName;

            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var casinoWager = JsonConvert.DeserializeObject<CasinoWager>(message);

                if (casinoWager != null)
                {
                    await ProcessCasinoWager(casinoWager);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _queueName,
                                  autoAck: false,
                                  consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessCasinoWager(CasinoWager casinoWager)
        {
            var existingPlayer = await _playerRepository.GetPlayerAccountAsync(casinoWager.AccountId);
            if (existingPlayer == null)
            {
                await _playerRepository.CreatePlayerAccountAsync(new PlayerAccount
                {
                    AccountId = casinoWager.AccountId,
                    Username = casinoWager.Username
                });
            }

            await _playerRepository.CreateCasinoWagerAsync(casinoWager);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            await base.StopAsync(cancellationToken);
        }
    }

}
