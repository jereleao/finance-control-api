using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTech.Domain.Entities;

namespace FinTech.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<IEnumerable<Currency>> GetAsync();
    Task<Currency?> GetByIdAsync(string id);
    Task<Currency> CreateAsync(Currency entity);
    Task<Currency> UpdateAsync(Currency entity);
    Task RemoveAsync(Currency entity);

}
