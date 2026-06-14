using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    using Core.Interfaces.Database;
    using Core.Interfaces.Repository;
    using Infrastructure.Extensions;
    using Dapper;
    using System.Data;
    using System.Data.Common;
    using System.Reflection;
    using Core.Entities;
    using Core.DTO;

    /// <summary>
    /// Lớp base repository sử dụng Dapper để thực hiện các thao tác CRUD cơ bản trên database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// Created by Phuong 25/02/2026
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Cấu hình Dapper mapping cho Entity. Gọi 1 lần duy nhất tại Startup.
        /// </summary>
        /// Created by Phuong 25/02/2026
        public static void ConfigureDapperMapping()
        {
            SqlMapper.SetTypeMap(
                typeof(Organization),
                new CustomPropertyTypeMap(
                    typeof(Organization),
                    (type, columnName) => type.GetProperty(columnName.ToPascalCase())
                )
            );

            SqlMapper.SetTypeMap(
                typeof(SalaryCompositionSystem),
                new CustomPropertyTypeMap(
                    typeof(SalaryCompositionSystem),
                    (type, columnName) =>
                    {
                        // Map các trường riêng của System
                        if (columnName.Equals("scs_id", StringComparison.OrdinalIgnoreCase) || 
                            columnName.Equals("scs_is_in_used", StringComparison.OrdinalIgnoreCase))
                        {
                            return type.GetProperty(columnName.ToPascalCase());
                        }

                        // Standard Mapping cho TPL Hệ thống:
                        // Ví dụ: scs_code (DB) -> ScCode (Entity), scs_nature (DB) -> ScNature (Entity)
                        if (columnName.StartsWith("scs_", StringComparison.OrdinalIgnoreCase))
                        {
                            var baseColumnName = string.Concat("sc_", columnName.AsSpan(4)); 
                            var property = type.GetProperty(baseColumnName.ToPascalCase());
                            if (property != null) return property;
                        }

                        return type.GetProperty(columnName.ToPascalCase());
                    }
                )
            );

            SqlMapper.SetTypeMap(
                typeof(SalaryComposition),
                new CustomPropertyTypeMap(
                    typeof(SalaryComposition),
                    (type, columnName) => type.GetProperty(columnName.ToPascalCase())
                )
            );

            SqlMapper.SetTypeMap(
                typeof(OrganizationMapping),
                new CustomPropertyTypeMap(
                    typeof(OrganizationMapping),
                    (type, columnName) => type.GetProperty(columnName.ToPascalCase())
                )
            );

            SqlMapper.SetTypeMap(
                typeof(OrganizationLookupDTO),
                new CustomPropertyTypeMap(
                    typeof(OrganizationLookupDTO),
                    (type, columnName) => type.GetProperty(columnName.ToPascalCase())
                )
            );
        }

        /// <summary>
        /// Tên bảng trong database. Mặc định là tên class T chuyển sang snake_case. Override trong repository cụ thể nếu tên bảng khác.
        /// </summary>
        protected virtual string TableName => typeof(T).Name.ToSnakeCase();

        /// <summary>
        /// Tên cột ID chính. Mặc định là "Id" hoặc "{ClassName}Id". Override trong repository cụ thể nếu tên cột khác.
        /// </summary>
        protected virtual string KeyName => $"{typeof(T).Name}Id";

        // Hàm tiện ích để đặt tên bảng và cột trong MySQL với dấu backticks để tránh lỗi với từ khóa hoặc tên có dấu cách
        private static string MysqlQuote(string identifier) => $"`{identifier}`";

        /// <summary>
        /// Lấy một đối tượng Entity theo ID.
        /// </summary>
        /// <param name="id">ID của đối tượng cần lấy.</param>
        /// <returns>Đối tượng Entity nếu tìm thấy, ngược lại là null.</returns>
        /// Created by Phuong 25/02/2026
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            // 1. Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            
            // 2. Mở kết nối (nếu là DbConnection, sử dụng OpenAsync).
            if (connection is DbConnection dbConnection) 
                await dbConnection.OpenAsync();
            else
                connection.Open();

            var table = MysqlQuote(TableName);
            var key = MysqlQuote(KeyName.ToSnakeCase());
            var sql = $"SELECT * FROM {table} WHERE {key} = @Id";
            // 3. Thực thi truy vấn và trả về đối tượng đầu tiên tìm thấy hoặc null.
            return await connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        /// <summary>
        /// Lấy tất cả các đối tượng Entity từ bảng tương ứng.
        /// </summary>
        /// <returns>Một IEnumerable chứa tất cả các đối tượng Entity.</returns>
        /// Created by Phuong 25/02/2026
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // 1. Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            
            // 2. Mở kết nối.
            if (connection is DbConnection dbConnection) 
                await dbConnection.OpenAsync();
            else
                connection.Open();
            // 3. Thực thi truy vấn SELECT * và trả về danh sách các đối tượng.
            var table = MysqlQuote(TableName);
            var sql = $"SELECT * FROM {table}";
            return await connection.QueryAsync<T>(sql);
        }

        /// <summary>
        /// Thêm mới object
        /// </summary> 
        /// <param name="entity">Đối tượng Entity cần thêm mới.</param>
        /// <returns>ID của đối tượng Entity vừa được tạo.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu entity là null.</exception>
        /// Created by Phuong 25/02/2026
        public virtual async Task<Guid> CreateAsync(T entity)
        {
            // 1. Kiểm tra entity đầu vào có null hay không.
            ArgumentNullException.ThrowIfNull(entity);

            // 2. Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            // 3. Mở kết nối.
            if (connection is DbConnection dbConnection) 
                await dbConnection.OpenAsync();
            else
                connection.Open();

            return await CreateAsync(entity, connection, null);
        }

        /// <summary>
        /// Phương thức nội bộ để thêm mới một đối tượng Entity vào cơ sở dữ liệu, có hỗ trợ transaction.
        /// </summary>
        /// <param name="entity">Đối tượng Entity cần thêm mới.</param>
        /// <param name="connection">Đối tượng IDbConnection đang sử dụng.</param>
        /// <param name="transaction">Đối tượng IDbTransaction (tùy chọn) để thực hiện trong một transaction.</param>
        /// <returns>ID của đối tượng Entity vừa được tạo.</returns>
        /// <exception cref="InvalidOperationException">Ném ra nếu Entity không có thuộc tính nào để insert.</exception>
        /// Created by Phuong 25/02/2026
        protected virtual async Task<Guid> CreateAsync(T entity, IDbConnection connection, IDbTransaction? transaction)
        {
            // 1. Xử lý ID của Entity: Nếu ID là Guid và chưa được gán, tạo mới một Guid.
            var idProp = typeof(T).GetProperty(KeyName, BindingFlags.Public | BindingFlags.Instance); 
            Guid id; 
            if (idProp != null && (idProp.PropertyType == typeof(Guid) || idProp.PropertyType == typeof(Guid?)) && idProp.CanWrite)
            {
                var current = idProp.GetValue(entity);
                id = current is Guid g && g != Guid.Empty ? g : Guid.NewGuid();
                idProp.SetValue(entity, id);
            }
            else
            {
                id = Guid.NewGuid();
            }
            
            // 2. Lấy tất cả các thuộc tính công khai, có thể đọc/ghi của Entity.
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.CanWrite)
                                 .ToArray();

            // 3. Kiểm tra xem có thuộc tính nào để insert hay không.
            if (props.Length == 0)
                throw new InvalidOperationException($"Type {typeof(T).FullName} has no readable/writable properties to insert.");

            // 4. Xây dựng danh sách tên cột (snake_case) và tên tham số (PascalCase) cho câu lệnh INSERT.
            var columns = props.Select(p => MysqlQuote(p.Name.ToSnakeCase()));
            var parameters = props.Select(p => "@" + p.Name);

            var columnList = string.Join(", ", columns);
            var paramList = string.Join(", ", parameters);

            // 5. Xây dựng câu lệnh SQL INSERT.
            var table = MysqlQuote(TableName);
            var sql = $"INSERT INTO {table} ({columnList}) VALUES ({paramList})";

            var affected = await connection.ExecuteAsync(sql, entity, transaction: transaction);
            return affected > 0 ? id : Guid.Empty;
        }

        /// <summary>
        /// Cập nhật object
        /// </summary> 
        /// <param name="entity">Đối tượng Entity cần cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu entity là null.</exception>
        /// Created by Phuong 25/02/2026
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            // 1. Kiểm tra entity đầu vào có null hay không.
            ArgumentNullException.ThrowIfNull(entity);

            // 2. Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            // 3. Mở kết nối.
            if (connection is DbConnection dbConnection) 
                await dbConnection.OpenAsync();
            else
                connection.Open();

            return await UpdateAsync(entity, connection, null);
        }

        /// <summary>
        /// Phương thức nội bộ để cập nhật một đối tượng Entity vào cơ sở dữ liệu, có hỗ trợ transaction.
        /// </summary>
        /// <param name="entity">Đối tượng Entity cần cập nhật.</param>
        /// <param name="connection">Đối tượng IDbConnection đang sử dụng.</param>
        /// <param name="transaction">Đối tượng IDbTransaction (tùy chọn) để thực hiện trong một transaction.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        /// <exception cref="InvalidOperationException">Ném ra nếu Entity không có thuộc tính nào để update.</exception>
        /// Created by Phuong 25/02/2026
        protected virtual async Task<bool> UpdateAsync(T entity, IDbConnection connection, IDbTransaction? transaction)
        {
            // 1. Lấy tất cả các thuộc tính công khai, có thể đọc/ghi của Entity, trừ thuộc tính khóa chính.
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.CanWrite && !string.Equals(p.Name, KeyName, StringComparison.OrdinalIgnoreCase))
                                 .ToArray();
            // 2. Kiểm tra xem có thuộc tính nào để update hay không.
            if (props.Length == 0)
                throw new InvalidOperationException($"Type {typeof(T).FullName} has no updatable properties.");
            // Set columns to snake_case, parameters use CLR property names
            var setClause = string.Join(", ", props.Select(p => $"{MysqlQuote(p.Name.ToSnakeCase())} = @{p.Name}"));
            var table = MysqlQuote(TableName);
            var key = MysqlQuote(KeyName.ToSnakeCase());
            var sql = $"UPDATE {table} SET {setClause} WHERE {key} = @{KeyName}";

            var affected = await connection.ExecuteAsync(sql, entity, transaction: transaction);
            return affected > 0;
        }

        /// <summary>
        /// Xóa object theo ID
        /// </summary> 
        /// <param name="id">ID của đối tượng Entity cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        /// Created by Phuong 25/02/2026
        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            // 1. Tạo kết nối đến cơ sở dữ liệu.
            using var connection = _connectionFactory.CreateConnection();
            // 2. Mở kết nối.
            if (connection is DbConnection dbConnection) 
                await dbConnection.OpenAsync();
            else
                connection.Open();

            return await DeleteAsync(id, connection, null);
        }

        /// <summary>
        /// Phương thức nội bộ để xóa một đối tượng Entity theo ID khỏi cơ sở dữ liệu, có hỗ trợ transaction.
        /// </summary>
        /// <param name="id">ID của đối tượng Entity cần xóa.</param>
        /// <param name="connection">Đối tượng IDbConnection đang sử dụng.</param>
        /// <param name="transaction">Đối tượng IDbTransaction (tùy chọn) để thực hiện trong một transaction.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        /// Created by Phuong 25/02/2026
        protected virtual async Task<bool> DeleteAsync(Guid id, IDbConnection connection, IDbTransaction? transaction)
        {
            var table = MysqlQuote(TableName);
            var key = MysqlQuote(KeyName.ToSnakeCase());
            var sql = $"DELETE FROM {table} WHERE {key} = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id }, transaction: transaction);
            return affected > 0;
        }
    }
}
