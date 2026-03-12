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
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _repository;

    public CustomersController(IRepository<Customer> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Получить список клиентов
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CustomerResponse>> GetCustomers()
    {
        var customers = await _repository.GetAllAsync();

        return customers.Select(MapCustomer);
    }

    /// <summary>
    /// Получить клиента по Id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetCustomer(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        return MapCustomer(customer);
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(Customer customer)
    {
        await _repository.AddAsync(customer);

        return Ok(customer);
    }

    /// <summary>
    /// Редактировать клиента
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, Customer updatedCustomer)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        customer.FirstName = updatedCustomer.FirstName;
        customer.LastName = updatedCustomer.LastName;
        customer.Email = updatedCustomer.Email;

        // Можно обновить Preferences через CustomerPreferences, если нужно

        await _repository.UpdateAsync(customer);
        return Ok(MapCustomer(customer));
    }
    
    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();
        
        await _repository.DeleteAsync(customer);
        return Ok();
    }

    private CustomerResponse MapCustomer(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Preferences = customer.CustomerPreferences
                .Select(x => new PreferenceResponse
                {
                    Id = x.Preference.Id,
                    Name = x.Preference.Name
                }).ToList()
        };
    }
}