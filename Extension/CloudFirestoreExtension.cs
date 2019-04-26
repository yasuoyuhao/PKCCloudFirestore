using CloudFirestore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFirestore.Extension
{
    public static class CloudFirestoreExtension
    {
        /// <summary>
        /// add CloudFirestore
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddKTCloudFirestore(this IServiceCollection services)
        {
            services.AddSingleton<CloudFirestoreService>();
            return services;
        }
    }
}
