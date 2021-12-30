local commons = import '../../../static/partials/functionapptest_commons.libsonnet';
local vars = import '../../../static/partials/secrets.libsonnet';
function(
    ADFPipeline = "GPL_AzureBlobStorage_ParquetAzureSqlTable_NA_IRA",
    Pattern = "Azure Storage to SQL Database",
    TestNumber = "1",
    SourceFormat = "Azure SQL",
    SourceType = "Azure SQL",
    DataFilename = "SalesLT.Customer.parquet",
    SchemaFileName = "SalesLT.Customer.json",
    SourceSystemAuthType = "MSI",
    SkipLineCount = "",
    SheetName = "",
    MaxConcorrentConnections = 0,
    Recursively = "false",
    DeleteAfterCompletion = "",
    TargetFormat = "Parquet",
    TargetType = "Azure Blob", 
    TableSchema,
    TableName,
    StagingTableSchema,
    StagingTableName,
    AutoCreateTable,
    PreCopySQL,
    PostCopySQL,
    AutoGenerateMerge,
    MergeSQL
    )
{
    local TaskMasterJson =     
    {
        "Source":{
            "Type": SourceFormat,                       
            "RelativePath": "/samples/",
            "DataFileName": DataFilename,
            "SchemaFileName": SchemaFileName,
            "SkipLineCount": SkipLineCount,
            "MaxConcorrentConnections": MaxConcorrentConnections,
            "Recursively": Recursively,
            "DeleteAfterCompletion": DeleteAfterCompletion,
        },
        "Target":{
            "Type":TargetFormat,
            "DataFileName": DataFilename,
            "SchemaFileName": SchemaFileName,
            "TableSchema":TableSchema,
            "TableName":TableName+TestNumber,
            "StagingTableSchema":StagingTableSchema,
            "StagingTableName":StagingTableName+TestNumber,
            "AutoCreateTable":AutoCreateTable,
            "PreCopySQL":PreCopySQL,
            "PostCopySQL":PostCopySQL,
            "AutoGenerateMerge":AutoGenerateMerge,
            "MergeSQL":MergeSQL
        }
    },

    local TaskInstanceJson =  
    {
        "SourceRelativePath": "/samples/"
    },

    local SourceSystemJson = 
    {
        "Container" : "datalakeraw"       
    },

    local TargetSystemJson = 
    {   
        "Database": vars.AdsOpts_CD_Services_AzureSQLServer_SampleDB_Name,
        "UsernameKeyVaultSecretName":"",
        "PasswordKeyVaultSecretName":""
    },
             
    "TaskInstanceJson":std.manifestJson(TaskInstanceJson),
    "TaskTypeId":1,
    "TaskType":Pattern,
    "DataFactoryName":vars.AdsOpts_CD_Services_DataFactory_Name,
    "DataFactoryResourceGroup":vars.AdsOpts_CD_ResourceGroup_Name,
    "DataFactorySubscriptionId":vars.AdsOpts_CD_Services_DataFactory_SubscriptionId,
    "TaskMasterJson":std.manifestJson(TaskMasterJson),       
    "TaskMasterId":TestNumber,
    "SourceSystemJSON":std.manifestJson(SourceSystemJson),
    "SourceSystemType":SourceType,
    "SourceSystemServer":if(SourceType == "Azure Blob") then "https://" + vars.AdsOpts_CD_Services_Storage_Blob_Name + ".blob.core.windows.net" else "https://" + vars.AdsOpts_CD_Services_Storage_ADLS_Name + ".dfs.core.windows.net",
    "SourceKeyVaultBaseUrl":"https://" + vars.AdsOpts_CD_Services_KeyVault_Name +".vault.azure.net",
    "SourceSystemAuthType":SourceSystemAuthType,
    "SourceSystemSecretName":"",
    "SourceSystemUserName":"",   
    "TargetSystemJSON":std.manifestJson(TargetSystemJson),
    "TargetSystemType":TargetType,
    "TargetSystemServer":vars.AdsOpts_CD_Services_AzureSQLServer_Name + ".database.windows.net",
    "TargetKeyVaultBaseUrl":"https://" + vars.AdsOpts_CD_Services_KeyVault_Name +".vault.azure.net",
    "TargetSystemAuthType":"MSI",
    "TargetSystemSecretName":"",
	"TargetSystemUserName":"",
    "ADFPipeline": ADFPipeline
}+commons

