using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OBDC.Core.Interfaces;
using OBDC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OBDC.API.Configurations;

namespace OBDC.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<RabbitMQSettings>(hostContext.Configuration.GetSection("RabbitMQSettings"));

                    services.AddHostedService<Worker>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddScoped<IPlayerRepository, PlayerRepository>();
                });
    }
}