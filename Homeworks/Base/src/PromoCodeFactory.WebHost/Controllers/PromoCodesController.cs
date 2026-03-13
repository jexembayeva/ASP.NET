using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Customers;
using PromoCodeFactory.Core.Domain.PromoCodes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromoCodesController : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;

        public PromoCodesController(
            IRepository<PromoCode> promoRepository,
            IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository)
        {
            _promoRepository = promoRepository;
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Выдать новый промокод клиентам с указанным предпочтением
        /// </summary>
        [HttpPost("give")]
        public async Task<IActionResult> GivePromocodesToCustomersWithPreferenceAsync(Guid preferenceId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Promo code cannot be empty");

            var preference = await _preferenceRepository.GetByIdAsync(preferenceId);
            if (preference == null)
                return NotFound("Preference not found");

            var customers = (await _customerRepository.GetAllAsync())
                .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == preferenceId))
                .ToList();

            foreach (var customer in customers)
            {
                var promo = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    BeginDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    PreferenceId = preferenceId,
                    CustomerId = customer.Id
                };

                await _promoRepository.AddAsync(promo);
            }

            return Ok($"Promo code '{code}' issued to {customers.Count} customers.");
        }

        /// <summary>
        /// Получить промокоды в диапазоне дат
        /// </summary>
        [HttpGet("get")]
        public async Task<IActionResult> GetPromocodesAsync(string fromDate, string toDate)
        {
            if (!DateTime.TryParse(fromDate, out var from) ||
                !DateTime.TryParse(toDate, out var to))
            {
                return BadRequest("Invalid date format");
            }

            var promocodes = (await _promoRepository.GetAllAsync())
                .Where(p => p.BeginDate >= from && p.BeginDate <= to)
                .Select(p => new
                {
                    p.Id,
                    p.Code,
                    p.BeginDate,
                    p.EndDate,
                    CustomerName = p.Customer != null
                        ? $"{p.Customer.FirstName} {p.Customer.LastName}"
                        : null,
                    PreferenceName = p.Preference != null
                        ? p.Preference.Name
                        : null
                })
                .ToList();

            return Ok(promocodes);
        }
    }
}