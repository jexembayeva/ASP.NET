using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(T entity)
        {
            var list = Data.ToList();
            entity.Id = Guid.NewGuid();
            list.Add(entity);
            Data = list;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            var list = Data.ToList();
            var existing = list.FirstOrDefault(x => x.Id == entity.Id);
            if (existing != null)
            {
                var index = list.IndexOf(existing);
                list[index] = entity;
                Data = list;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            var list = Data.ToList();
            if (list.Contains(entity))
            {
                list.Remove(entity);
                Data = list;
            }
            return Task.CompletedTask;
        }
    }
}