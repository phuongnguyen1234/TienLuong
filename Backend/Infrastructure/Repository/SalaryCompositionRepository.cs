using Core.Entities;
using Core.Interfaces.Database;
using Core.Interfaces.Repository;
using Core.DTO;
using Dapper;
using Infrastructure.Extensions;
using System.Data; // Keep this as it's used for CommandType

namespace Infrastructure.Repository
{
    public class SalaryCompositionRepository : BaseRepository<SalaryComposition>, ISalaryCompositionRepository
    {
        public SalaryCompositionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "pa_salary_composition";
        protected override string KeyName => "ScId";

        /// <summary>
        /// Lấy danh sách thành phần lương có phân trang, tìm kiếm và lọc.
        /// Sử dụng cache để cải thiện hiệu suất cho các truy vấn lặp lại.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tùy chọn).</param>
        /// <param name="filters">Danh sách các tiêu chí lọc (tùy chọn).</param>
        /// <param name="pageIndex">Chỉ số trang.</param>
        /// <param name="pageSize">Kích thước trang.</param>
        /// <returns>Một tuple chứa danh sách các thành phần lương, các ánh xạ tổ chức và tổng số bản ghi.</returns>
        /// <summary>
        /// Thực thi proc_get_pa_salary_composition_paging
        /// </summary>
        public async Task<(IEnumerable<SalaryComposition> Items, 
            IEnumerable<OrganizationMapping> Mappings, 
            long TotalCount)> GetPagingAsync(string? searchTerm, List<FilterCriteria>? filters, int pageIndex, int pageSize)
        {
            // Bước 2: Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            
            // Bước 3: Lấy tên stored procedure từ file cấu hình JSON.
            var sql = SqlExtension.GetQuery("SalaryComposition", "GetPaging");
            
            // Bước 4: Xây dựng mệnh đề WHERE động từ các tiêu chí lọc.
            string? whereClause = BuildWhereClause(filters);
            
            // Bước 5: Khởi tạo DynamicParameters cho Dapper để truyền tham số vào stored procedure.
            var parameters = new DynamicParameters();
            parameters.Add("p_search_term", searchTerm);
            parameters.Add("p_where_clause", whereClause); 
            parameters.Add("p_page_index", pageIndex);
            parameters.Add("p_page_size", pageSize);

            // Bước 6: Thực thi stored procedure và đọc nhiều tập kết quả.
            // Stored procedure này trả về 3 tập kết quả:
            // 1. Tổng số bản ghi (TotalCount)
            // 2. Danh sách các thành phần lương (Items)
            // 3. Danh sách các ánh xạ tổ chức (Mappings)
            using var multi = await connection.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            
            var totalCount = await multi.ReadFirstAsync<long>();
            var items = (await multi.ReadAsync<SalaryComposition>()).ToList();
            
            var mappings = (await multi.ReadAsync<OrganizationMapping>()).ToList();

            // Bước 7: Đóng gói kết quả và lưu vào cache trước khi trả về.
            return (Items: items, Mappings: mappings, TotalCount: totalCount);
        }

        /// <summary>
        /// Thực thi thêm mới TPL và mapping đơn vị sử dụng Transaction
        /// </summary> 
        /// <param name="entity">Đối tượng SalaryComposition cần thêm mới.</param>
        /// <param name="organizationIds">Danh sách các ID tổ chức áp dụng.</param>
        /// <returns>ID của thành phần lương vừa được tạo.</returns>
        public async Task<Guid> CreateWithOrganizationsAsync(SalaryComposition entity, List<Guid> organizationIds)
        {
            // Bước 1: Mở kết nối và bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // 1. Thêm mới bản ghi chính vào pa_salary_composition
                var sqlInsert = SqlExtension.GetQuery("SalaryComposition", "Insert");
                var parameters = new DynamicParameters();
                
                // Map thủ công hoặc chuẩn hóa prefix sc_ để khớp với Procedure
                parameters.Add("p_sc_id", entity.ScId);
                parameters.Add("p_sc_code", entity.ScCode);
                parameters.Add("p_sc_name", entity.ScName);
                parameters.Add("p_sc_type", entity.ScType);
                parameters.Add("p_sc_nature", entity.ScNature);
                parameters.Add("p_sc_tax_status", entity.ScTaxStatus);
                parameters.Add("p_sc_is_tax_deductible", entity.ScIsTaxDeductible);
                parameters.Add("p_sc_taxable_expression", entity.ScTaxableExpression);
                parameters.Add("p_sc_exempt_expression", entity.ScExemptExpression);
                parameters.Add("p_sc_limit_expression", entity.ScLimitExpression);
                parameters.Add("p_sc_allow_exceed_limit", entity.ScAllowExceedLimit);
                parameters.Add("p_sc_value_type", entity.ScValueType);
                parameters.Add("p_sc_calculation_method", entity.ScCalculationMethod);
                parameters.Add("p_sc_aggregation_scope", entity.ScAggregationScope);
                parameters.Add("p_sc_composition_code", entity.ScCompositionCode);
                parameters.Add("p_sc_organization_level", entity.ScOrganizationLevel);
                parameters.Add("p_sc_formula_expression", entity.ScFormulaExpression);
                parameters.Add("p_sc_description", entity.ScDescription);
                parameters.Add("p_sc_is_displayed_on_payroll", entity.ScIsDisplayedOnPayroll);
                parameters.Add("p_sc_source", entity.ScSource);

                // Thực thi stored procedure để thêm bản ghi chính.
                await connection.ExecuteAsync(sqlInsert, parameters, transaction, commandType: CommandType.StoredProcedure);

                // 2. Thêm mapping đơn vị vào salarycomposition_organization
                if (organizationIds != null && organizationIds.Count != 0)
                {
                    var sqlOrg = SqlExtension.GetQuery("SalaryComposition", "UpsertOrganization");
                    // Chuyển danh sách GUID thành chuỗi phân tách bằng dấu phẩy để truyền vào stored procedure.
                    var orgIdsString = string.Join(",", organizationIds);
                    await connection.ExecuteAsync(sqlOrg, new { p_sc_id = entity.ScId, p_organization_ids = orgIdsString }, transaction, commandType: CommandType.StoredProcedure);
                }

                transaction.Commit();
                return entity.ScId;
            }
            // Nếu có lỗi, rollback transaction và ném lại ngoại lệ.
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Cập nhật thành phần lương (TPL) và các ánh xạ đơn vị liên quan trong một transaction.
        /// </summary>
        /// <param name="entity">Đối tượng SalaryComposition chứa dữ liệu cập nhật.</param>
        /// <param name="organizationIds">Danh sách các ID tổ chức mới áp dụng cho TPL.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        public async Task<bool> UpdateWithOrganizationsAsync(SalaryComposition entity, List<Guid> organizationIds)
        {
            // Bước 1: Mở kết nối và bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // 1. Cập nhật bản ghi chính
                var sqlUpdate = SqlExtension.GetQuery("SalaryComposition", "Update");
                var parameters = new DynamicParameters();
                parameters.Add("p_sc_id", entity.ScId);
                parameters.Add("p_sc_name", entity.ScName);
                parameters.Add("p_sc_type", entity.ScType);
                parameters.Add("p_sc_nature", entity.ScNature);
                parameters.Add("p_sc_tax_status", entity.ScTaxStatus);
                parameters.Add("p_sc_is_tax_deductible", entity.ScIsTaxDeductible);
                parameters.Add("p_sc_taxable_expression", entity.ScTaxableExpression);
                parameters.Add("p_sc_exempt_expression", entity.ScExemptExpression);
                parameters.Add("p_sc_limit_expression", entity.ScLimitExpression);
                parameters.Add("p_sc_allow_exceed_limit", entity.ScAllowExceedLimit);
                parameters.Add("p_sc_value_type", entity.ScValueType);
                parameters.Add("p_sc_calculation_method", entity.ScCalculationMethod);
                parameters.Add("p_sc_aggregation_scope", entity.ScAggregationScope);
                parameters.Add("p_sc_composition_code", entity.ScCompositionCode);
                parameters.Add("p_sc_organization_level", entity.ScOrganizationLevel);
                parameters.Add("p_sc_formula_expression", entity.ScFormulaExpression);
                parameters.Add("p_sc_description", entity.ScDescription);
                parameters.Add("p_sc_is_displayed_on_payroll", entity.ScIsDisplayedOnPayroll);
                parameters.Add("p_sc_status", (int)entity.ScStatus);

                // Thực thi stored procedure để cập nhật bản ghi chính.
                var affectedRows = await connection.ExecuteAsync(sqlUpdate, parameters, transaction, commandType: CommandType.StoredProcedure);

                // 2. Cập nhật mapping đơn vị (Proc này sẽ DELETE cũ và INSERT mới)
                var sqlOrg = SqlExtension.GetQuery("SalaryComposition", "UpsertOrganization");
                // Chuyển danh sách GUID thành chuỗi phân tách bằng dấu phẩy để truyền vào stored procedure.
                var orgIdsString = organizationIds != null ? string.Join(",", organizationIds) : "";
                await connection.ExecuteAsync(sqlOrg, new { p_sc_id = entity.ScId, p_organization_ids = orgIdsString }, transaction, commandType: CommandType.StoredProcedure);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch
            // Nếu có lỗi, rollback transaction và ném lại ngoại lệ.
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Thực thi proc_get_pa_salary_composition_by_id để lấy data đa thành phần
        /// </summary> 
        /// <param name="id">ID của thành phần lương cần lấy.</param>
        /// <returns>Một tuple chứa đối tượng SalaryComposition và danh sách các OrganizationLookupDTO liên quan.</returns>
        public async Task<(SalaryComposition? Entity, List<OrganizationLookupDTO> Organizations)> GetByIdWithOrganizationsAsync(Guid id)
        {
            // Bước 1: Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();

            // Bước 2: Lấy tên stored procedure từ file cấu hình JSON.
            var sql = SqlExtension.GetQuery("SalaryComposition", "GetById");

            // Bước 3: Thực thi stored procedure và đọc nhiều tập kết quả (thành phần lương và tổ chức).         
            using var multi = await connection.QueryMultipleAsync(sql, new { p_sc_id = id }, commandType: CommandType.StoredProcedure);
            
            var entity = await multi.ReadFirstOrDefaultAsync<SalaryComposition>();
            var organizations = (await multi.ReadAsync<OrganizationLookupDTO>()).ToList();

            return (entity, organizations);
        }

        /// <summary>
        /// Ghi đè phương thức xóa để thực hiện xóa mềm qua Stored Procedure
        /// </summary> 
        /// <param name="id">ID của thành phần lương cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        public override async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryComposition", "Delete");
            return await connection.ExecuteAsync(sql, new { p_sc_id = id }, commandType: CommandType.StoredProcedure) > 0;

        }

        /// <summary>
        /// Thực thi xóa mềm hàng loạt thông qua Stored Procedure
        /// </summary>
        /// <param name="ids">Danh sách các ID của thành phần lương cần xóa.</param>
        /// <returns>Số bản ghi đã xóa. Có thể nhỏ hơn số lượng id ban đầu do có thể chứa TPL hệ thống</returns>
        /// Created by Phuong 24/05/2026
        public async Task<int> BulkDeleteAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0) return 0;

            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryComposition", "BulkDelete");
            return await connection.ExecuteAsync(sql, new { p_ids = string.Join(",", ids) }, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Thực thi cập nhật trạng thái hàng loạt qua Stored Procedure
        /// </summary> 
        /// <param name="ids">Danh sách các ID của thành phần lương cần cập nhật trạng thái.</param>
        /// <param name="status">Trạng thái mới (ví dụ: 0 cho Đang theo dõi, 1 cho Ngừng theo dõi).</param>
        /// <returns>True nếu cập nhật thành công ít nhất một bản ghi, ngược lại là false.</returns>
        public async Task<bool> BulkUpdateStatusAsync(List<Guid> ids, int status)
        {
            if (ids == null || ids.Count == 0) return false;

            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryComposition", "BulkUpdateStatus");
            var affected = await connection.ExecuteAsync(sql, 
                new { p_ids = string.Join(",", ids), p_status = status }, 
                commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public override async Task<Guid> CreateAsync(SalaryComposition entity)
        {
            var id = await base.CreateAsync(entity);
            return id;
        }

        public override async Task<bool> UpdateAsync(SalaryComposition entity)
        {
            var result = await base.UpdateAsync(entity);
            return result;
        }

        /// <summary>
        /// Lấy danh sách các thành phần lương dưới dạng lookup (rút gọn) có sử dụng IMemoryCache.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tùy chọn). Nếu có searchTerm, sẽ không sử dụng cache hoặc cache theo key search.</param>
        /// <returns>Danh sách các SalaryCompositionLookupDTO.</returns>
        public async Task<IEnumerable<SalaryCompositionLookupDTO>> GetLookupsAsync(string? searchTerm)
        {
            return await FetchLookupsFromDb(searchTerm);
        }
        /// <summary>
        /// Lấy danh sách các thành phần lương dưới dạng lookup trực tiếp từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tùy chọn).</param>
        /// <returns>Danh sách các SalaryCompositionLookupDTO.</returns>
        /// Created by Phuong 24/05/2026
        private async Task<IEnumerable<SalaryCompositionLookupDTO>> FetchLookupsFromDb(string? searchTerm)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryComposition", "GetLookup");
            return await connection.QueryAsync<SalaryCompositionLookupDTO>(sql, new { p_search_term = searchTerm }, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Kiểm tra xem mã thành phần lương có bị trùng lặp trong cơ sở dữ liệu hay không.
        /// </summary>
        /// <param name="code">Mã thành phần lương cần kiểm tra.</param>
        /// <param name="excludeId">ID của bản ghi hiện tại (nếu đang sửa) để loại trừ khỏi kiểm tra trùng lặp.</param>
        /// <returns>True nếu mã bị trùng, ngược lại là false.</returns>
        public async Task<bool> CheckCodeDuplicateAsync(string code, Guid? excludeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryComposition", "CheckCode");
            var isDuplicate = await connection.ExecuteScalarAsync<bool>(sql, new { p_sc_code = code, p_sc_id = excludeId }, commandType: CommandType.StoredProcedure);
            return isDuplicate;
        }

        /// <summary>
        /// Xây dựng mệnh đề WHERE động dựa trên danh sách các tiêu chí lọc.
        /// </summary>
        /// <param name="criteria">Danh sách các FilterCriteria.</param>
        /// <returns>Chuỗi mệnh đề WHERE (ví dụ: " AND (sc.sc_code LIKE '%ABC%')") hoặc null nếu không có tiêu chí.</returns>
        /// Created by Phuong 24/05/2026
        private static string? BuildWhereClause(List<FilterCriteria>? criteria)
        {
            if (criteria == null || criteria.Count == 0) return null; // Nếu không có tiêu chí, trả về null.
            
            // Nhóm các tiêu chí theo cột. Nếu một cột có nhiều tiêu chí (ví dụ: nhiều OrganizationId), nối bằng OR.
            var groupedClauses = criteria
                .GroupBy(item => item.Column)
                .Select(group => {
                    var columnClauses = group.Select(item => {
                        var columnName = GetDbColumnName(item.Column);
                        var value = item.Value?.Replace("'", "''");
                        return item.Operation switch
                        {
                            FilterOperation.Contains => $"{columnName} LIKE '%{value}%'",
                            FilterOperation.NotContains => $"{columnName} NOT LIKE '%{value}%'",
                            FilterOperation.StartsWith => $"{columnName} LIKE '{value}%'",
                            FilterOperation.EndsWith => $"{columnName} LIKE '%{value}'",
                            FilterOperation.Equals => $"{columnName} = '{value}'",
                            FilterOperation.Empty => $"({columnName} IS NULL OR {columnName} = '')",
                            FilterOperation.NotEmpty => $"({columnName} IS NOT NULL AND {columnName} <> '')",
                            _ => "1=1"
                        };
                    });
                    return $"({string.Join(" OR ", columnClauses)})";
                });

            return " AND " + string.Join(" AND ", groupedClauses);
        }

        /// <summary>
        /// Ánh xạ tên thuộc tính từ DTO sang tên cột trong cơ sở dữ liệu, kèm theo alias bảng (sc hoặc sco).
        /// </summary>
        /// <param name="propertyName">Tên thuộc tính từ DTO (PascalCase).</param>
        /// <returns>Tên cột trong DB với alias (ví dụ: "sc.sc_code", "sco.organization_id").</returns>
        private static string GetDbColumnName(string propertyName)
        {
            // Cột nằm ở bảng mapping đơn vị
            if (propertyName == "OrganizationId") return "sco.organization_id";

            var snake = propertyName.ToSnakeCase();
            
            // Các trường đặc thù trong DTO map sang DB (TaxStatus -> sc_tax_status)
            if (propertyName == "TaxStatus") return "sc.sc_tax_status";
            if (propertyName == "IsTaxDeductible") return "sc.sc_is_tax_deductible";
            
            // Nếu thuộc tính đã có tiền tố Sc (như ScCode -> sc_code)
            if (snake.StartsWith("sc_")) return $"sc.{snake}";
            
            // Các trường trong pa_salary_composition nhưng DTO không để tiền tố Sc (TaxStatus -> sc_tax_status)
            return $"sc.sc_{snake}";
        }
    }
}