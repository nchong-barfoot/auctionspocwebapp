using BT.Auctions.PoC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BT.Auctions.PoC.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register application services
            // Examples:
            // services.AddScoped<ICharacterRepository, CharacterRepository>();
            // services.AddTransient<IOperationTransient, Operation>();
            // services.AddScoped<IOperationScoped, Operation>();
            // services.AddSingleton<IOperationSingleton, Operation>();
            // services.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));
            // services.AddTransient<OperationService, OperationService>();
            services.AddScoped<IVideoService, VideoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
