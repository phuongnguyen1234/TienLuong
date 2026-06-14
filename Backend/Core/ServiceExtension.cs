﻿using Core.DTO;
using Core.Interfaces.Service;
using Core.Interfaces.Validators;
using Core.Services;
using Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServiceDI(
            this IServiceCollection services)
        {
            // Services
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<ISalaryCompositionSystemService, SalaryCompositionSystemService>();
            services.AddScoped<ISalaryCompositionService, SalaryCompositionService>();
            
            // Đăng ký để các controller dùng IBaseCrudService<TypeDto> có thể resolve được
            services.AddScoped<IBaseCrudService<OrganizationDTO>>(sp => sp.GetRequiredService<IOrganizationService>());
            services.AddScoped<IBaseCrudService<SalaryCompositionSystemDTO>>(sp => sp.GetRequiredService<ISalaryCompositionSystemService>());
            services.AddScoped<IBaseCrudService<SalaryCompositionDTO>>(sp => sp.GetRequiredService<ISalaryCompositionService>());

            // Validators
            services.AddScoped<IAppValidator<SalaryCompositionDTO>, SalaryCompositionAppValidator>();
            services.AddScoped<IAppValidator<OrganizationDTO>, OrganizationAppValidator>();
            services.AddScoped<IAppValidator<SalaryCompositionSystemDTO>, SalaryCompositionSystemAppValidator>();

            return services;
        }
    }
}
