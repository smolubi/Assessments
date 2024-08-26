using Microsoft.AspNetCore.Connections;
using OBDC.Core.Models;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using OBDC.API.Configurations;

namespace OBDC.API.Services.Impl
{
    public class RabbitMQService(IOptions<RabbitMQSettings> options) : IRabbitMQService
    {
        private readonly RabbitMQSettings _settings = options.Value;

        public async Task PublishCasinoWagerAsync(CasinoWager casinoWager)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Hostname,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: _settings.QueueName,
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                var message = JsonConvert.SerializeObject(casinoWager);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: _settings.QueueName,
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            await Task.CompletedTask;
        }

    }
}
