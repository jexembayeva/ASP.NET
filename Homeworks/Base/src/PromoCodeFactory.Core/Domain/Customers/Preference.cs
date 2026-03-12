using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Customers;

public class Preference : BaseEntity
{
    public string Name { get; set; }

    public ICollection<CustomerPreference> CustomerPreferences { get; set; }
        = new List<CustomerPreference>();
}