/*-----------------------------------------------------------------------

 Copyright (c) Microsoft Corporation.
 Licensed under the MIT license.

-----------------------------------------------------------------------*/
using AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.AZ_SQL_StoredProcedure;
using AdsGoFast.SqlServer;
using AdsGoFast.TaskMetaData;
using Dapper;
using FormatWith;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.ContainerRegistry.Fluent;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using AdsGoFast.Services;
using AdsGoFast.Models;
using Microsoft.Extensions.Options;
using AdsGoFast.Models.Options;

namespace AdsGoFast
{

    public class RunFrameworkTasksHttpTrigger
    {
        private readonly ISecurityAccessProvider _sap;
        public RunFrameworkTasksHttpTrigger(ISecurityAccessProvider sap)
        {
            _sap = sap;
        }

        [FunctionName("RunFrameworkTasksHttpTrigger")]
        public  async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log, ExecutionContext context, System.Security.Claims.ClaimsPrincipal principal)
        {
            bool IsAuthorised = _sap.IsAuthorised(req, log);
            if (IsAuthorised)
            {
                Guid ExecutionId = context.InvocationId;
                using FrameworkRunner FR = new FrameworkRunner(log, ExecutionId);

                FrameworkRunner.FrameworkRunnerWorkerWithHttpRequest worker = RunFrameworkTasks.RunFrameworkTasksCore;
                FrameworkRunner.FrameworkRunnerResult result = FR.Invoke(req, "RunFrameworkTasksHttpTrigger", worker);
                if (result.Succeeded)
                {
                    return new OkObjectResult(JObject.Parse(result.ReturnObject));
                }
                else
                {
                    return new BadRequestObjectResult(new { Error = "Execution Failed...." });
                }
            }
            else
            {
                log.LogWarning("User is not authorised to call RunFrameworkTasksHttpTrigger.");
                TaskMetaDataDatabase TMD = new TaskMetaDataDatabase();
                short TaskRunnerId = System.Convert.ToInt16(req.Query["TaskRunnerId"]);
                TMD.ExecuteSql(string.Format("exec [dbo].[UpdFrameworkTaskRunner] {0}", TaskRunnerId));
                return new BadRequestObjectResult(new { Error = "User is not authorised to call this API...." });
            }
        }
    }

    public  class RunFrameworkTasksTimerTrigger
    {
        private readonly ICoreFunctionsContext _functionscontext;

        private readonly IOptions<ApplicationOptions> _appOptions;
        public RunFrameworkTasksTimerTrigger(ICoreFunctionsContext functionsContext, IOptions<ApplicationOptions> appOptions)
        {
            _functionscontext = functionsContext;
            _appOptions = appOptions;
        }

        /// <summary>
        /// 
        /// 
        ///             "0 */5 * * * *" once every five minutes
        ///              "0 0 * * * *"   once at the top of every hour
        ///              "0 0 */2 * * *" once every two hours
        ///              "0 0 9-17 * * *"    once every hour from 9 AM to 5 PM
        ///              "0 30 9 * * *"  at 9:30 AM every day
        ///              "0 30 9 * * 1-5"    at 9:30 AM every weekday
        ///              "0 30 9 * Jan Mon"  at 9:30 AM every Monday in January
        ///
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        [FunctionName("RunFrameworkTasksTimerTrigger")]         
        public  async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation("FunctionAppDirectory:" + context.FunctionAppDirectory);
            if (_appOptions.Value.TimerTriggers.EnableRunFrameworkTasks)
            {
                TaskMetaDataDatabase TMD = new TaskMetaDataDatabase();
                var client = _functionscontext.httpClient.CreateClient(_functionscontext.httpClientName);
                
                    using (SqlConnection _con = TMD.GetSqlConnection())
                    {
                        var ftrs = _con.QueryWithRetry("Exec dbo.GetFrameworkTaskRunners");

                        foreach (var ftr in ftrs)
                        {
                            int TaskRunnerId = ((dynamic)ftr).TaskRunnerId;

                            try
                            {

                                //Lets get an access token based on MSI or Service Principal
                                var secureFunctionAPIURL = string.Format("{0}/api/RunFrameworkTasksHttpTrigger?TaskRunnerId={1}", Shared._ApplicationOptions.ServiceConnections.CoreFunctionsURL, TaskRunnerId.ToString());
                                //var accessToken = Shared.Azure.AzureSDK.GetAzureRestApiToken(Shared.GlobalConfigs.GetStringConfig("AzureFunctionURL"));

                                using HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                                {
                                    Method = HttpMethod.Get,
                                    RequestUri = new Uri(secureFunctionAPIURL)//,
                                    //Headers = { { System.Net.HttpRequestHeader.Authorization.ToString(), "Bearer " + accessToken } }
                                };
                                
                                //Todo Add some error handling in case function cannot be reached. Note Wait time is there to provide sufficient time to complete post before the HttpClient is disposed.
                                var HttpTask = client.SendAsync(httpRequestMessage).Wait(3000);
                            
                        }
                            catch (Exception e)
                            {
                                _con.ExecuteWithRetry($"[dbo].[UpdFrameworkTaskRunner] {TaskRunnerId.ToString()}");
                                throw e;
                            }
                        }
                        
                    }                   
                
            }
            
        }

    }

   
}







