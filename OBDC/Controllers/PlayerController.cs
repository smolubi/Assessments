using Microsoft.AspNetCore.Mvc;
using OBDC.API.Services;
using OBDC.Common.API.Controllers;
using OBDC.Core.Interfaces;
using OBDC.Core.Models;

namespace OBDC.API.Controllers
{
    public class PlayerController(IPlayerRepository playerRepository, IRabbitMQService rabbitMQService) : ApiControllerBase
    {
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IRabbitMQService _rabbitMQService = rabbitMQService;

        [HttpPost]
        [Route(nameof(CasinoWager))]
        public async Task<IActionResult> CasinoWager([FromBody] CasinoWager casinoWager)
        {
            await _rabbitMQService.PublishCasinoWagerAsync(casinoWager);
            return Ok();
        }

        [HttpGet("{playerId}/casino")]
        public async Task<IActionResult> GetPlayerCasinoWagers(Guid playerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (wagers, totalCount) = await _playerRepository.GetPlayerCasinoWagersAsync(playerId, page, pageSize);

            var result = new
            {
                Data = wagers.Select(w => new
                {
                    w.WagerId,
                    Game = w.GameName,
                    w.Provider,
                    w.Amount,
                    CreatedDate = w.CreatedDateTime
                }),
                Page = page,
                PageSize = pageSize,
                Total = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }

        [HttpGet("topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count = 10)
        {
            var topSpenders = await _playerRepository.GetTopSpendersAsync(count);

            var result = topSpenders.Select(s => new
            {
                s.AccountId,
                s.Username,
                TotalAmountSpent = s.TotalAmountSpent
            });

            return Ok(result);
        }
    }
}
