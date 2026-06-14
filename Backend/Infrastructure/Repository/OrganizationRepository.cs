using Core.Entities;
using Core.Interfaces.Database;
using Core.Interfaces.Repository;
using Dapper;
using Infrastructure.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Repository triển khai cho Organization
    /// </summary>
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "pa_organization";
        protected override string KeyName => "OrganizationId";

        public override async Task<IEnumerable<Organization>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = SqlExtension.GetQuery("Organization", "GetAll");
            return await connection.QueryAsync<Organization>(sql);
        }
    }
}