using System;

namespace PromoCodeFactory.Core.Domain.Partners;

public class PartnerLimit : BaseEntity
{
    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; }

    public int MaxPromoCodes { get; set; } // Лимит промокодов
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; } // Дата окончания лимита
    public DateTime? CancelDate { get; set; } // Дата отмены лимита
}