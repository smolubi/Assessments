using OBDC.Core.Models;
using System.Threading.Tasks;

namespace OBDC.API.Services
{
    public interface IRabbitMQService
    {
        Task PublishCasinoWagerAsync(CasinoWager casinoWager);
    }
}
