using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using QChat.Application.Behaviors;
using System.Reflection;

namespace QChat.Application;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAuthentication(config =>
        {
            config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(option =>
        {
            option.LoginPath = new PathString("/Account");
            option.LogoutPath = new PathString("/Logout");
            option.ExpireTimeSpan = TimeSpan.FromDays(30);
            option.AccessDeniedPath = "/Account";
        });
        return services;
    }
}
