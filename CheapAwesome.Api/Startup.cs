using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheapAwesome.Core.Common.Interface;
using CheapAwesome.Core.Services;
using CheapAwesome.Supplier.BargainsForCouples;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;

namespace CheapAwesome.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CheapAwesome API",
                    Description = "Cheap Awesome travel",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Cheap Awesome Travel",
                        Email = "info@cheapawesome.travel",
                        Url = new Uri("http://cheapawesome.travel/"),
                    }
                });
            });

            // Dependency Injection
            services.AddTransient<IHotelAvailabilityService, HotelAvailabilityService>();

            Random jitterer = new Random();
            var retryPolicy = HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .WaitAndRetryAsync(5,
                      retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))  // exponential back-off: 2, 4, 8 etc
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)) // plus some jitter: up to 1 second
                  );

            var breakerPolicy = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .AdvancedCircuitBreakerAsync(
                        failureThreshold: 0.5,
                        samplingDuration: TimeSpan.FromSeconds(5),
                        minimumThroughput: 20,
                        durationOfBreak: TimeSpan.FromSeconds(30));

            //TODO : Register all implementations of IHotelAvailabilitySupplier dynamically
            services.AddHttpClient<IHotelAvailabilitySupplier, BargainsForCouplesHotelAvailabilityService>()
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(breakerPolicy);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // SWAGGER
            app.UseSwagger();

            // Swagger endpoint config
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CheapAwesome API v1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
