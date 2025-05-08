
using FluentValidation;
using FSAKEB.Application.Extensions.FluentValidationRules;
using Hangfire;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Infrastructure.Context;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace FSAKEB.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddSingleton<ILoggerProvider, NLogLoggerProvider>();

            services.AddCors(opt => opt.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        //.AllowCredentials()
                        ;
            }));

            //services.AddHangfire((sp, config) =>
            //{
            //    config.UseSqlServerStorage(sp.GetRequiredService<IConfiguration>().GetConnectionString("AppConnection"));
            //});

            //services.AddHangfireServer();

            services.AddDbContextPool<ExamBankContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ExamBankConnection"));
            });

            services.AddAutoMapper();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #region Validators
            services.AddScoped<IValidator<UpdateUser>, UserUpdateRequestValidator>();
            services.AddScoped<IValidator<UserCreateDTO>, UserCreateRequestValidator>();
            services.AddScoped<IValidator<QuestionTypeCreateDto>, AddQuestionTypeValidator>();
            services.AddScoped<IValidator<AddSingleQuestionRequest>, QuestionAddRequestValidator>();
            #endregion

            return services;
        }
    }
}
