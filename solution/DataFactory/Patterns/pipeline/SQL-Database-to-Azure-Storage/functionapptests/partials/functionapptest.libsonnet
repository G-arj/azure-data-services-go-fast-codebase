local commons = import '../../../static/partials/functionapptest_commons.libsonnet';
local vars = import 'secrets.libsonnet';
function(
    Pattern = "Test",
    TestNumber = "1"
    )
{
    

    local TaskMasterJson =     
    {
        "Source":{
            "Type": "Azure SQL",
            "Database": {
                "SystemName": vars.AdsOpts_CD_Services_AzureSQLServer_Name+ ".database.windows.net",
                "Name": vars.AdsOpts_CD_Services_AzureSQLServer_SampleDB_Name,
                "AuthenticationType": "MSI"
            },
            "Extraction": {
                "Type": "Table",
                "FullOrIncremental": "Full",
                "IncrementalType": "Full",
                "TableSchema": "SalesLT",
                "TableName": "SalesOrderHeader",
                "ExtractionSQL": "",                        
                "SQLStatement": ""
            }
        },
        "Target":{
            "Type":"Parquet",
            "RelativePath":"/Tests/"+Pattern+"/"+TestNumber,
            "DataFileName":"SalesL.Customer.parquet",
            "SchemaFileName":"SalesL.Customer.json"
        }
    },

    local TaskInstanceJson =  
    {
        "TargetRelativePath": "/Tests/"+Pattern+"/"+TestNumber+"/"
    },

    local SourceSystemJson = 
    {
        "Database": vars.AdsOpts_CD_Services_AzureSQLServer_SampleDB_Name
    },

    local TargetSystemJson = 
    {
        "Container" : "datalakeraw" 
    },

               
    "TaskInstanceJson":std.manifestJson(TaskInstanceJson),
    "TaskType":"SQL Database to Azure Storage",
    "DataFactoryName":vars.AdsOpts_CD_Services_DataFactory_Name,
    "DataFactoryResourceGroup":vars.AdsOpts_CD_ResourceGroup_Name,
    "DataFactorySubscriptionId":vars.AdsOpts_CD_Services_DataFactory_SubscriptionId,
    "TaskMasterJson":std.manifestJson(TaskMasterJson),       
    "TaskMasterId":TestNumber,
    "SourceSystemJSON":std.manifestJson(SourceSystemJson),
    "SourceSystemType":"Azure SQL",
    "SourceSystemServer":vars.AdsOpts_CD_Services_AzureSQLServer_Name + ".database.windows.net",
    "SourceKeyVaultBaseUrl":"https://" + vars.AdsOpts_CD_Services_KeyVault_Name +".vault.azure.net",
    "SourceSystemAuthType":"MSI",   
    "TargetSystemJSON":std.manifestJson(TargetSystemJson),
    "TargetSystemType":"Azure Blob",
    "TargetSystemServer":"https://" + vars.AdsOpts_CD_Services_Storage_Blob_Name + ".blob.core.windows.net",
    "TargetKeyVaultBaseUrl":"https://" + vars.AdsOpts_CD_Services_KeyVault_Name +".vault.azure.net",
    "TargetSystemAuthType":"MSI",
}+commons

