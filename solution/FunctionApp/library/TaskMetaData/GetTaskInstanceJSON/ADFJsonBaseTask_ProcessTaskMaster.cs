/*-----------------------------------------------------------------------

 Copyright (c) Microsoft Corporation.
 Licensed under the MIT license.

-----------------------------------------------------------------------*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using AdsGoFast.GetTaskInstanceJSON.TaskMasterJson;
using System.Reflection;

namespace AdsGoFast.GetTaskInstanceJSON
{
    partial class ADFJsonBaseTask : BaseTask
    {
        public void ProcessTaskMaster(TaskTypeMappings ttm)
        {

            //Validate TaskmasterJson based on JSON Schema
            var mapping = ttm.GetMapping(SourceSystemType, TargetSystemType, _TaskMasterJsonSource["Type"].ToString(), _TaskMasterJsonTarget["Type"].ToString(), TaskDatafactoryIR, TaskExecutionType, TaskTypeId);            
            string mapping_schema = mapping.TaskMasterJsonSchema;
            _TaskIsValid = Shared.JsonHelpers.ValidateJsonUsingSchema(logging, mapping_schema, this.TaskMasterJson, "Failed to validate TaskMaster JSON for TaskTypeMapping: " + mapping.MappingName + ". ");
            
            if (_TaskIsValid)
            {
                if (TaskType == "SQL Database to Azure Storage")
                {
                    ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet();
                    goto ProcessTaskMasterEnd;
                }

                if (TaskType == "Execute SQL Statement")
                {
                    ProcessTaskMaster_Mapping_AZ_SQL_StoredProcedure();
                    goto ProcessTaskMasterEnd;
                }

                if (TaskType == "Azure Storage to SQL Database")
                {
                    ProcessTaskMaster_Default();
                    goto ProcessTaskMasterEnd;
                }
                
                //Default Processing Branch              
                {
                    ProcessTaskMaster_Default();
                    goto ProcessTaskMasterEnd;
                }


            ProcessTaskMasterEnd:
                logging.LogInformation("ProcessTaskMasterJson Finished");

            }


        }

        public void ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet()
        {
            JObject Source = ((JObject)_JsonObjectForADF["Source"]) == null ? new JObject() : (JObject)_JsonObjectForADF["Source"]; 
            JObject Target = ((JObject)_JsonObjectForADF["Target"]) == null ? new JObject() : (JObject)_JsonObjectForADF["Target"];            

            Source.Merge(_TaskMasterJson["Source"], new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            Target.Merge(_TaskMasterJson["Target"], new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });


            JObject Extraction = ((JObject)Source["Extraction"]) == null ? new JObject() : (JObject)Source["Extraction"];

            Extraction["Type"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "Type", _TaskMasterJsonSource, "", true);
            Extraction["IncrementalType"] = ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet_IncrementalType();
            Extraction["IncrementalColumnType"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "IncrementalColumnType", _TaskMasterJsonSource, "", true);
            Extraction["IncrementalValue"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "IncrementalValue", _TaskMasterJsonSource, "", true);
            Extraction["IncrementalField"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "IncrementalField", _TaskMasterJsonSource, "", true);
            Extraction["ChunkField"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "ChunkField", _TaskMasterJsonSource, "", true);
            Extraction["ChunkField"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "ChunkField", _TaskMasterJsonSource, "", true);
            Extraction["ChunkSize"] = System.Convert.ToInt32(Shared.JsonHelpers.GetDynamicValueFromJSON(logging, "ChunkSize", _TaskMasterJsonSource, "0", false));
            Extraction["TableSchema"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "TableSchema", _TaskMasterJsonSource, "", true);  
            Extraction["TableName"] = Shared.JsonHelpers.GetStringValueFromJSON(logging, "TableName", _TaskMasterJsonSource, "", true);

            Extraction["IncrementalSQLStatement"] = ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet_CreateIncrementalSQLStatement(Extraction);
            Extraction["SQLStatement"] = ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet_CreateSQLStatement(Extraction);


            Source["Extraction"] = Extraction;

            JObject Execute = new JObject();
            if (Shared.JsonHelpers.CheckForJSONProperty(logging, "StoredProcedure", _TaskMasterJsonSource))
            {
                string _storedProcedure = _TaskMasterJsonSource["StoredProcedure"].ToString();
                if (_storedProcedure.Length > 0)
                {
                    string _spParameters = string.Empty;
                    if (Shared.JsonHelpers.CheckForJSONProperty(logging, "Parameters", _TaskMasterJsonSource))
                    {
                        _spParameters = _TaskMasterJsonSource["Parameters"].ToString();
                    }
                    _storedProcedure = string.Format("Exec {0} {1} {2} {3}", _storedProcedure, _spParameters, Environment.NewLine, " Select 1");

                }
                Execute["StoredProcedure"] = _storedProcedure;
            }
            Source["Execute"] = Execute;           

            _JsonObjectForADF["Source"] = Source;
            _JsonObjectForADF["Target"] = Target;


        }

        public string ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet_IncrementalType()
        {
            string _Type = Shared.JsonHelpers.GetStringValueFromJSON(logging, "Type", _TaskMasterJsonSource, "", true);
            if (!string.IsNullOrWhiteSpace(_Type))
            {
                JToken _IncrementalType = Shared.JsonHelpers.GetStringValueFromJSON(logging, "IncrementalType", _TaskMasterJsonSource, "", true);
                Int32 _ChunkSize = System.Convert.ToInt32(Shared.JsonHelpers.GetDynamicValueFromJSON(logging, "ChunkSize", _TaskMasterJsonSource, "0", false));
                if (_IncrementalType.ToString() == "Full" && _ChunkSize==0)
                {
                    _Type = "Full";
                }
                else if (_IncrementalType.ToString() == "Full" && _ChunkSize > 0)
                {
                    _Type = "Full_Chunk";
                }
                else if (_IncrementalType.ToString() == "Watermark" && _ChunkSize==0)
                {
                    _Type = "Watermark";
                }
                else if (_IncrementalType.ToString() == "Watermark" && _ChunkSize > 0)
                {
                    _Type = "Watermark_Chunk";
                }                             
            }

            return _Type;
        }

        public string ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet_CreateIncrementalSQLStatement(JObject Extraction)
        {
            string _SQLStatement = "";

            if (Extraction["IncrementalType"] != null)
            {

                if (Extraction["IncrementalType"].ToString().ToLower() == "full")
                {
                    _SQLStatement = "";
                }

                if (Extraction["IncrementalType"].ToString().ToLower() == "full_chunk")
                {
                    _SQLStatement = @$"
                       SELECT 
		                    CAST(CEILING(count(*)/{Extraction["ChunkSize"]} + 0.00001) as int) as  batchcount
	                    FROM [{Extraction["TableSchema"]}].[{Extraction["TableName"]}] 
                    ";
                }

                if (Extraction["IncrementalType"].ToString().ToLower() == "watermark" && Extraction["IncrementalColumnType"].ToString().ToLower() == "datetime")
                {
                    _SQLStatement = @$"
                        SELECT 
	                        MAX([{Extraction["IncrementalField"]}]) AS newWatermark
                        FROM 
	                        [{Extraction["TableSchema"]}].[{Extraction["TableName"]}] 
                        WHERE [{Extraction["IncrementalField"]}] > CAST('{Extraction["IncrementalValue"]}' as datetime)
                    ";
                }

                if (Extraction["IncrementalType"].ToString().ToLower() == "watermark" && Extraction["IncrementalColumnType"].ToString().ToLower() != "datetime")
                {
                    _SQLStatement = @$"
                        SELECT 
	                        MAX([{Extraction["IncrementalField"]}]) AS newWatermark
                        FROM 
	                        [{Extraction["TableSchema"]}].[{Extraction["TableName"]}] 
                        WHERE [{Extraction["IncrementalField"]}] > {Extraction["IncrementalValue"]}
                    ";
                }

                if (Extraction["IncrementalType"].ToString().ToLower() == "watermark_chunk" && Extraction["IncrementalColumnType"].ToString().ToLower() == "datetime")
                {
                    _SQLStatement = @$"
                        SELECT MAX([{Extraction["IncrementalField"]}]) AS newWatermark, 
		                       CAST(CASE when count(*) = 0 then 0 else CEILING(count(*)/{Extraction["ChunkSize"]} + 0.00001) end as int) as  batchcount
	                    FROM  [{Extraction["TableSchema"]}].[{Extraction["TableName"]}] 
	                    WHERE [{Extraction["IncrementalField"]}] > CAST('{Extraction["IncrementalValue"]}' as datetime)
                    ";
                }

                if (Extraction["IncrementalType"].ToString().ToLower() == "watermark_chunk" && Extraction["IncrementalColumnType"].ToString().ToLower() != "datetime")
                {
                    _SQLStatement = @$"
                        SELECT MAX([{Extraction["IncrementalField"]}]) AS newWatermark, 
		                       CAST(CASE when count(*) = 0 then 0 else CEILING(count(*)/{Extraction["ChunkSize"]} + 0.00001) end as int) as  batchcount
	                    FROM  [{Extraction["TableSchema"]}].[{Extraction["TableName"]}] 
	                    WHERE [{Extraction["IncrementalField"]}] > {Extraction["IncrementalValue"]}
                    ";
                }

            }

            return _SQLStatement;
        }

        public string ProcessTaskMaster_Mapping_XX_SQL_AZ_Storage_Parquet_CreateSQLStatement(JObject Extraction)
        {
            string _SQLStatement = "";
            JObject TmSource = (JObject)_TaskMasterJsonSource["Source"];
            if (Extraction["IncrementalType"] != null)
            {
                string _IncrementalType = (string)Extraction["IncrementalType"];
                Int32 _ChunkSize = (Int32)Extraction["ChunkSize"];
                JToken _IncrementalField = Shared.JsonHelpers.GetStringValueFromJSON(logging, "IncrementalField", _TaskMasterJsonSource, "", true);
                JToken _IncrementalColumnType = Shared.JsonHelpers.GetStringValueFromJSON(logging, "IncrementalColumnType", _TaskMasterJsonSource, "", true);
                JToken _ChunkField = (string)Extraction["ChunkField"];
                JToken _TableSchema = Extraction["TableSchema"];
                JToken _TableName = Extraction["TableName"];
                string _ExtractionSQL = Shared.JsonHelpers.GetStringValueFromJSON(logging, "ExtractionSQL", Extraction, "", false);               


                //If Extraction SQL Explicitly set then overide _SQLStatement with that explicit value
                if (!string.IsNullOrWhiteSpace(_ExtractionSQL.ToString()))
                {
                    _SQLStatement = _ExtractionSQL.ToString();
                    goto EndOfSQLStatementSet;
                }


                //Chunk branch
                if (_ChunkSize > 0)
                {
                    if (_IncrementalType.ToString() == "Full" && _ChunkSize == 0)
                    {
                        _SQLStatement = string.Format("SELECT * FROM {0}.{1}", _TableSchema, _TableName);
                    }
                    else if (_IncrementalType.ToString() == "Full" && _ChunkSize > 0)
                    {
                        _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE CAST({2} AS BIGINT) %  <batchcount> = <item> -1. ", _TableSchema, _TableName, _ChunkField);
                    }
                    else if (_IncrementalType.ToString() == "Watermark" && _ChunkSize == 0)
                    {
                        if (_IncrementalColumnType.ToString() == "DateTime")
                        {
                            DateTime _IncrementalValueDateTime = (DateTime)_TaskInstanceJson["IncrementalValue"];
                            _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE {2} > Cast('{3}' as datetime) AND {2} <= Cast('<newWatermark>' as datetime)", _TableSchema, _TableName, _IncrementalField, _IncrementalValueDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }
                        else if (_IncrementalColumnType.ToString() == "BigInt")
                        {
                            int _IncrementalValueBigInt = (int)_TaskInstanceJson["IncrementalValue"];
                            _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE {2} > Cast('{3}' as bigint) AND {2} <= cast('<newWatermark>' as bigint)", _TableSchema, _TableName, _IncrementalField, _IncrementalValueBigInt);
                        }
                    }
                    else if (_IncrementalType.ToString() == "Watermark" && !string.IsNullOrWhiteSpace(_TaskMasterJsonSource["Source"]["ChunkSize"].ToString()))
                    {
                        if (_IncrementalColumnType.ToString() == "DateTime")
                        {
                            DateTime _IncrementalValueDateTime = (DateTime)_TaskInstanceJson["IncrementalValue"];
                            _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE {2} > Cast('{3}' as datetime) AND {2} <= Cast('<newWatermark>' as datetime) AND CAST({4} AS BIGINT) %  <batchcount> = <item> -1.", _TableSchema, _TableName, _IncrementalField, _IncrementalValueDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), _ChunkField);
                        }
                        else if (_IncrementalColumnType.ToString() == "BigInt")
                        {
                            int _IncrementalValueBigInt = (int)_TaskInstanceJson["IncrementalValue"];
                            _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE {2} > Cast('{3}' as bigint) AND {2} <= Cast('<newWatermark>' as bigint) AND CAST({4} AS BIGINT) %  <batchcount> = <item> -1.", _TableSchema, _TableName, _IncrementalField, _IncrementalValueBigInt, _ChunkField);
                        }
                    }
                }
                else
                //Non Chunk
                {
                    if (_IncrementalType.ToString() == "Full")
                    {
                        _SQLStatement = string.Format("SELECT * FROM {0}.{1}", _TableSchema, _TableName);
                    }
                    else if (_IncrementalType.ToString() == "Watermark")
                    {
                        if (_IncrementalColumnType.ToString() == "DateTime")
                        {
                            DateTime _IncrementalValueDateTime = (DateTime)_TaskInstanceJson["IncrementalValue"];
                            _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE {2} > Cast('{3}' as datetime) AND {2} <= Cast('<newWatermark>' as datetime)", _TableSchema, _TableName, _IncrementalField, _IncrementalValueDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }
                        else if (_IncrementalColumnType.ToString() == "BigInt")
                        {
                            int _IncrementalValueBigInt = (int)_TaskInstanceJson["IncrementalValue"];
                            _SQLStatement = string.Format("SELECT * FROM {0}.{1} WHERE {2} > Cast('{3}' as bigint) AND {2} <= cast('<newWatermark>' as bigint)", _TableSchema, _TableName, _IncrementalField, _IncrementalValueBigInt);
                        }
                    }
                }

            }

        EndOfSQLStatementSet:
            return _SQLStatement;
        }


        public void ProcessTaskMaster_Mapping_AZ_SQL_StoredProcedure()
        {
            AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.AZ_SQL_StoredProcedure.TaskMasterJson
                TaskMasterJsonObject = JsonConvert.DeserializeObject<AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.AZ_SQL_StoredProcedure.TaskMasterJson>(JsonConvert.SerializeObject(_TaskMasterJsonSource));

            JObject Source = (JObject)_JsonObjectForADF["Source"];
            JObject Execute = new JObject();
            Execute["StoredProcedure"] = string.Format("Exec {0} {1} {2} {3}", TaskMasterJsonObject.Source.StoredProcedure, TaskMasterJsonObject.Source.Parameters, Environment.NewLine, " Select 1");

            Source["Execute"] = Execute;
            _JsonObjectForADF["Source"] = Source;

       }


        /// <summary>
        /// Default Method which merges Source & Target attributes on TaskMasterJson with existing Source and Target Attributes on TaskObject payload.
        /// </summary>

        public void ProcessTaskMaster_Default()
        {            

            JObject Source = (JObject)_TaskMasterJson["Source"];
            JObject Target = (JObject)_TaskMasterJson["Target"];

            Source.Merge(_JsonObjectForADF["Source"], new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            Target.Merge(_JsonObjectForADF["Target"], new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            _JsonObjectForADF["Source"] = Source;
            _JsonObjectForADF["Target"] = Target;
        }


        public void ProcessTaskMaster_SourceSystem_TypeIsStorage_TaskSourceTypeIsCSV()
        {
            AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.SourceSystemTypeIsStorageAndTaskSourceTypeIsCSV TaskMasterJsonObject = JsonConvert.DeserializeObject<AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.SourceSystemTypeIsStorageAndTaskSourceTypeIsCSV>(JsonConvert.SerializeObject(_TaskMasterJsonSource));

            JObject Source = (JObject)_JsonObjectForADF["Source"];

            Source["Type"] = TaskMasterJsonObject.Type;
            Source["DataFileName"] = TaskMasterJsonObject.DataFileName;
            Source["SchemaFileName"] = TaskMasterJsonObject.SchemaFileName;
            Source["FirstRowAsHeader"] = TaskMasterJsonObject.FirstRowAsHeader;
            Source["SheetName"] = TaskMasterJsonObject.SheetName;
            Source["SkipLineCount"] = TaskMasterJsonObject.SkipLineCount;

            _JsonObjectForADF["Source"] = Source;
        }

        public void ProcessTaskMaster_Target_TypeIsSQL()
        {
            AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.TargetSystemTypeIsSQL TaskMasterJsonObject = JsonConvert.DeserializeObject<AdsGoFast.GetTaskInstanceJSON.TaskMasterJson.TargetSystemTypeIsSQL>(JsonConvert.SerializeObject(_TaskMasterJsonTarget));

            JObject Target = (JObject)_JsonObjectForADF["Target"];

            Target["Type"] = TaskMasterJsonObject.Type;
            Target["TableSchema"] = TaskMasterJsonObject.TableSchema;
            Target["TableName"] = TaskMasterJsonObject.TableSchema;
            Target["StagingTableSchema"] = TaskMasterJsonObject.StagingTableSchema;
            Target["StagingTableName"] = TaskMasterJsonObject.StagingTableName;
            Target["AutoCreateTable"] = TaskMasterJsonObject.AutoGenerateMerge;
            Target["PreCopySQL"] = TaskMasterJsonObject.PreCopySQL;
            Target["PostCopySQL"] = TaskMasterJsonObject.PostCopySQL;
            Target["MergeSQL"] = TaskMasterJsonObject.MergeSQL;
            Target["AutoGenerateMerge"] = TaskMasterJsonObject.AutoGenerateMerge;
            Target["DynamicMapping"] = TaskMasterJsonObject.DynamicMapping;

            _JsonObjectForADF["Target"] = Target;
        }


    }
}
