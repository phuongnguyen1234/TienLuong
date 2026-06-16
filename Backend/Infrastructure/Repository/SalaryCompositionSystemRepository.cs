using Core.Entities;
using Core.Interfaces.Database;
using Core.Interfaces.Repository;
using Core.DTO;
using Dapper;
using Infrastructure.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class SalaryCompositionSystemRepository : BaseRepository<SalaryCompositionSystem>, ISalaryCompositionSystemRepository
    {
        public SalaryCompositionSystemRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "pa_salary_composition_system";
        protected override string KeyName => "ScsId";

        /// <summary>
        /// Lấy danh sách phân trang và lọc cho TPL hệ thống
        /// </summary>
        public async Task<(IEnumerable<SalaryCompositionSystem> Items, long TotalCount)> GetPagingAsync(
            string? searchTerm, List<FilterCriteria>? filters, int pageIndex, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryCompositionSystem", "GetPaging");
            
            string? whereClause = BuildWhereClause(filters);
            
            var parameters = new DynamicParameters();
            parameters.Add("p_search_term", searchTerm);
            parameters.Add("p_where_clause", whereClause);
            parameters.Add("p_page_index", pageIndex);
            parameters.Add("p_page_size", pageSize);

            // Standard: Đọc multi result set (ResultSet 1: Count, ResultSet 2: Items)
            using var multi = await connection.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            
            var totalCount = await multi.ReadFirstAsync<long>();
            var items = (await multi.ReadAsync<SalaryCompositionSystem>()).ToList();

            var result = (Items: (IEnumerable<SalaryCompositionSystem>)items, TotalCount: totalCount);
            return result;
        }

        /// <summary>
        /// Thực hiện Clone hàng loạt và xóa cache
        /// </summary>
        public async Task<int> BulkCloneAsync(List<Guid> systemIds)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("SalaryCompositionSystem", "BulkClone");
            
            var affected = await connection.ExecuteScalarAsync<int>(sql, 
                new { p_scs_ids = string.Join(",", systemIds) }, 
                commandType: CommandType.StoredProcedure);

            return affected;
        }

        /// <summary>
        /// Lấy cấu hình cột theo GridKey
        /// </summary>
        public async Task<GridConfig?> GetGridConfigAsync(string gridKey)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("GridConfig", "GetConfig");
            return await connection.QueryFirstOrDefaultAsync<GridConfig>(
                sql, 
                new { p_grid_key = gridKey }, 
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Lưu hoặc cập nhật cấu hình cột
        /// </summary>
        public async Task<bool> SaveGridConfigAsync(GridConfig config)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("GridConfig", "UpsertConfig");
            var affected = await connection.ExecuteAsync(
                sql, 
                new { p_grid_key = config.GridKey, p_config_data = config.ConfigData }, 
                commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            // Gọi base delete (xóa vật lý hoặc theo logic base)
            var result = await base.DeleteAsync(id);
            return result;
        }

        private string? BuildWhereClause(List<FilterCriteria>? criteria)
        {
            if (criteria == null || criteria.Count == 0) return null;

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

        private string GetDbColumnName(string propertyName)
        {
            var snake = propertyName.ToSnakeCase();
            if (snake.StartsWith("scs_")) return snake; // Ví dụ: ScsId -> scs_id
            
            // Nếu property truyền vào là ScCode (từ Entity base) nhưng DB là scs_code
            if (snake.StartsWith("sc_")) return "scs_" + snake.Substring(3);

            return "scs_" + snake;
        }
    }
}