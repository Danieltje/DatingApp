using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    // when you create an extension method the class needs to be static
    // static here means we do not need to create a new instance of this class in order to use it
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<PresenceTracker>();

            /* A way to strongly-type our configuration settings for Cloudinary */
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // for HTTP requests it is advised to use AddScoped
            // testing is the main reason we implement an Interface (here). It would work if you leave out the Interface
            // AddScoped is scoped to the lifetime of the HTTP request in this case. When the request is finished the service is closed
            services.AddScoped<ITokenService, TokenService>();
            
            services.AddScoped<IPhotoService, PhotoService>();

            services.AddScoped<LogUserActivity>();

            // add a service for our UserRepository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            // service for our AutoMapperProfile
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}