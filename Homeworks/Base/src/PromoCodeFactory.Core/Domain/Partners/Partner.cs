using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.PromoCodes;

namespace PromoCodeFactory.Core.Domain.Partners
{
    public class Partner : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        // Количество промокодов, которые партнер выдал
        public int NumberIssuedPromoCodes { get; set; }

        // Список лимитов
        public ICollection<PartnerLimit> PartnerLimits { get; set; } = new List<PartnerLimit>();

        public ICollection<PromoCode> PromoCodes { get; set; }
            = new List<PromoCode>();
    }
}