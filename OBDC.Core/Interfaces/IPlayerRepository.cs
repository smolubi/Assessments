using OBDC.Core.Models;

namespace OBDC.Core.Interfaces
{
    public interface IPlayerRepository
    {
        Task<PlayerAccount> GetPlayerAccountAsync(Guid accountId);
        Task CreatePlayerAccountAsync(PlayerAccount playerAccount);
        Task CreateCasinoWagerAsync(CasinoWager casinoWager);
        Task<(IEnumerable<CasinoWager> Wagers, int TotalCount)> GetPlayerCasinoWagersAsync(Guid playerId, int page, int pageSize);
        Task<IEnumerable<(Guid AccountId, string Username, decimal TotalAmountSpent)>> GetTopSpendersAsync(int count);
    }
}
