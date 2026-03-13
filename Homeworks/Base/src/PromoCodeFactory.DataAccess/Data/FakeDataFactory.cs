using System;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Customers;
using PromoCodeFactory.Core.Domain.Partners;
using PromoCodeFactory.Core.Domain.PromoCodes;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        // ------------------ Роли ------------------
        public static readonly Role AdminRole = new Role
        {
            Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
            Name = "Admin",
            Description = "Администратор"
        };

        public static readonly Role PartnerManagerRole = new Role
        {
            Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
            Name = "PartnerManager",
            Description = "Партнерский менеджер"
        };

        public static IList<Role> Roles => new List<Role> { AdminRole, PartnerManagerRole };

        // ------------------ Сотрудники ------------------
        public static IList<Employee> Employees => new List<Employee>
        {
            new Employee
            {
                Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                Email = "owner@somemail.ru",
                FirstName = "Иван",
                LastName = "Сергеев",
                Roles = new List<Role> { AdminRole },
                AppliedPromocodesCount = 5
            },
            new Employee
            {
                Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                Email = "andreev@somemail.ru",
                FirstName = "Петр",
                LastName = "Андреев",
                Roles = new List<Role> { PartnerManagerRole },
                AppliedPromocodesCount = 10
            }
        };

        // ------------------ Preferences ------------------
        public static readonly Preference SportsPreference = new Preference
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Sports"
        };

        public static readonly Preference MusicPreference = new Preference
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Music"
        };

        public static readonly Preference FoodPreference = new Preference
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Food"
        };

        public static IList<Preference> Preferences => new List<Preference>
        {
            SportsPreference, MusicPreference, FoodPreference
        };

        // ------------------ Customers ------------------
        public static IList<Customer> Customers => new List<Customer>
        {
            new Customer
            {
                Id = Guid.Parse("aaaaaaa1-1111-1111-1111-111111111111"),
                FirstName = "Гаухар",
                LastName = "Демир",
                Email = "gaukhar@example.com",
                CustomerPreferences = new List<CustomerPreference>
                {
                    new CustomerPreference { Preference = SportsPreference },
                    new CustomerPreference { Preference = MusicPreference }
                }
            },
            new Customer
            {
                Id = Guid.Parse("bbbbbbb2-2222-2222-2222-222222222222"),
                FirstName = "Алексей",
                LastName = "Петров",
                Email = "aleksey@example.com",
                CustomerPreferences = new List<CustomerPreference>
                {
                    new CustomerPreference { Preference = FoodPreference }
                }
            }
        };

        // ------------------ Промокоды ------------------
        public static IList<PromoCode> PromoCodes => new List<PromoCode>
        {
            new PromoCode
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Code = "PROMO10",
                ServiceInfo = "Discount 10%",
                BeginDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Preference = SportsPreference,
                Customer = Customers[0],
                Partner = DefaultPartner
            }
        };

        // ------------------ Partners ------------------
        public static readonly Partner DefaultPartner = new Partner
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            Name = "Amazon",
            IsActive = true,
            NumberIssuedPromoCodes = 0
        };

        public static IList<Partner> Partners => new List<Partner>
        {
            DefaultPartner
        };
        
        // ------------------ Метод Seed ------------------
        public static void Seed(PromoCodeFactoryDbContext context)
        {
            // Роли
            foreach (var role in Roles)
                if (!context.Roles.Any(r => r.Id == role.Id))
                    context.Roles.Add(role);

            context.SaveChanges();

            // Сотрудники
            foreach (var employee in Employees)
                if (!context.Employees.Any(e => e.Id == employee.Id))
                    context.Employees.Add(employee);

            context.SaveChanges();

            // Preferences
            foreach (var pref in Preferences)
                if (!context.Preferences.Any(p => p.Id == pref.Id))
                    context.Preferences.Add(pref);

            context.SaveChanges();

            // Customers
            foreach (var customer in Customers)
            {
                if (!context.Customers.Any(c => c.Id == customer.Id))
                {
                    // Привязка CustomerPreferences к существующим Preferences из контекста
                    foreach (var cp in customer.CustomerPreferences)
                    {
                        var existingPref = context.Preferences.Find(cp.Preference.Id);
                        cp.Preference = existingPref;
                        cp.PreferenceId = existingPref.Id;
                        cp.Customer = customer;
                    }

                    context.Customers.Add(customer);
                }
            }

            context.SaveChanges();

            // Промокоды
            foreach (var promo in PromoCodes)
            {
                if (!context.PromoCodes.Any(p => p.Id == promo.Id))
                {
                    promo.Preference = context.Preferences.Find(promo.Preference.Id);
                    promo.PreferenceId = promo.Preference.Id;
                    promo.Customer = context.Customers.Find(promo.Customer.Id);
                    promo.CustomerId = promo.Customer.Id;
                    promo.Partner = context.Partners.Find(promo.Partner.Id);
                    promo.PartnerId = promo.Partner.Id;

                    context.PromoCodes.Add(promo);
                }
            }

            context.SaveChanges();
            
            // Partners
            foreach (var partner in Partners)
                if (!context.Partners.Any(p => p.Id == partner.Id))
                    context.Partners.Add(partner);

            context.SaveChanges();
        }
    }
}