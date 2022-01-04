function(GenerateArm="true",GFPIR="{IRA}",SourceType="AzureBlobFS",SourceFormat="Binary", TargetType="AzureBlobFS", TargetFormat="Binary")

local generateArmAsBool = GenerateArm == "true";
local Wrapper = import '../static/partials/wrapper.libsonnet';

local typeProperties = import './partials/typeProperties/typeProperties.libsonnet';
local datasets = {
	"Binary" : import './partials/datasetReferences/Binary.libsonnet',
	"DelimitedText" : import './partials/datasetReferences/DelimitedText.libsonnet',
	"Excel" : import './partials/datasetReferences/Excel.libsonnet',
	"Json" : import './partials/datasetReferences/Json.libsonnet',
	"Parquet" : import './partials/datasetReferences/Parquet.libsonnet'
};

local parameterDefaultValue = import './partials/parameterDefaultValue.libsonnet';

local name =  if(!generateArmAsBool) 
			then "GPL_"+SourceType+"_"+SourceFormat+"_"+TargetType+"_"+TargetFormat+"_"+GFPIR 
			else "[concat(parameters('dataFactoryName'), '/','GPL_"+SourceType+"_"+SourceFormat+"_"+TargetType+"_"+TargetFormat+"_" + "', parameters('integrationRuntimeShortName'))]";

local copyActivityName = "Copy %(Source)s to %(Target)s" % {Source: SourceType, Target: TargetType};
local logSuccessActivityName = "%(ActivityName)s Succeed" % {ActivityName: copyActivityName};
local logStartedActivityName = "%(ActivityName)s Started" % {ActivityName: copyActivityName};
local logFailedActivityName = "%(ActivityName)s Failed" % {ActivityName: copyActivityName};

local pipeline = {
	"name": name,
	"properties": {
		"activities": [
            {
                "name": copyActivityName,
                "type": "Copy",
                "dependsOn": [
                    {
                        "activity": logStartedActivityName,
                        "dependencyConditions": [
                            "Succeeded"
                        ]
                    }
                ],
                "policy": {
                    "timeout": "7.00:00:00",
                    "retry": 0,
                    "retryIntervalInSeconds": 30,
                    "secureOutput": false,
                    "secureInput": false
                },
                "userProperties": [],
                "typeProperties": typeProperties(generateArmAsBool, GFPIR, SourceType, SourceFormat, TargetType, TargetFormat),
                "inputs": [datasets[SourceFormat](generateArmAsBool, SourceType, GFPIR, 'Source')],
                "outputs": [datasets[TargetFormat](generateArmAsBool, TargetType, GFPIR, 'Target')],
            },
			{
			"name": logFailedActivityName,
			"type": "ExecutePipeline",
			"dependsOn": [
				{
					"activity": copyActivityName,
					"dependencyConditions": [
						"Failed"
					]
				}
			],
			"userProperties": [],
			"typeProperties": {
				"pipeline": {
					"referenceName": "SPL_AzureFunction",
					"type": "PipelineReference"
				},
				"waitOnCompletion": false,
				"parameters": {
					"Body": {
						"value": "@json(concat('{\"TaskInstanceId\":\"', string(pipeline().parameters.TaskObject.TaskInstanceId), '\",\"ExecutionUid\":\"', string(pipeline().parameters.TaskObject.ExecutionUid), '\",\"RunId\":\"', string(pipeline().RunId), '\",\"LogTypeId\":1,\"LogSource\":\"ADF\",\"ActivityType\":\"Copy Blob to Blob\",\"StartDateTimeOffSet\":\"', string(pipeline().TriggerTime), '\",\"EndDateTimeOffSet\":\"', string(utcnow()), '\",\"Comment\":\"', string(activity('" + copyActivityName + "').error.message), '\",\"Status\":\"Failed\"}'))",
						"type": "Expression"
					},
					"FunctionName": "Log",
					"Method": "Post"
				}
			}
		},
		{
			"name": logStartedActivityName,
			"type": "ExecutePipeline",
			"dependsOn": [],
			"userProperties": [],
			"typeProperties": {
				"pipeline": {
					"referenceName": "SPL_AzureFunction",
					"type": "PipelineReference"
				},
				"waitOnCompletion": false,
				"parameters": {
					"Body": {
						"value": "@json(concat('{\"TaskInstanceId\":\"', string(pipeline().parameters.TaskObject.TaskInstanceId), '\",\"ExecutionUid\":\"', string(pipeline().parameters.TaskObject.ExecutionUid), '\",\"RunId\":\"', string(pipeline().RunId), '\",\"LogTypeId\":3,\"LogSource\":\"ADF\",\"ActivityType\":\"Copy Blob to Blob\",\"StartDateTimeOffSet\":\"', string(pipeline().TriggerTime), '\",\"Status\":\"Started\"}'))",
						"type": "Expression"
					},
					"FunctionName": "Log",
					"Method": "Post"
				}
			}
		},
		{
			"name": logSuccessActivityName,
			"type": "ExecutePipeline",
			"dependsOn": [
				{
					"activity": copyActivityName,
					"dependencyConditions": [
						"Succeeded"
					]
				}
			],
			"userProperties": [],
			"typeProperties": {
				"pipeline": {
					"referenceName": "SPL_AzureFunction",
					"type": "PipelineReference"
				},
				"waitOnCompletion": false,
				"parameters": {
					"Body": {
						"value": "@json(concat('{\"TaskInstanceId\":\"', string(pipeline().parameters.TaskObject.TaskInstanceId), '\",\"ExecutionUid\":\"', string(pipeline().parameters.TaskObject.ExecutionUid), '\",\"RunId\":\"', string(pipeline().RunId), '\",\"LogTypeId\":1,\"LogSource\":\"ADF\",\"ActivityType\":\"Copy Blob to Blob\",\"StartDateTimeOffSet\":\"', string(pipeline().TriggerTime), '\",\"EndDateTimeOffSet\":\"', string(utcnow()), '\",\"RowsInserted\":\"', string(activity('" + copyActivityName + "').output.filesWritten), '\",\"Comment\":\"\",\"Status\":\"Complete\"}'))",
						"type": "Expression"
					},
					"FunctionName": "Log",
					"Method": "Post"
				}
			}
		},
		{
			"name": "Pipeline AF Log - Failed",
			"type": "ExecutePipeline",
			"dependsOn": [
				{
					"activity": "Copy AzureBlobStorage to AzureBlobStorage Failed",
					"dependencyConditions": [
						"Completed"
					]
				}
			],
			"userProperties": [],
			"typeProperties": {
				"pipeline": {
					"referenceName": "SPL_AzureFunction",
					"type": "PipelineReference"
				},
				"waitOnCompletion": true,
				"parameters": {
					"Body": {
						"value": "@json(\r\n    concat('{\"TaskInstanceId\":\"', \r\n        string(pipeline().parameters.TaskObject.TaskInstanceId), \r\n        '\",\"ExecutionUid\":\"',\r\n         string(pipeline().parameters.TaskObject.ExecutionUid), \r\n         '\",\"RunId\":\"', \r\n         string(pipeline().RunId), \r\n         '\",\"LogTypeId\":1,\"LogSource\":\"ADF\",\"ActivityType\":\"Data-Movement-Master\",\"StartDateTimeOffSet\":\"'\r\n         , string(pipeline().TriggerTime), \r\n         '\",\"EndDateTimeOffSet\":\"', \r\n         string(utcnow()), '\",\"Comment\":\"', \r\n         concat(pipeline().Pipeline, 'Failed'), \r\n         '\",\"Status\":\"Failed\",\"NumberOfRetries\":\"', \r\n         string(pipeline().parameters.TaskObject.NumberOfRetries),\r\n         '\"}'\r\n    )\r\n)",
						"type": "Expression"
					},
					"FunctionName": "Log",
					"Method": "Post"
				}
			}
		},
		{
			"name": "Pipeline AF Log - Succeed",
			"type": "ExecutePipeline",
			"dependsOn": [
				{
					"activity": "Copy AzureBlobStorage to AzureBlobStorage Succeed",
					"dependencyConditions": [
						"Completed"
					]
				}
			],
			"userProperties": [],
			"typeProperties": {
				"pipeline": {
					"referenceName": "SPL_AzureFunction",
					"type": "PipelineReference"
				},
				"waitOnCompletion": true,
				"parameters": {
					"Body": {
						"value": "@json(concat('{\"TaskInstanceId\":\"', string(pipeline().parameters.TaskObject.TaskInstanceId), '\",\"ExecutionUid\":\"', string(pipeline().parameters.TaskObject.ExecutionUid), '\",\"RunId\":\"', string(pipeline().RunId), '\",\"LogTypeId\":1,\"LogSource\":\"ADF\",\"ActivityType\":\"Data-Movement-Master\",\"StartDateTimeOffSet\":\"', string(pipeline().TriggerTime), '\",\"EndDateTimeOffSet\":\"', string(utcnow()), '\",\"Comment\":\"\",\"Status\":\"Complete\",\"NumberOfRetries\":\"', string(pipeline().parameters.TaskObject.NumberOfRetries),'\"}'))",
						"type": "Expression"
					},
					"FunctionName": "Log",
					"Method": "Post"
				}
			}
		}
		],
		"parameters": parameterDefaultValue(SourceType),
		"folder": {
			"name": if(GenerateArm=="false") 
					then "ADS Go Fast/Data Movement/" + GFPIR
					else "[concat('ADS Go Fast/Data Movement/', parameters('integrationRuntimeShortName'))]",
		},
		"annotations": [],
		"lastPublishTime": "2020-08-05T04:14:00Z"
	},
	"type": "Microsoft.DataFactory/factories/pipelines"
};
Wrapper(GenerateArm,pipeline)+{}