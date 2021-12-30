function(GenerateArm="false",GFPIR="IRA", TargetType="AzureBlobFS", TargetFormat="Parquet") 
if (TargetType=="AzureBlobFS"&&TargetFormat=="Parquet") then
{
  local referenceName = "GDS_AzureBlobFS_Parquet_",
  "dataset": {    
    "referenceName":if(GenerateArm=="false") 
                    then referenceName + GFPIR
                    else "[concat('"+referenceName+"', parameters('integrationRuntimeShortName'))]",   
    "type": "DatasetReference",
    "parameters": {
      "RelativePath": {
        "value": "@pipeline().parameters.TaskObject.Target.Instance.TargetRelativePath",
        "type": "Expression"
      },
      "FileName": {
        "value": "@if(equals(pipeline().parameters.TaskObject.Source.ChunkSize,0),\n    pipeline().parameters.TaskObject.Target.DataFileName,\n    replace(\n        pipeline().parameters.TaskObject.Target.DataFileName,\n        '.parquet',\n        concat('.chunk_', string(pipeline().parameters.Item),'.parquet')\n    )\n)",
        "type": "Expression"
      },
      "StorageAccountEndpoint": {
        "value": "@pipeline().parameters.TaskObject.Target.System.SystemServer",
        "type": "Expression"
      },
      "StorageAccountContainerName": {
        "value": "@pipeline().parameters.TaskObject.Target.System.Container",
        "type": "Expression"
      }
    }
  },
  "fieldList": [
    "structure"
  ],
  "storeSettings": {
    "type": "AzureBlobFSReadSettings",
    "recursive": true
  }
}
else if (TargetType=="AzureBlobStorage"&&TargetFormat=="Parquet") then
{
  local referenceName = "GDS_AzureBlobStorage_Parquet_",
  "dataset": {    
    "referenceName":if(GenerateArm=="false") 
                    then referenceName + GFPIR
                    else "[concat('"+referenceName+"', parameters('integrationRuntimeShortName'))]",   
    "type": "DatasetReference",
    "parameters": {
      "RelativePath": {
        "value": "@pipeline().parameters.TaskObject.Target.Instance.TargetRelativePath",
        "type": "Expression"
      },
      "FileName": {
        "value": "@if(equals(pipeline().parameters.TaskObject.Source.ChunkSize,0),\n    pipeline().parameters.TaskObject.Target.DataFileName,\n    replace(\n        pipeline().parameters.TaskObject.Target.DataFileName,\n        '.parquet',\n        concat('.chunk_', string(pipeline().parameters.Item),'.parquet')\n    )\n)",
        "type": "Expression"
      },
      "StorageAccountEndpoint": {
        "value": "@pipeline().parameters.TaskObject.Target.System.SystemServer",
        "type": "Expression"
      },
      "StorageAccountContainerName": {
        "value": "@pipeline().parameters.TaskObject.Target.System.Container",
        "type": "Expression"
      }
    }
  },
  "fieldList": [
    "structure"
  ],
  "storeSettings": {
    "type": "AzureBlobFSReadSettings",
    "recursive": true
  }
}
else 
  error 'Full_Load_GetTargetMetadata.libsonnet failed. No mapping for:' +GFPIR+","+TargetType+","+TargetFormat

