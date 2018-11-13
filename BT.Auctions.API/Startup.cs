using AutoMapper;
using BT.Auctions.API.Data.Contexts;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Data.Repositories;
using BT.Auctions.API.Helpers;
using BT.Auctions.API.Hubs;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;

namespace BT.Auctions.API
{
    /// <summary>
    /// ASP Net Core Startup Class. Creates and configures the application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the Service Configuration interface on runtime start
        /// </summary>
        /// <value>
        /// The configuration
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// Configures the Services
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddVersionedApiExplorer(options =>
            {
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddAutoMapper(o =>
            {
                o.AllowNullCollections = true;
            });

            services.AddDataProtection();

            services.AddResponseCaching();

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
                {
                    //Ignore reference loops to allow correct propogation of Many to Many relationships
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddApiVersioning(o => { o.ReportApiVersions = true; });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                corsBuilder =>
                {
                    corsBuilder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:55192")
                        .WithOrigins("http://localhost:3000")
                        .WithOrigins("http://localhost:8080")
                        .WithOrigins("https://btauctionswebdev.azurewebsites.net")
                        .WithOrigins("https://btauctionswebtest.azurewebsites.net")
                        .WithOrigins("https://btauctionswebprod.azurewebsites.net")
                        .WithOrigins("https://test-auctionpresentation.barfoot.co.nz")
                        .WithOrigins("https://auctionpresentation.barfoot.co.nz")
                        .AllowCredentials();
                }));

            services.AddSignalR(config =>
            {
                config.EnableDetailedErrors = true;
            });

            services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                options.OperationFilter<SwaggerDefaultValues>();
                options.OperationFilter<SwaggerFormFileFilter>();
                options.SchemaFilter<SwaggerReadOnlyValues>();
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
            });

            services.AddLogging(loggingBuilder => { loggingBuilder.AddSeq(Configuration.GetSection("Seq")); });

            services.Configure<HubMethods>(Configuration.GetSection("HubMethods"));
            services.Configure<ServiceSettings>(Configuration.GetSection("ServiceSettings"));
            services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = long.MaxValue);
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new GoogleTokenValidator(new List<string>
                    {Configuration["GoogleClientId"]}));
            });

            DbContextOptionsBuilder<AuctionsContext> builder = new DbContextOptionsBuilder<AuctionsContext>();

            string connectionString = configuration.GetConnectionString("AuctionsDbConnection");

            builder.UseSqlServer(connectionString);
            //https://stackoverflow.com/questions/48443567/adddbcontext-or-adddbcontextpool
            services.AddDbContext<AuctionsContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            services.AddScoped<IpFilterAttribute>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<IVenueService, VenueService>();
            services.AddScoped<IAuctionSessionRepository, AuctionSessionRepository>();
            services.AddScoped<IAuctionSessionService, AuctionSessionService>();
            services.AddScoped<ILotRepository, LotRepository>();
            services.AddScoped<ILotService, LotService>();
            services.AddScoped<ILotDetailRepository, LotDetailRepository>();
            services.AddScoped<ILotDetailService, LotDetailService>();
            services.AddScoped<IDisplayConfigurationRepository, DisplayConfigurationRepository>();
            services.AddScoped<IDisplayConfigurationService, DisplayConfigurationService>();
            services.AddScoped<IDisplayRepository, DisplayRepository>();
            services.AddScoped<IDisplayService, DisplayService>();
            services.AddScoped<IDisplayGroupRepository, DisplayGroupRepository>();
            services.AddScoped<IDisplayGroupService, DisplayGroupService>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IBidRepository, BidRepository>();
            services.AddScoped<IBidService, BidService>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IMediaService, MediaService>();
        }

        /// <summary>
        /// Configures the specified application.
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="provider">The provider.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");

            loggerFactory.AddAzureWebAppDiagnostics(
                new AzureAppServicesDiagnosticsSettings
                {
                    OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss zzz} [{Level}] {RequestId}-{SourceContext}: {Message}{NewLine}{Exception}"
                }
            );

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                options.RoutePrefix = string.Empty;
                options.InjectStylesheet("/swagger-ui/barfoot-thompson.css");
                options.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("BT.Auctions.API.Resources.Swagger_BT_Index.html");
            });
            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new [] { "Accept-Encoding" };
                await next();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Values}/{action=values}/");
            });
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<PresentationHub>("/hubs/presentationhub");
            });
        }

        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"Barfoot and Thompson Auctions API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Application to help carry out live real estate auctions",
                Contact = new Contact() { Name = "Auction Support", Email = "auction@barfoot.co.nz" }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
