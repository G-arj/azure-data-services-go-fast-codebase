local commons = import '../../../static/partials/functionapptest_commons.libsonnet';
local vars = import '../../../static/partials/secrets.libsonnet';
function(
    ADFPipeline = "GPL_AzureSqlTable_NA_AzureBlobStorage_Parquet_IRA",
    Pattern = "SQL Database to Azure Storage",
    TestNumber = "1",
    SourceFormat = "Azure SQL",
    SourceType = "Azure SQL",
    ExtractionSQL = "",
    DataFilename = "SalesLT.Customer.parquet",
    SchemaFileName = "SalesLT.Customer.json",
    SourceSystemAuthType = "MSI",
    TargetFormat = "Parquet",
    TargetType = "Azure Blob", 
    ChunkField = "",
    ChunkSize = 0,
    IncrementalType = "Full",
    IncrementalField = "",
    IncrementalColumnType = "",
    IncrementalValue = "0"
    )
{
    local TaskMasterJson =     
    {
        "Source":{
            "Type": SourceFormat,
            "IncrementalType": IncrementalType,
            "IncrementalColumnType":IncrementalColumnType,
            "IncrementalField":IncrementalField,
            "IncrementalValue":IncrementalValue,
            "TableSchema": "SalesLT",
            "TableName": "Customer",
            "ExtractionSQL": ExtractionSQL,                   
            "ChunkField":ChunkField,
            "ChunkSize":ChunkSize
        },
        "Target":{
            "Type":TargetFormat,
            "RelativePath":"/Tests/"+Pattern+"/"+TestNumber,
            "DataFileName": DataFilename,
            "SchemaFileName": SchemaFileName
        }
    },

    local TaskInstanceJson =  
    {
        "TargetRelativePath": "/Tests/"+Pattern+"/"+TestNumber+"/"
    },

    local SourceSystemJson = 
    {
        "Database": vars.AdsOpts_CD_Services_AzureSQLServer_SampleDB_Name,
        "UsernameKeyVaultSecretName":"",
        "PasswordKeyVaultSecretName":""        
    },

    local TargetSystemJson = 
    {
        "Container" : "datalakeraw" 
    },
             
    "TaskInstanceJson":std.manifestJson(TaskInstanceJson),
    "TaskType":Pattern,
    "DataFactoryName":vars.AdsOpts_CD_Services_DataFactory_Name,
    "DataFactoryResourceGroup":vars.AdsOpts_CD_ResourceGroup_Name,
    "DataFactorySubscriptionId":vars.AdsOpts_CD_Services_DataFactory_SubscriptionId,
    "TaskMasterJson":std.manifestJson(TaskMasterJson),       
    "TaskMasterId":TestNumber,
    "SourceSystemJSON":std.manifestJson(SourceSystemJson),
    "SourceSystemType":SourceType,
    "SourceSystemServer":vars.AdsOpts_CD_Services_AzureSQLServer_Name + ".database.windows.net",
    "SourceKeyVaultBaseUrl":"https://" + vars.AdsOpts_CD_Services_KeyVault_Name +".vault.azure.net",
    "SourceSystemAuthType":SourceSystemAuthType,
    "SourceSystemSecretName":"",
    "SourceSystemUserName":"",   
    "TargetSystemJSON":std.manifestJson(TargetSystemJson),
    "TargetSystemType":TargetType,
    "TargetSystemServer":if(TargetType == "Azure Blob") then "https://" + vars.AdsOpts_CD_Services_Storage_Blob_Name + ".blob.core.windows.net" else "https://" + vars.AdsOpts_CD_Services_Storage_ADLS_Name + ".dfs.core.windows.net",
    "TargetKeyVaultBaseUrl":"https://" + vars.AdsOpts_CD_Services_KeyVault_Name +".vault.azure.net",
    "TargetSystemAuthType":"MSI",
    "TargetSystemSecretName":"",
	"TargetSystemUserName":"",
    "ADFPipeline": ADFPipeline
}+commons

