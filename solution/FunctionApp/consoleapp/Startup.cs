using AdsGoFast.Models;
using AdsGoFast.Models.Options;
using AdsGoFast.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;

namespace AdsGoFast.Console
{
public class Startup 
    {
        

        public IServiceCollection Services => services;

        // access the built configuration
        public IConfiguration Configuration => config;

        private readonly IConfiguration config;
        private IServiceCollection services;

        public Startup()
        {

            config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();
        }


        public void ConfigureServices(IServiceCollection _services)
        {
            services = _services;
            services.Configure<AuthOptions>(config.GetSection("AzureAdAuth"));
            services.Configure<ApplicationOptions>(config.GetSection("ApplicationOptions"));
            services.Configure<DownstreamAuthOptionsDirect>(config.GetSection("AzureAdAzureServicesDirect"));
            services.Configure<DownstreamAuthOptionsViaAppReg>(config.GetSection("AzureAdAzureServicesViaAppReg"));

            var AppOptions = config.GetSection("ApplicationOptions").Get<ApplicationOptions>();
            var DownstreamAuthOptionsDirect = config.GetSection("AzureAdAzureServicesDirect").Get<DownstreamAuthOptionsDirect>();
            Shared._ApplicationBasePath = Environment.CurrentDirectory;
            Shared._ApplicationOptions = AppOptions;
            Shared._DownstreamAuthOptionsDirect = DownstreamAuthOptionsDirect;
            Shared._AzureAuthenticationCredentialProvider = new AzureAuthenticationCredentialProvider(Options.Create(AppOptions), DownstreamAuthOptionsDirect);

            //builder.Services.AddSingleton<IConfiguration>(config); 
            services.AddSingleton<ISecurityAccessProvider>((provider) =>
            {
                var authOptions = provider.GetService<IOptions<DownstreamAuthOptionsViaAppReg>>();
                var appOptions = provider.GetService<IOptions<ApplicationOptions>>();
                return new SecurityAccessProvider(authOptions, appOptions);
            });

            //Inject Http Client for chained calling of core functions
            services.AddHttpClient("CoreFunctions", async (s, c) =>
            {
                var downstreamAuthOptionsViaAppReg = s.GetService<IOptions<DownstreamAuthOptionsViaAppReg>>();
                var appOptions = s.GetService<IOptions<ApplicationOptions>>();
                var authProvider = new AzureAuthenticationCredentialProvider(appOptions, downstreamAuthOptionsViaAppReg.Value);
                var token = authProvider.GetAzureRestApiToken(downstreamAuthOptionsViaAppReg.Value.Audience);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));  //Set lifetime to five minutes

            //Inject Context for chained calling of core functions
            services.AddSingleton<ICoreFunctionsContext, CoreFunctionsContext>();

            services.AddSingleton<IAppInsightsContext, AppInsightsContext>();

            services.AddHttpClient("AppInsights", async (s, c) =>
            {
                var downstreamAuthOptions = s.GetService<IOptions<DownstreamAuthOptionsDirect>>();
                var appOptions = s.GetService<IOptions<ApplicationOptions>>();
                var authProvider = new AzureAuthenticationCredentialProvider(appOptions, downstreamAuthOptions.Value);
                var token = authProvider.GetAzureRestApiToken("https://api.applicationinsights.io");
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));  //Set lifetime to five minutes          

            services.AddHttpClient("LogAnalytics", async (s, c) =>
            {
                var downstreamAuthOptions = s.GetService<IOptions<DownstreamAuthOptionsDirect>>();
                var appOptions = s.GetService<IOptions<ApplicationOptions>>();
                var authProvider = new AzureAuthenticationCredentialProvider(appOptions, downstreamAuthOptions.Value);
                var token = authProvider.GetAzureRestApiToken("https://api.loganalytics.io");
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));  //Set lifetime to five minutes

            services.AddSingleton<ILogAnalyticsContext, LogAnalyticsContext>();

            services.AddHttpClient("TaskMetaDataDatabase", async (s, c) =>
            {
                var downstreamAuthOptions = s.GetService<IOptions<DownstreamAuthOptionsDirect>>();
                var appOptions = s.GetService<IOptions<ApplicationOptions>>();
                var authProvider = new AzureAuthenticationCredentialProvider(appOptions, downstreamAuthOptions.Value);
                var token = authProvider.GetAzureRestApiToken("https://database.windows.net/");
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));  //Set lifetime to five minutes


        }

    }





}