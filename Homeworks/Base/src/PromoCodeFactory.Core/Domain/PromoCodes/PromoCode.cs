using System;
using PromoCodeFactory.Core.Domain.Customers;
using PromoCodeFactory.Core.Domain.Partners;

namespace PromoCodeFactory.Core.Domain.PromoCodes;

public class PromoCode : BaseEntity
{
    public string Code { get; set; }

    public string ServiceInfo { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime EndDate { get; set; }

    public Guid PreferenceId { get; set; }
    
    public Preference Preference { get; set; }

    public Guid? CustomerId { get; set; }
   
    public Customer Customer { get; set; }

    public Guid PartnerId { get; set; }
    
    public Partner Partner { get; set; }
}