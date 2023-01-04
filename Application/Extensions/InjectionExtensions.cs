using Application.Interfaces;
using Application.Services;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            //services.AddFluentValidationAutoValidation();
            services.AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(w => !w.IsDynamic));
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IArticleApplication, ArticleApplication>();
            services.AddScoped<IMuseumApplication, MuseumApplication>();

            return services;
        }
    }
}
