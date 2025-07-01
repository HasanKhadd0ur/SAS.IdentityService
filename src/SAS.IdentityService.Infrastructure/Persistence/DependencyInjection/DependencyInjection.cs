using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAS.IdentityService.ApplicationCore.Entities;
using SAS.IdentityService.Infrastructure.Persistence.DataContext;


namespace SAS.EventsService.Infrastructure.Persistence.DependencyInjection
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDataContext(configuration)
                .AddRepositories()
                .AddIdentity();

            return services;
        }




        #region Register Identity 

        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {

            // Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();



            return services;
        }
        #endregion Register Identity 

        #region Register Repositories
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {

            return services;

        }


        #endregion Register Repositoryies

        #region Register Data context 
        private static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

          
            return services;

        }

        #endregion Register Data Context 
    }
}
