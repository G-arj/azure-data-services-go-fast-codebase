using AdsGoFast.Models.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

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

        public App(IOptions<ApplicationOptions> appOptions,
            ILogger<App> logger)
        {
            _appOptions = appOptions;
            _logger = logger;
        }

        public void Run()
        {           
            var LogHelper = new Logging();
            Logging.ActivityLogItem activityLogItem = new Logging.ActivityLogItem
            {
                LogSource = "AF",
                ExecutionUid = Guid.NewGuid()
            };
            LogHelper.InitializeLog(_logger, activityLogItem);

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
                JArray TIsJson = new JArray();
                try
                {
                    AdsGoFast.TaskMetaData.TaskInstancesStatic.GetActive_ADFJSON_ProcessTask(TBase, TIsJson, InvalidTIs, (Guid)activityLogItem.ExecutionUid, ttm, system_schemas, LogHelper);
                }
                catch { 
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
                    obj["TaskObject"] = TIsJson[0];

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
  
    }
}
