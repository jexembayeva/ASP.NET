using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Partners;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnersController : ControllerBase
    {
        private readonly IRepository<Partner> _partnerRepository;
        private readonly IRepository<PartnerLimit> _limitRepository;

        public PartnersController(IRepository<Partner> partnerRepository,
                                  IRepository<PartnerLimit> limitRepository)
        {
            _partnerRepository = partnerRepository;
            _limitRepository = limitRepository;
        }

        /// <summary>
        /// Установить лимит промокодов партнеру
        /// </summary>
        [HttpPost("{partnerId}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid partnerId, SetPartnerLimitDto dto)
        {
            if (dto.MaxPromoCodes <= 0)
                return BadRequest("Limit must be greater than 0");

            var partner = await _partnerRepository.GetByIdAsync(partnerId);
            if (partner == null)
                return NotFound();

            if (!partner.IsActive)
                return BadRequest("Partner is inactive");

            
            if (partner.PartnerLimits != null)
            {
                foreach (var oldLimit in partner.PartnerLimits)
                {
                    if (oldLimit.EndDate == null)
                    {
                        oldLimit.EndDate = DateTime.UtcNow;
                    }
                }
            }
            
            // Отключаем предыдущие активные лимиты
            var activeLimits = partner.PartnerLimits
                .Where(l => l.CancelDate == null && (l.EndDate == null || l.EndDate > DateTime.UtcNow))
                .ToList();

            foreach (var limit in activeLimits)
            {
                limit.CancelDate = DateTime.UtcNow;
                await _limitRepository.UpdateAsync(limit);
            }

            // Если лимит закончился, NumberIssuedPromoCodes не обнуляется
            if (!activeLimits.Any(l => l.EndDate.HasValue && l.EndDate > DateTime.UtcNow))
            {
                partner.NumberIssuedPromoCodes = 0;
            }

            // Создаем новый лимит
            var newLimit = new PartnerLimit
            {
                PartnerId = partner.Id,
                MaxPromoCodes = dto.MaxPromoCodes,
                CreatedDate = DateTime.UtcNow
            };

            await _limitRepository.AddAsync(newLimit);
            await _partnerRepository.UpdateAsync(partner);

            return Ok(newLimit);
        }
    }
}