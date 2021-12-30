function(GenerateArm="false",GFPIR="IRA") 
{
  "outputs": [
    {
      local referenceName = "GDS_AzureBlobFS_Parquet_",
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
          "value": "@replace(pipeline().parameters.TaskObject.Target.DataFileName,'.parquet',concat('.chunk_', string(pipeline().parameters.Item),'.parquet'))",
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
    }
  ]
}