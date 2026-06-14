﻿using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.Validators; // Import IAppValidator

namespace Core.Services
{
    /// <summary>
    /// Cung cấp logic CRUD cơ bản, chung cho các service đơn giản làm việc với DTO.
    /// </summary>
    /// <typeparam name="TEntity">Kiểu entity</typeparam>
    /// <typeparam name="TDto">Kiểu DTO</typeparam>
    /// <typeparam name="TRepository">Kiểu repository cụ thể</typeparam>
    /// Created by Phuong 26/02/2026
    public abstract class CrudService<TEntity, TDto, TRepository> : BaseService<TEntity>, IBaseCrudService<TDto>
        where TEntity : class
        where TDto : class
        where TRepository : class, IBaseRepository<TEntity>
    {
        protected readonly IAppValidator<TDto> _validator; // Change to IAppValidator
        protected new readonly TRepository _repository;

        protected CrudService(TRepository repository, IAppValidator<TDto> validator) // Change parameter type
            : base(repository)
        {
            _repository = repository;
            _validator = validator;
        }

        #region phương thức trừu tượng để map
        /// <summary>
        /// Lớp con phải định nghĩa cách ánh xạ từ Entity sang DTO.
        /// </summary>
        /// <param name="entity">Đối tượng Entity cần ánh xạ.</param>
        /// <returns>Đối tượng DTO tương ứng.</returns>
        /// Created by Phuong 26/02/2026
        /// </summary>
        protected abstract TDto MapToDto(TEntity entity);

        /// <summary>
        /// Lớp con phải định nghĩa cách ánh xạ từ DTO sang Entity (để tạo mới).
        /// </summary>
        /// <param name="dto">Đối tượng DTO chứa dữ liệu để tạo Entity.</param>
        /// <returns>Đối tượng Entity đã được ánh xạ.</returns>
        /// Created by Phuong 26/02/2026
        /// </summary>
        protected abstract TEntity MapToEntity(TDto dto);

        /// <summary>
        /// Lớp con phải định nghĩa cách cập nhật một Entity từ một DTO.
        /// </summary>
        /// <param name="entity">Đối tượng Entity hiện có cần cập nhật.</param>
        /// <param name="dto">Đối tượng DTO chứa dữ liệu cập nhật.</param>
        /// Created by Phuong 26/02/2026
        /// </summary>
        protected abstract void MapToEntity(TEntity entity, TDto dto);
        #endregion
        
        #region triển khai CRUD

        /// <summary>
        /// Tạo object
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exceptions.ValidationException"></exception>
        public virtual async Task<Guid> CreateAsync(TDto dto)
        {
            // 1. Kiểm tra DTO đầu vào có null hay không
            ArgumentNullException.ThrowIfNull(dto);

            // 2. Thực hiện validate DTO
            var errors = _validator.Validate(dto); // Use our custom validator
            if (errors.Count > 0)
            {
                // Nếu có lỗi validation, ném ra ValidationException
                throw new Exceptions.ValidationException(errors);
            }

            // 3. Ánh xạ DTO sang Entity và gọi phương thức tạo mới của BaseService
            var entity = MapToEntity(dto);
            return await base.CreateAsync(entity);
        }

        /// <summary>
        /// Sửa object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exceptions.ValidationException"></exception>
        public virtual async Task<bool> UpdateAsync(Guid id, TDto dto)
        {
            // 1. Kiểm tra DTO đầu vào có null hay không
            ArgumentNullException.ThrowIfNull(dto);

            // 2. Thực hiện validate DTO
            var errors = _validator.Validate(dto); // Use our custom validator
            if (errors.Count > 0)
            {
                // Nếu có lỗi validation, ném ra ValidationException
                throw new Exceptions.ValidationException(errors);
            }

            // 3. Lấy Entity hiện có từ repository
            var existingEntity = await base.GetByIdAsync(id);
            if (existingEntity == null) return false;

            // 4. Ánh xạ dữ liệu từ DTO vào Entity hiện có và gọi phương thức cập nhật của BaseService
            MapToEntity(existingEntity, dto);
            return await base.UpdateAsync(existingEntity);
        }

        /// <summary>
        /// Lấy tất cả object
        /// </summary>
        /// <returns></returns>
        public new virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            // 1. Lấy tất cả các Entity từ repository
            var entities = await _repository.GetAllAsync();
            // 2. Ánh xạ danh sách Entity sang danh sách DTO và trả về
            return entities.Select(MapToDto);
        }

        /// <summary>
        /// Lấy object theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            // 1. Lấy Entity theo ID từ repository
            var entity = await base.GetByIdAsync(id);
            // 2. Nếu tìm thấy Entity, ánh xạ sang DTO và trả về, ngược lại trả về null
            return entity == null ? null : MapToDto(entity);
        }
        #endregion
    }
}
