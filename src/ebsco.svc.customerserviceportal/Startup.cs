using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using MediaServerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReallySimpleFeatureToggle.EBSCO.Client;
using Swashbuckle.AspNetCore.Swagger;
using WorkItemTracker;

namespace ebsco.svc.customerserviceportal
{
    public class Startup
    {
        public Func<ApplicationSettings> ApplicationSettingsFactory { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Configuration Setup
            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration); //< Grab Environment Vars
            services.AddTransient(s => s.GetService<IOptions<ApplicationSettings>>().Value);
            #endregion

            #region Build an intermediate service provider
            var isp = services.BuildServiceProvider();
            ApplicationSettingsFactory = () => isp.GetService<ApplicationSettings>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateFormatString = "dddd, MMMM dd, yyyy";
                });
            #endregion

            #region Register JWT Authorization
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ApplicationSettingsFactory().EBSCONET_JWT_Secret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Clock skew compensates for server time drift.
                        // We recommend 5 minutes or less:
                        ClockSkew = TimeSpan.FromMinutes(5),
                        // Specify the key used to sign the token:
                        ValidateAudience = false,
                        //ValidAudience = "http://www.ebsconet.com",

                        ValidateIssuer = true,
                        ValidIssuer = "http://www.ebsconet.com",

                        IssuerSigningKey = signingKey,
                        ValidateIssuerSigningKey = true,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                    };
                });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                options.AddPolicy("CustomerPortalAccess", policy => policy.RequireClaim("CustomerPortal"));
            });
            #endregion

            #region Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "EBSCO Customer Service Portal",
                        Version = "v1",
                        Description = "Service to provide EBSCO Work Item Tracker data."
                    });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region Register all services and dependencies here
            services.AddSingleton<IFeatureFlagService>(new FeatureFlagService(
                ApplicationSettingsFactory().FeatureFlag_Url,
                ApplicationSettingsFactory().FeatureFlag_Prefix,
                ApplicationSettingsFactory().FeatureFlag_Tenant,
                ApplicationSettingsFactory().FeatureFlag_CacheTime));

            services.AddTransient<IWorkItemTrackerServiceRepository, WorkItemTrackerServiceRepository>();
            services.AddTransient<IMediaServerServiceRepository, MediaServerServiceRepository>();
            services.AddTransient<ITranslationsService, TranslationService>();

            var netTcpBinding = new NetTcpBinding
            {
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue,
                Security = new NetTcpSecurity
                {
                    Mode = SecurityMode.None
                }
            };

            services.AddTransient<Func<IWorkItemService>>(cont => () =>
            {
                return new WorkItemServiceClient(
                    new NetTcpBinding
                    {
                        MaxBufferSize = int.MaxValue,
                        ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                        MaxReceivedMessageSize = int.MaxValue,
                        Security = new NetTcpSecurity
                        {
                            Mode = SecurityMode.None
                        }
                    },
                    new EndpointAddress(Configuration.Get<ApplicationSettings>().WorkItemTrackerService_Url));
            });

            services.AddTransient<Func<IMediaServerService>>(cont => () =>
            {
                return new MediaServerServiceClient(
                    new NetTcpBinding
                    {
                        MaxBufferSize = int.MaxValue,
                        ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                        MaxReceivedMessageSize = int.MaxValue,
                        TransferMode = TransferMode.Streamed,
                        Security = new NetTcpSecurity
                        {
                            Mode = SecurityMode.None
                        }
                    },
                    new EndpointAddress(Configuration.Get<ApplicationSettings>().MediaServerService_URL));
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            #region Fault Handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appbuilder =>
                {
                    appbuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Please try again later.");
                    });
                });
            }
            #endregion

            #region Swagger Implementation
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            //Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EBSCO Customer Service Portal API v1");
                c.RoutePrefix = "";
            });
            #endregion

            #region Set Default Culture
            //To-Do - Set to Culture being set in JWT
            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            #endregion

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            app.UseMvc();

            app.UseStaticFiles();
        }
    }
}

