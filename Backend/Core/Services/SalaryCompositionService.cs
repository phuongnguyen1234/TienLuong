using Core.DTO;
using Core.Entities;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Mappers;
using Core.Interfaces.Validators;
using System.Text;
using System.Globalization;
using System.Data.Common;

namespace Core.Services
{
    public class SalaryCompositionService : CrudService<SalaryComposition, SalaryCompositionDTO, ISalaryCompositionRepository>, ISalaryCompositionService
    {
        public SalaryCompositionService(ISalaryCompositionRepository repository, IAppValidator<SalaryCompositionDTO> validator)
            : base(repository, validator)
        {
        }

        /// <summary>
        /// Triển khai mapping từ Entity sang DTO cho Base Class
        /// </summary>
        /// <param name="entity">Đối tượng SalaryComposition Entity.</param>
        /// <returns>Đối tượng SalaryCompositionDTO đã được ánh xạ.</returns>
        /// Created by Phuong 24/05/2026
        protected override SalaryCompositionDTO MapToDto(SalaryComposition entity)
        {
            var dto = SalaryCompositionMapper.ToDto(entity);
            return dto;
        }

        /// <summary>
        /// Ánh xạ từ SalaryCompositionDTO sang SalaryComposition Entity.
        /// </summary>
        /// <param name="dto">Đối tượng SalaryCompositionDTO.</param>
        /// <returns>Đối tượng SalaryComposition Entity đã được ánh xạ.</returns>
        /// Created by Phuong 24/05/2026
        protected override SalaryComposition MapToEntity(SalaryCompositionDTO dto) => SalaryCompositionMapper.ToEntity(dto);

        /// <summary>
        /// Cập nhật SalaryComposition Entity từ SalaryCompositionDTO.
        /// </summary>
        /// <param name="entity">Đối tượng SalaryComposition Entity hiện có.</param>
        /// <param name="dto">Đối tượng SalaryCompositionDTO chứa dữ liệu cập nhật.</param>
        /// Created by Phuong 24/05/2026
        protected override void MapToEntity(SalaryComposition entity, SalaryCompositionDTO dto)
        {
            SalaryCompositionMapper.UpdateFromDto(entity, dto);
        }
        /// <summary>
        /// Ghi đè phương thức tạo mới để xử lý lưu danh sách đơn vị áp dụng
        /// </summary>
        public override async Task<Guid> CreateAsync(SalaryCompositionDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // 1. Xử lý mã thành phần lương (ScCode)
            // Nếu mã không được cung cấp hoặc rỗng, tự động sinh mã từ tên.
            if (string.IsNullOrWhiteSpace(dto.ScCode))
            {
                dto.ScCode = await GenerateCodeAsync(dto.ScName);
            }
            else
            {
                // Chuẩn hóa mã do client gửi lên.
                dto.ScCode = NormalizeCode(dto.ScCode);
                // Kiểm tra trùng lặp mã trong DB để tránh Race Condition hoặc lỗi nhập liệu.
                if (await _repository.CheckCodeDuplicateAsync(dto.ScCode, null))
                {
                    // Nếu mã đã tồn tại, ném ra ngoại lệ ValidationException.
                    var errors = new Dictionary<string, string> { { "ScCode", "Mã thành phần lương đã tồn tại trong hệ thống." } };
                    throw new Exceptions.ValidationException(errors);
                }
            }

            // 2. Chuyển đổi sang Entity và gọi Repo xử lý nghiệp vụ lưu trữ
            // Sử dụng khối try-catch để bắt các lỗi liên quan đến DB, đặc biệt là lỗi trùng lặp.
            try
            {
                var entity = MapToEntity(dto);
                // Trích xuất và gom nhóm (Simplify) danh sách OrganizationId từ DTO để đảm bảo dữ liệu sạch trước khi truyền xuống Repository.
                var organizationIds = SimplifyOrganizationIds([.. dto.AppliedOrganizations.Select(o => o.OrganizationId)]);
                
                
                var newId = await _repository.CreateWithOrganizationsAsync(entity, organizationIds);
                dto.ScId = newId;
                return newId;
            }
            catch (DbException ex) when (ex.Message.Contains("Duplicate") || ex.ErrorCode == 1062)
            {
                // Handle race condition từ DB
                throw new Exceptions.ValidationException(new Dictionary<string, string> 
                { 
                    { "ScCode", "Mã thành phần lương đã tồn tại (Race Condition detected)." } 
                });
            }
        }

        /// <summary>
        /// Ghi đè phương thức cập nhật
        /// </summary>
        public override async Task<bool> UpdateAsync(Guid id, SalaryCompositionDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // Chuẩn hóa mã do client gửi lên.
            dto.ScCode = NormalizeCode(dto.ScCode);
            
            // 1. Kiểm tra trùng lặp mã, loại trừ bản ghi hiện tại đang được sửa để tránh báo lỗi trùng với chính nó.
            // Nếu mã đã tồn tại với một ID khác, ném ra ngoại lệ ValidationException.
            if (await _repository.CheckCodeDuplicateAsync(dto.ScCode, id))
            {
                var errors = new Dictionary<string, string> { { "ScCode", "Mã thành phần lương đã tồn tại trong hệ thống." } };
                throw new Exceptions.ValidationException(errors);
            }

            // 2. Kiểm tra tồn tại
            // Lấy Entity hiện có từ repository để đảm bảo bản ghi tồn tại trước khi cập nhật.
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null) return false;

            // 3. Map dữ liệu thay đổi từ DTO vào Entity
            MapToEntity(existingEntity, dto);
            existingEntity.ScId = id; // Đảm bảo ID không bị thay đổi
            

            // Trích xuất danh sách OrganizationId từ AppliedOrganizations để truyền xuống Repository
            var organizationIds = SimplifyOrganizationIds([.. dto.AppliedOrganizations.Select(o => o.OrganizationId)]);
            
            // 4. Gọi Repo xử lý Transaction
            return await _repository.UpdateWithOrganizationsAsync(existingEntity, organizationIds);
        }

        /// <summary>
        /// TODO: Implement logic kiểm tra cha-con dựa trên DB để loại bỏ các ID con nếu ID cha đã tồn tại.
        /// Hiện tại bước này đã được xử lý triệt để ở Frontend để gửi dữ liệu tối ưu.
        /// </summary> 
        /// <param name="ids">Danh sách các GUID của tổ chức.</param>
        /// <returns>Danh sách các GUID tổ chức đã được đơn giản hóa (loại bỏ trùng lặp).</returns>
        private static List<Guid> SimplifyOrganizationIds(List<Guid> ids)
        {
            // Ở đây nếu có Service lấy Tree của Organization, ta sẽ filter ids.
            // Tạm thời trả về list gốc vì Frontend đã đảm bảo gom nhóm trước khi gửi.
            return [.. ids.Distinct()];
        }

        /// <summary>
        /// Ghi đè để lấy thêm danh sách đơn vị áp dụng
        /// </summary>
        /// <param name="id">ID của thành phần lương cần lấy.</param>
        /// <returns>SalaryCompositionDTO kèm danh sách các tổ chức áp dụng nếu tìm thấy, ngược lại là null.</returns>
        /// Created by Phuong 24/05/2026
        public override async Task<SalaryCompositionDTO?> GetByIdAsync(Guid id)
        {
            var (entity, organizations) = await _repository.GetByIdWithOrganizationsAsync(id);
            
            if (entity == null) return null;

            var dto = MapToDto(entity);
            dto.AppliedOrganizations = organizations;
            return dto;
        }

        /// <summary>
        /// Xóa hàng loạt thành phần lương
        /// </summary> 
        /// <param name="ids">Danh sách các ID của thành phần lương cần xóa.</param>
        /// <returns>True nếu xóa thành công ít nhất một bản ghi, ngược lại là false.</returns>
        /// Created by Phuong 24/05/2026
        public async Task<int> BulkDeleteAsync(List<Guid> ids)
        {
            return await _repository.BulkDeleteAsync(ids);
        }

        /// <summary>
        /// Cập nhật trạng thái hàng loạt cho các thành phần lương.
        /// </summary>
        /// <param name="ids">Danh sách các ID của thành phần lương cần cập nhật.</param>
        /// <param name="status">Trạng thái mới (ví dụ: 0 cho Đang theo dõi, 1 cho Ngừng theo dõi).</param>
        /// <returns>True nếu cập nhật thành công ít nhất một bản ghi, ngược lại là false.</returns>
        /// Created by Phuong 24/05/2026
        public async Task<bool> BulkUpdateStatusAsync(List<Guid> ids, int status)
        {
            return await _repository.BulkUpdateStatusAsync(ids, status);
        }

        /// <summary>
        /// Lấy danh sách các thành phần lương dưới dạng lookup (rút gọn) để sử dụng trong các dropdown hoặc gợi ý.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tùy chọn) để lọc danh sách lookup.</param>
        /// <returns>Danh sách các SalaryCompositionLookupDTO.</returns>
        /// Created by Phuong 24/05/2026
        public async Task<IEnumerable<SalaryCompositionLookupDTO>> GetLookupsAsync(string? searchTerm)
        {
            return await _repository.GetLookupsAsync(searchTerm);
        }

        /// <summary>
        /// Kiểm tra trùng mã
        /// </summary>
        public async Task<bool> CheckCodeDuplicateAsync(string code, Guid? excludeId)
        {
            return await _repository.CheckCodeDuplicateAsync(code, excludeId);
        }

        /// <summary>
        /// Sinh mã tự động từ tên thành phần lương, đảm bảo tính duy nhất.
        /// Phục vụ cho việc gợi ý mã ở Frontend và kiểm tra trùng lặp.
        /// </summary>
        /// <param name="name">Tên của thành phần lương.</param>
        /// <returns>Mã thành phần lương duy nhất.</returns>
        /// Created by Phuong 24/05/2026
        public async Task<string> GenerateCodeAsync(string name)
        {
            // Chuẩn hóa tên thành mã cơ sở (ví dụ: LUONG_CO_BAN).
            string baseCode = NormalizeCode(name);
            if (string.IsNullOrEmpty(baseCode)) return string.Empty;

            string uniqueCode = baseCode;
            int counter = 1;

            // Vòng lặp kiểm tra trùng lặp trong cơ sở dữ liệu.
            // Nếu mã đã tồn tại, thêm hậu tố số (ví dụ: _1, _2) cho đến khi tìm được mã duy nhất.
            while (await _repository.CheckCodeDuplicateAsync(uniqueCode, null))
            {
                uniqueCode = $"{baseCode}_{counter}";
                counter++;
            }

            return uniqueCode;
        }

        /// <summary>
        /// Chuẩn hóa một chuỗi đầu vào thành định dạng UPPER_SNAKE_CASE không dấu.
        /// Ví dụ: "Lương cơ bản" -> "LUONG_CO_BAN".
        /// </summary>
        /// <param name="input">Chuỗi cần chuẩn hóa.</param>
        /// <returns>Chuỗi đã được chuẩn hóa.</returns>
        /// Created by Phuong 24/05/2026
        private static string NormalizeCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // 1. Loại bỏ dấu tiếng Việt
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    // Chỉ lấy chữ cái và số, còn lại coi như khoảng trắng
                    if (char.IsLetterOrDigit(c))
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }
            }

            string noSign = sb.ToString().Normalize(NormalizationForm.FormC);

            // 2. Chuyển đổi chuỗi đã loại bỏ dấu sang định dạng UPPER_SNAKE_CASE.
            // Tách chuỗi thành các từ dựa trên khoảng trắng, chuyển mỗi từ thành chữ hoa, sau đó nối lại bằng dấu gạch dưới.
            var words = noSign.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(w => w.ToUpper());

            return string.Join("_", words);
        }
        /// <summary>
        /// Lấy danh sách thành phần lương có phân trang, tìm kiếm và lọc.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm theo mã hoặc tên.</param>
        /// <param name="filters">Danh sách các tiêu chí lọc.</param>
        /// <param name="pageIndex">Chỉ số trang hiện tại.</param>
        /// <param name="pageSize">Kích thước trang.</param>
        /// <returns>Một PagedResult chứa danh sách SalaryCompositionDTO, thông tin phân trang và tổng số bản ghi.</returns>
        /// Created by Phuong 24/05/2026
        public async Task<PagedResult<SalaryCompositionDTO>> GetPagingAsync(string? searchTerm, List<string>? filters, int pageIndex, int pageSize)
        {
            // Parse filters và build where clause
            var criteria = FilterQueryParser.Parse(filters);

            var (entities, mappings, totalCount) = await _repository.GetPagingAsync(searchTerm, criteria, pageIndex, pageSize);
            
            var dtos = entities.Select(entity => {
                var dto = MapToDto(entity);
                // Lắp ghép thông tin tổ chức từ danh sách mappings vào DTO
                dto.AppliedOrganizations = [.. mappings
                    .Where(m => m.ScId == entity.ScId)
                    .Select(m => new OrganizationLookupDTO {
                        OrganizationId = m.OrganizationId,
                        OrganizationName = m.OrganizationName
                    })];
                return dto;
            }).ToList();
            
            return new PagedResult<SalaryCompositionDTO>(
                dtos, 
                pageIndex, 
                pageSize, 
                totalCount);
        }
    }
}