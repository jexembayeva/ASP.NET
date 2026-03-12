using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Customers
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<CustomerPreference> CustomerPreferences { get; set; } = new List<CustomerPreference>();

        public ICollection<PromoCode.PromoCode> PromoCodes { get; set; } = new List<PromoCode.PromoCode>();
    }
}