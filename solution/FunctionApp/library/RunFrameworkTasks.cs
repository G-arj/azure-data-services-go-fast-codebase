/*-----------------------------------------------------------------------

 Copyright (c) Microsoft Corporation.
 Licensed under the MIT license.

-----------------------------------------------------------------------*/
using AdsGoFast.GetTaskInstanceJSON;
using AdsGoFast.TaskMetaData;
using Dapper;
using FormatWith;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Http;


namespace AdsGoFast
{
   
    public static class RunFrameworkTasks
    {

        public static dynamic RunFrameworkTasksCore(HttpRequest req, Logging logging)
        {
            short TaskRunnerId = System.Convert.ToInt16(req.Query["TaskRunnerId"]);
            return RunFrameworkTasksCore(TaskRunnerId, logging);
        }

        public static dynamic RunFrameworkTasksCore(short TaskRunnerId, Logging logging)
        {

            TaskMetaDataDatabase TMD = new TaskMetaDataDatabase();
            
            try
            {
                TMD.ExecuteSql(string.Format("Insert into Execution values ('{0}', '{1}', '{2}')", logging.DefaultActivityLogItem.ExecutionUid, DateTimeOffset.Now.ToString("u"), DateTimeOffset.Now.AddYears(999).ToString("u")));

                //Fetch Top # tasks
                List<ADFJsonBaseTask> _Tasks = AdsGoFast.TaskMetaData.TaskInstancesStatic.GetActive_ADFJSON((Guid)logging.DefaultActivityLogItem.ExecutionUid, TaskRunnerId, logging);

                var UtcCurDay = DateTime.UtcNow.ToString("yyyyMMdd");
                foreach (ADFJsonBaseTask _Task in _Tasks)
                {

                    long _TaskInstanceId = _Task.TaskInstanceId;
                    logging.DefaultActivityLogItem.TaskInstanceId = _TaskInstanceId;

                    //TO DO: Update TaskInstance yto UnTried if failed
                    string _pipelinename = _Task.ADFPipeline.ToString();
                    System.Collections.Generic.Dictionary<string, object> _pipelineparams = new System.Collections.Generic.Dictionary<string, object>();

                    logging.LogInformation(string.Format("Executing ADF Pipeline for TaskInstanceId {0} ", _TaskInstanceId.ToString()));
                    //Check Task Type and execute appropriate ADF Pipeline
                    //Todo: Potentially extract switch into metadata

                    if (Shared._ApplicationOptions.TestingOptions.GenerateTaskObjectTestFiles)
                    {
                        string FileFullPath = Shared._ApplicationOptions.TestingOptions.TaskObjectTestFileLocation +  /*UtcCurDay +*/ "/";
                        // Determine whether the directory exists.
                        if (!System.IO.Directory.Exists(FileFullPath))
                        {
                            // Try to create the directory.
                            System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(FileFullPath);
                        }

                        FileFullPath = FileFullPath + _Task.TaskType + "_" + _pipelinename.ToString() + "_" + _Task.TaskMasterId + ".json";
                        System.IO.File.WriteAllText(FileFullPath, _Task.ToString());
                        TMD.LogTaskInstanceCompletion(_TaskInstanceId, (Guid)logging.DefaultActivityLogItem.ExecutionUid, TaskMetaData.BaseTasks.TaskStatus.Complete, System.Guid.Empty, "Complete");
                    }
                    else
                    {
                        try
                        {
                            if (_Task.TaskExecutionType.ToString() == "ADF")
                            {


                                _pipelinename = _pipelinename + "_" + _Task.TaskDatafactoryIR.ToString();                                
                                _pipelineparams.Add("TaskObject", _Task._JsonObjectForADF);

                                if (_pipelinename != "")
                                {
                                    JObject _pipelineresult = ExecutePipeline.ExecutePipelineMethod(_Task.DataFactorySubscriptionId.ToString(), _Task.DataFactoryResourceGroup.ToString(), _Task.DataFactoryName.ToString(), _pipelinename, _pipelineparams, logging);
                                    logging.DefaultActivityLogItem.AdfRunUid = Guid.Parse(_pipelineresult["RunId"].ToString());
                                    TMD.GetSqlConnection().Execute(string.Format(@"
                                            INSERT INTO TaskInstanceExecution (
	                                                        [ExecutionUid]
	                                                        ,[TaskInstanceId]
	                                                        ,[DatafactorySubscriptionUid]
	                                                        ,[DatafactoryResourceGroup]
	                                                        ,[DatafactoryName]
	                                                        ,[PipelineName]
	                                                        ,[AdfRunUid]
	                                                        ,[StartDateTime]
	                                                        ,[Status]
	                                                        ,[Comment]
	                                                        )
                                                        VALUES (
	                                                         @ExecutionUid
	                                                        ,@TaskInstanceId
	                                                        ,@DatafactorySubscriptionUid
	                                                        ,@DatafactoryResourceGroup
	                                                        ,@DatafactoryName
	                                                        ,@PipelineName
	                                                        ,@AdfRunUid
	                                                        ,@StartDateTime
	                                                        ,@Status
	                                                        ,@Comment
	                                        )"), new
                                    {
                                        ExecutionUid = logging.DefaultActivityLogItem.ExecutionUid.ToString(),
                                        TaskInstanceId = _Task.TaskInstanceId,
                                        DatafactorySubscriptionUid = _Task.DataFactorySubscriptionId,
                                        DatafactoryResourceGroup = _Task.DataFactoryResourceGroup,
                                        DatafactoryName = _Task.DataFactoryName,
                                        PipelineName = _pipelineresult["PipelineName"].ToString(),
                                        AdfRunUid = Guid.Parse(_pipelineresult["RunId"].ToString()),
                                        StartDateTime = DateTimeOffset.UtcNow,
                                        Status = _pipelineresult["Status"].ToString(),
                                        Comment = ""

                                    });


                                }
                                //To Do // Batch to make less "chatty"
                                //To Do // Upgrade to stored procedure call
                            }

                            else if (_Task.TaskExecutionType == "AF")
                            {



                                //The "AF" branch is for calling Azure Function Based Tasks that do not require ADF. Calls are made async (just like the ADF calls) and calls are made using "AsyncHttp" requests even though at present the "AF" based Tasks reside in the same function app. This is to "future proof" as it is expected that these AF based tasks will be moved out to a separate function app in the future. 
                                switch (_pipelinename)
                                {
                                    case "AZ-Storage-SAS-Uri-SMTP-Email":
                                        using (var client = new System.Net.Http.HttpClient())
                                        {
                                            //Lets get an access token based on MSI or Service Principal
                                            var secureFunctionAPIURL = string.Format("{0}/api/GetSASUriSendEmailHttpTrigger", Shared._ApplicationOptions.ServiceConnections.CoreFunctionsURL);
                                            var accessToken = Shared._AzureAuthenticationCredentialProvider.GetAzureRestApiToken(secureFunctionAPIURL);

                                            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                                            {
                                                Method = HttpMethod.Post,
                                                RequestUri = new Uri(secureFunctionAPIURL),
                                                Content = new StringContent(_Task.ToString(), System.Text.Encoding.UTF8, "application/json"),
                                                Headers = { { System.Net.HttpRequestHeader.Authorization.ToString(), "Bearer " + accessToken } }
                                            };


                                            //Todo Add some error handling in case function cannot be reached. Note Wait time is there to provide sufficient time to complete post before the HttpClient is disposed.
                                            var HttpTask = client.SendAsync(httpRequestMessage).Wait(3000);

                                        }
                                        break;
                                    case "AZ-Storage-Cache-File-List":
                                        using (var client = new System.Net.Http.HttpClient())
                                        {

                                            //Lets get an access token based on MSI or Service Principal
                                            var secureFunctionAPIURL = string.Format("{0}/api/AZStorageCacheFileListHttpTrigger", Shared._ApplicationOptions.ServiceConnections.CoreFunctionsURL);
                                            var accessToken = Shared._AzureAuthenticationCredentialProvider.GetAzureRestApiToken(secureFunctionAPIURL);

                                            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                                            {
                                                Method = HttpMethod.Post,
                                                RequestUri = new Uri(secureFunctionAPIURL),
                                                Content = new StringContent(_Task.ToString(), System.Text.Encoding.UTF8, "application/json"),
                                                Headers = { { System.Net.HttpRequestHeader.Authorization.ToString(), "Bearer " + accessToken } }
                                            };


                                            //Todo Add some error handling in case function cannot be reached. Note Wait time is there to provide sufficient time to complete post before the HttpClient is disposed.
                                            var HttpTask = client.SendAsync(httpRequestMessage).Wait(3000);

                                        }
                                        break;
                                    case "StartAndStopVMs":
                                        using (var client = new System.Net.Http.HttpClient())
                                        {
                                            //Lets get an access token based on MSI or Service Principal                                            
                                            var accessToken = GetSecureFunctionToken(_pipelinename);

                                            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                                            {
                                                Method = HttpMethod.Post,
                                                RequestUri = new Uri(GetSecureFunctionURI(_pipelinename)),
                                                Content = new StringContent(_Task.ToString(), System.Text.Encoding.UTF8, "application/json"),
                                                Headers = { { System.Net.HttpRequestHeader.Authorization.ToString(), "Bearer " + accessToken } }
                                            };

                                            //Todo Add some error handling in case function cannot be reached. Note Wait time is there to provide sufficient time to complete post before the HttpClient is disposed.
                                            var HttpTask = client.SendAsync(httpRequestMessage).Wait(3000);

                                        }
                                        break;
                                    case "Cache-File-List-To-Email-Alert":
                                        using (var client = new System.Net.Http.HttpClient())
                                        {
                                            SendAlert(_Task, logging);
                                        }
                                        break;

                                    default:
                                        var msg = $"Could not find execution path for Task Type of {_pipelinename} and Execution Type of {_Task.TaskExecutionType.ToString()}";
                                        logging.LogErrors(new Exception(msg));
                                        TMD.LogTaskInstanceCompletion((Int64)_TaskInstanceId, (System.Guid)logging.DefaultActivityLogItem.ExecutionUid, BaseTasks.TaskStatus.FailedNoRetry, Guid.Empty, (String)msg);
                                        break;
                                }
                                //To Do // Batch to make less "chatty"
                                //To Do // Upgrade to stored procedure call

                            }
                        }
                        catch (Exception TaskException)
                        {
                            logging.LogErrors(TaskException);
                            TMD.LogTaskInstanceCompletion((Int64)_TaskInstanceId, (System.Guid)logging.DefaultActivityLogItem.ExecutionUid, BaseTasks.TaskStatus.FailedNoRetry, Guid.Empty, (String)"Runner failed to execute task.");
                        }

                    }
                }
            }
            catch (Exception RunnerException)
            {
                //Set Runner back to Idle
                TMD.ExecuteSql(string.Format("exec [dbo].[UpdFrameworkTaskRunner] {0}", TaskRunnerId));
                logging.LogErrors(RunnerException);
                //log and re-throw the error
                throw RunnerException;
            }
            //Set Runner back to Idle
            TMD.ExecuteSql(string.Format("exec [dbo].[UpdFrameworkTaskRunner] {0}", TaskRunnerId));

            //Return success
            JObject Root = new JObject
            {
                ["Succeeded"] = true
            };

            return Root;

        }

        private static string GetSecureFunctionURI(string FunctionName)
        {
            return string.Format("{0}/api/{1}", Shared._ApplicationOptions.ServiceConnections.CoreFunctionsURL, FunctionName);
        }
        private static string GetSecureFunctionToken(string FunctionName)
        {
            string ret = "";
            string secureFunctionAPIURL = Shared._ApplicationOptions.ServiceConnections.CoreFunctionsURL;
            if (!secureFunctionAPIURL.Contains("localhost"))
            {
                ret = Shared._AzureAuthenticationCredentialProvider.GetAzureRestApiToken(secureFunctionAPIURL);
            }

            return ret;
        }


        public static void SendAlert(ADFJsonBaseTask task, Logging logging)
        {
            TaskMetaDataDatabase TMD = new TaskMetaDataDatabase();
            try
            {
                var target = (JObject)task.TargetSystemJSON;
                var source = (JObject)task.SourceSystemJSON;
                if (target != null)
                {
                    if ((JArray)target["Alerts"] != null)
                    {
                        foreach (JObject Alert in (JArray)target["Alerts"])
                        {
                            //Only Send out for Operator Level Alerts
                            //if (Alert["AlertCategory"].ToString() == "Task Specific Operator Alert")
                            {
                                //Get Plain Text and Email Subject from Template Files 
                                System.Collections.Generic.Dictionary<string, string> Params = new System.Collections.Generic.Dictionary<string, string>();
                                Params.Add("Source.RelativePath", source["RelativePath"].ToString());
                                Params.Add("Source.DataFileName", source["DataFileName"].ToString());
                                Params.Add("Alert.EmailRecepientName", Alert["EmailRecepientName"].ToString());

                                string _plainTextContent = System.IO.File.ReadAllText(System.IO.Path.Combine(Shared._ApplicationBasePath, Shared._ApplicationOptions.LocalPaths.HTMLTemplateLocation, Alert["EmailTemplateFileName"].ToString() + ".txt"));
                                _plainTextContent = _plainTextContent.FormatWith(Params, MissingKeyBehaviour.ThrowException, null, '{', '}');

                                string _htmlContent = System.IO.File.ReadAllText(System.IO.Path.Combine(Shared._ApplicationBasePath, Shared._ApplicationOptions.LocalPaths.HTMLTemplateLocation, Alert["EmailTemplateFileName"].ToString() + ".html"));
                                _htmlContent = _htmlContent.FormatWith(Params, MissingKeyBehaviour.ThrowException, null, '{', '}');

                                var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                                var client = new SendGridClient(apiKey);
                                var msg = new SendGridMessage()
                                {
                                    From = new EmailAddress(source["SenderEmail"].ToString(), source["SenderDescription"].ToString()),
                                    Subject = Alert["EmailSubject"].ToString(),
                                    PlainTextContent = _plainTextContent,
                                    HtmlContent = _htmlContent
                                };
                                msg.AddTo(new EmailAddress(Alert["EmailRecepient"].ToString(), Alert["EmailRecepientName"].ToString()));
                                var res = client.SendEmailAsync(msg).Result;
                            }
                        }
                    }

                    TMD.LogTaskInstanceCompletion(System.Convert.ToInt64(task.TaskInstanceId), (System.Guid)logging.DefaultActivityLogItem.ExecutionUid, TaskMetaData.BaseTasks.TaskStatus.Complete, System.Guid.Empty, "");
                }
            }
            catch (Exception e)
            {
                logging.LogErrors(e);
                TMD.LogTaskInstanceCompletion(System.Convert.ToInt64(task.TaskInstanceId), (System.Guid)logging.DefaultActivityLogItem.ExecutionUid, TaskMetaData.BaseTasks.TaskStatus.FailedNoRetry, System.Guid.Empty, "Failed to send email");
            }




        }
    }

}







