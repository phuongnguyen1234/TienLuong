using Core.Entities;
using Core.Interfaces.Repository;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddInfrastructureDI(
            this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<ISalaryCompositionSystemRepository, SalaryCompositionSystemRepository>();
            services.AddScoped<ISalaryCompositionRepository, SalaryCompositionRepository>();
            
            // Đăng ký mapping để BaseService<TypeEntity> có thể resolve được
            services.AddScoped<IBaseRepository<Organization>>(sp => sp.GetRequiredService<IOrganizationRepository>());
            services.AddScoped<IBaseRepository<SalaryCompositionSystem>>(sp => sp.GetRequiredService<ISalaryCompositionSystemRepository>());
            services.AddScoped<IBaseRepository<SalaryComposition>>(sp => sp.GetRequiredService<ISalaryCompositionRepository>());

            return services;
        }
    }
}