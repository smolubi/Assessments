using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBDC.Core.Interfaces;
using OBDC.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace OBDC.Infrastructure.Data
{
    public class PlayerRepository(AppDbContext context) : IPlayerRepository
    {
        private readonly AppDbContext _dbcontext = context;

        public async Task<PlayerAccount> GetPlayerAccountAsync(Guid accountId) => await _dbcontext.PlayerAccounts.FindAsync(accountId);

        public async Task CreatePlayerAccountAsync(PlayerAccount playerAccount)
        {
            await _dbcontext.PlayerAccounts.AddAsync(playerAccount);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task CreateCasinoWagerAsync(CasinoWager casinoWager)
        {
            await _dbcontext.CasinoWagers.AddAsync(casinoWager);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<(IEnumerable<CasinoWager> Wagers, int TotalCount)> GetPlayerCasinoWagersAsync(Guid playerId, int page, int pageSize)
        {
            var query = _dbcontext.CasinoWagers
                .Where(w => w.AccountId == playerId)
                .OrderByDescending(w => w.CreatedDateTime);

            var totalCount = await query.CountAsync();
            var wagers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (wagers, totalCount);
        }

        public async Task<IEnumerable<(Guid AccountId, string Username, decimal TotalAmountSpent)>> GetTopSpendersAsync(int count)
        {
            var result = await _dbcontext.CasinoWagers
                .GroupBy(w => new { w.AccountId, w.Username })
                .Select(g => new
                {
                    g.Key.AccountId,
                    g.Key.Username,
                    TotalAmountSpent = g.Sum(w => w.Amount)
                })
                .OrderByDescending(g => g.TotalAmountSpent)
                .Take(count)
                .ToListAsync();

            return result.Select(g => (g.AccountId, g.Username, g.TotalAmountSpent));
        }
    }
}
