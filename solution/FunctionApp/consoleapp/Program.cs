using AdsGoFast.GetTaskInstanceJSON;
using AdsGoFast.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using static System.Net.WebRequestMethods;
using Dapper;

namespace AdsGoFast.Console
{
    class Program
    {        

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            // Startup.cs finally :)
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            services.AddTransient<App>();

            services.AddLogging(builder => builder.AddConsole().AddFilter(level => level >= LogLevel.Information));


            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {                
                var app = serviceProvider.GetService<App>();
                app.Run();
            }

        }
        
    }

    public class App
    {
        private readonly IOptions<ApplicationOptions> _appOptions;
        private readonly ILogger<App> _logger;
        private Logging.ActivityLogItem _activityLogItem;
        private Logging _logHelper = new Logging();
        public App(IOptions<ApplicationOptions> appOptions,
            ILogger<App> logger)
        {
            _appOptions = appOptions;
            _logger = logger;
        }

        public void Run()
        {            
             _activityLogItem = new Logging.ActivityLogItem
            {
                LogSource = "AF",
                ExecutionUid = Guid.NewGuid()
            };
            _logHelper.InitializeLog(_logger, _activityLogItem);


            //DebugPrepareFrameworkTasks();
            //_activityLogItem.ExecutionUid = Guid.NewGuid();
            //DebugRunFrameworkTasks();

            InsertTestTasksIntoDb();




        }



        public void InsertTestTasksIntoDb()
        {
            // Test_GetSQLCreateStatementFromSchema(LogHelper);

            var testTaskInstances = GetTests();
            foreach (var testTaskInstance in testTaskInstances)
            {
                JObject processedTaskObject = null;
                try
                {
                    var T = new ADFJsonBaseTask(testTaskInstance, _logHelper);


                    var tmdb = new TaskMetaDataDatabase();
                    var con = tmdb.GetSqlConnection();
                    var parameters = new
                    {
                        TaskMasterId = T.TaskMasterId * -1,
                        TaskMasterName = T.ADFPipeline + T.TaskMasterId.ToString(),
                        TaskTypeId = T.TaskTypeId,
                        TaskGroupId = 1,
                        ScheduleMasterId = 4,
                        SourceSystemId = T.SourceSystemId,
                        TargetSystemId = T.TargetSystemId,
                        DegreeOfCopyParallelism = T.DegreeOfCopyParallelism,
                        AllowMultipleActiveInstances = 0,
                        TaskDatafactoryIR = "IRA",
                        TaskMasterJSON = T.TaskMasterJson,
                        ActiveYN = 1,
                        DependencyChainTag = "",
                        DataFactoryId = T.DataFactoryId
                    };
                    var sql = @"
                    SET IDENTITY_INSERT [dbo].[TaskMaster] ON;
                    insert into [dbo].[TaskMaster]
                    (
                        [TaskMasterId]                          ,
                        [TaskMasterName]                        ,
                        [TaskTypeId]                            ,
                        [TaskGroupId]                           ,
                        [ScheduleMasterId]                      ,
                        [SourceSystemId]                        ,
                        [TargetSystemId]                        ,
                        [DegreeOfCopyParallelism]               ,
                        [AllowMultipleActiveInstances]          ,
                        [TaskDatafactoryIR]                     ,
                        [TaskMasterJSON]                        ,
                        [ActiveYN]                              ,
                        [DependencyChainTag]                    ,
                        [DataFactoryId]                         
                    )
                    select 
                        @TaskMasterId                          ,
                        @TaskMasterName                        ,
                        @TaskTypeId                            ,
                        @TaskGroupId                           ,
                        @ScheduleMasterId                      ,
                        @SourceSystemId                        ,
                        @TargetSystemId                        ,
                        @DegreeOfCopyParallelism               ,
                        @AllowMultipleActiveInstances          ,
                        @TaskDatafactoryIR                     ,
                        @TaskMasterJSON                        ,
                        @ActiveYN                              ,
                        @DependencyChainTag                    ,
                        @DataFactoryId;  
                    SET IDENTITY_INSERT [dbo].[TaskMaster] OFF;";
                    var result = con.Query(sql, parameters);

                    //T.CreateJsonObjectForAdf(executionId);
                    //processedTaskObject = T.ProcessRoot(_taskTypeMappingProvider, _schemasProvider);
                }
                catch (Exception e)
                {
                    _logHelper.LogErrors(e);
                }
                string FileFullPath = "../../../UnitTestResults/Todo/";
                // Determine whether the directory exists.
                if (!Directory.Exists(FileFullPath))
                {
                    // Try to create the directory.
                    var di = Directory.CreateDirectory(FileFullPath);
                }

                if (processedTaskObject != null)
                {
                    JObject obj = new JObject();
                    obj["TaskObject"] = processedTaskObject;

                    FileFullPath = $"{FileFullPath}{testTaskInstance.TaskType}_{testTaskInstance.ADFPipeline}_{testTaskInstance.TaskMasterId}.json";
                    System.IO.File.WriteAllText(FileFullPath, JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
            }

        }
   

        public void DebugPrepareFrameworkTasks()
        {
            AdsGoFast.PrepareFrameworkTasks.PrepareFrameworkTasksCore(_logHelper);
        }


        public void DebugRunFrameworkTasks()
        {
            AdsGoFast.RunFrameworkTasks.RunFrameworkTasksCore(1, _logHelper);
        }

        /// <summary>
        /// 
        /// </summary>
        public void GenerateUnitTestResults()
        { 
             // Test_GetSQLCreateStatementFromSchema(LogHelper);

            var tis = GetTests();
                
            //Instantiate the Collections that contain the JSON Schemas 
            using TaskTypeMappings ttm = new TaskTypeMappings();
            using SourceAndTargetSystem_JsonSchemas system_schemas = new SourceAndTargetSystem_JsonSchemas();

            //Set up table to Store Invalid Task Instance Objects
            DataTable InvalidTIs = new DataTable();
            InvalidTIs.Columns.Add("ExecutionUid", System.Type.GetType("System.Guid"));
            InvalidTIs.Columns.Add("TaskInstanceId", System.Type.GetType("System.Int64"));
            InvalidTIs.Columns.Add("LastExecutionComment", System.Type.GetType("System.String"));

            foreach (var TBase in tis)
            {
                List<ADFJsonBaseTask> TIsJson = new List<ADFJsonBaseTask>();
                try
                {
                    AdsGoFast.TaskMetaData.TaskInstancesStatic.GetActive_ADFJSON_ProcessTask(TBase, TIsJson, InvalidTIs, (Guid)_activityLogItem.ExecutionUid, ttm, system_schemas, _logHelper);
                }
                catch (Exception e) {
                    _logHelper.LogErrors(e);
                } 
                string FileFullPath = "../../../UnitTestResults/Todo/";
                // Determine whether the directory exists.
                if (!System.IO.Directory.Exists(FileFullPath))
                {
                    // Try to create the directory.
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(FileFullPath);
                }

                if (TIsJson.Count > 0)
                {
                    JObject obj = new JObject();
                    obj["TaskObject"] = TIsJson[0]._JsonObjectForADF;

                    FileFullPath = FileFullPath + TBase.TaskType.ToString() + "_" + TBase.ADFPipeline.ToString() + "_" + TBase.TaskMasterId.ToString() + ".json";
                    System.IO.File.WriteAllText(FileFullPath, Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }));
                }
            }
        
        }

        public IEnumerable<AdsGoFast.GetTaskInstanceJSON.BaseTask> GetTests()
        {
            string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "..\\..\\..\\..\\UnitTests\\tests.json";
            string tests = "";
            using (var r = new StreamReader(_filePath))
            {
                tests = r.ReadToEnd();
            }
            
            var ts = JsonConvert.DeserializeObject<List<AdsGoFast.GetTaskInstanceJSON.BaseTask>>(tests);
            return ts;
        }


        public void Test_GetSQLCreateStatementFromSchema(AdsGoFast.Logging _logging)
        {
            string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "..\\..\\..\\..\\UnitTests\\GetSQLCreateStatementFromSchema.json";
            string tests = "";
            using (var r = new StreamReader(_filePath))
            {
                tests = r.ReadToEnd();
            }
            

            AdsGoFast.GetSQLCreateStatementFromSchemaCore.Execute(JObject.Parse(tests),_logging);

        }
  
    }
}
