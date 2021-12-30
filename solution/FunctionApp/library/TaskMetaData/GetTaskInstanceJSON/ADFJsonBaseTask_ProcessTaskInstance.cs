/*-----------------------------------------------------------------------

 Copyright (c) Microsoft Corporation.
 Licensed under the MIT license.

-----------------------------------------------------------------------*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdsGoFast.GetTaskInstanceJSON
{
    partial class ADFJsonBaseTask : BaseTask
    {

        public void ProcessTaskInstance(TaskTypeMappings ttm)
        {
            //Validate TaskInstance based on JSON Schema
            var mapping = ttm.GetMapping(SourceSystemType, TargetSystemType, _TaskMasterJsonSource["Type"].ToString(), _TaskMasterJsonTarget["Type"].ToString(), TaskDatafactoryIR, TaskExecutionType, TaskTypeId);
            string mapping_schema = mapping.TaskInstanceJsonSchema;
            if (mapping_schema != null)
            {
                _TaskIsValid = Shared.JsonHelpers.ValidateJsonUsingSchema(logging, mapping_schema, this.TaskInstanceJson, "Failed to validate TaskInstance JSON for TaskTypeMapping: " + mapping.MappingName + ". ");
            }

            if (_TaskIsValid)
            {
                ProcessTaskInstance_Default();
            }
        }

        /// <summary>
        /// Default Method which merges Source & Target attributes on TaskInstanceJson with existing Source and Target Attributes on TaskObject payload.
        /// </summary>

        public void ProcessTaskInstance_Default()
        {

            JObject Source = (JObject)_JsonObjectForADF["Source"];
            JObject Target = (JObject)_JsonObjectForADF["Target"];
            JObject Instance = new JObject();

            Instance.Merge(_TaskInstanceJson, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            Source["Instance"] = Instance;
            Target["Instance"] = Instance;


        }
    }
}
