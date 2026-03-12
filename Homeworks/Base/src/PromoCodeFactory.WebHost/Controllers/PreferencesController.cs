using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Customers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly IRepository<Preference> _repository;

    public PreferencesController(IRepository<Preference> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Получить предпочтение по Id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PreferenceResponse>> GetPreference(Guid id)
    {
        var preference = await _repository.GetByIdAsync(id);
        if (preference == null)
            return NotFound();

        return new PreferenceResponse
        {
            Id = preference.Id,
            Name = preference.Name
        };
    }
}