using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    /// <summary>
    /// Interface cơ sở cho các Service CRUD
    /// </summary>
    /// <typeparam name="TDto">Loại DTO sử dụng cho cả Request và Response</typeparam>
    public interface IBaseCrudService<TDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(TDto dto);
        Task<bool> UpdateAsync(Guid id, TDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
