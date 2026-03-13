using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.Partners;

namespace PromoCodeFactory.WebHost.Tests.Controllers.Builders;

public class PartnerBuilder
{
    private Guid _id = Guid.NewGuid();
    private bool _isActive = true;
    private int _issuedPromoCodes = 0;

    public PartnerBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PartnerBuilder Active()
    {
        _isActive = true;
        return this;
    }

    public PartnerBuilder Inactive()
    {
        _isActive = false;
        return this;
    }

    public PartnerBuilder WithIssuedPromoCodes(int value)
    {
        _issuedPromoCodes = value;
        return this;
    }

    public Partner Build()
    {
        return new Partner
        {
            Id = _id,
            IsActive = _isActive,
            NumberIssuedPromoCodes = _issuedPromoCodes,
            PartnerLimits = new List<PartnerLimit>()
        };
    }
}