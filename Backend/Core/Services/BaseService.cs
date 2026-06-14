﻿using Core.Interfaces.Repository;

namespace Core.Services
{
    /// <summary>
    /// Lớp Base Service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// Created by Phuong 24/05/2026
    public abstract class BaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;

        protected BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Lấy object T theo id
        /// </summary>
        /// <param name="id">ID của đối tượng cần lấy.</param>
        /// <returns>Đối tượng T nếu tìm thấy, ngược lại là null.</returns>
        /// Created by Phuong 24/05/2026
        public virtual Task<T?> GetByIdAsync(Guid id)
            => _repository.GetByIdAsync(id);

        /// <summary>
        /// Lấy tất cả object T
        /// </summary>
        /// <returns>Danh sách tất cả các đối tượng T.</returns>
        /// Created by Phuong 24/05/2026
        public virtual Task<IEnumerable<T>> GetAllAsync()
            => _repository.GetAllAsync();

        /// <summary>
        /// Tạo object T mới
        /// </summary>
        /// <param name="entity">Đối tượng T cần tạo.</param>
        /// <returns>ID của đối tượng T vừa được tạo.</returns>
        /// Created by Phuong 24/05/2026
        public virtual Task<Guid> CreateAsync(T entity)
            => _repository.CreateAsync(entity);

        /// <summary>
        /// Sửa object T
        /// </summary>
        /// <param name="entity">Đối tượng T cần cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        /// Created by Phuong 24/05/2026
        public virtual Task<bool> UpdateAsync(T entity)
            => _repository.UpdateAsync(entity);

        /// <summary>
        /// Xóa object T
        /// </summary>
        /// <param name="id">ID của đối tượng T cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        /// Created by Phuong 24/05/2026
        public virtual Task<bool> DeleteAsync(Guid id)
            => _repository.DeleteAsync(id);
    }
}
