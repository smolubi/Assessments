namespace OBDC.API.Configurations
{
    public class RabbitMQSettings
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
