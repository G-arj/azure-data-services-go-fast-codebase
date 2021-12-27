/*-----------------------------------------------------------------------

 Copyright (c) Microsoft Corporation.
 Licensed under the MIT license.

-----------------------------------------------------------------------*/
using AdsGoFast.Models.Options;
using AdsGoFast.SqlServer;
using AdsGoFast.TaskMetaData;
using Cronos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AdsGoFast
{

    public class PrepareFrameworkTasksTimerTrigger
    {
        private readonly IOptions<ApplicationOptions> _appOptions;
        public PrepareFrameworkTasksTimerTrigger(IOptions<ApplicationOptions> appOptions)
        {
            _appOptions = appOptions;
        }

        [FunctionName("PrepareFrameworkTasksTimerTrigger")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            Guid ExecutionId = context.InvocationId;
            if (_appOptions.Value.TimerTriggers.EnablePrepareFrameworkTasks)
            {
                using (FrameworkRunner FR = new FrameworkRunner(log, ExecutionId))
                {
                    FrameworkRunner.FrameworkRunnerWorker worker = PrepareFrameworkTasks.PrepareFrameworkTasksCore;
                    FrameworkRunner.FrameworkRunnerResult result = FR.Invoke("PrepareFrameworkTasksHttpTrigger", worker);
                }
            }
        }
    }

  
}







