using Application.Interfaces;
using Application.Mapping;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Automapper Configuration
            services.AddAutoMapper(typeof(MappingProfile));

            //Add Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            //Add Azure Storage
            services.AddSingleton<IStorageService, AzureBlobStorageService>();

            return services;
        }
    }
}
