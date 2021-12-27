local tests =
[
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":0,
        "SourceFormat":"Table",
        "SourceType":"Azure SQL",
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "MSI",
        "TargetFormat":"Parquet",
        "TargetType": "Azure Blob", 
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobStorage_Parquet_IRA", 
        "Description": "FulLoad"
    },
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":1,
        "SourceFormat":"Table",
        "SourceType":"Azure SQL",
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "MSI",
        "TargetFormat":"Parquet",
        "TargetType": "ADLS",
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobFS_Parquet_IRA",
        "Description": "FullLoad"
    },
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":2,
        "SourceFormat":"Table",
        "SourceType":"Azure SQL",
        "ExtractionSQL":"Select top 10 * from SalesLT.Customer",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "MSI",
        "TargetFormat":"Parquet",
        "TargetType": "ADLS",
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobFS_Parquet_IRA",
        "Description": "FullLoadUsingExtractionSql"
    },    
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":3,
        "SourceFormat":"Table",
        "SourceType":"SQL Server",
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "SQLAuth",
        "TargetFormat":"Parquet",
        "TargetType": "Azure Blob",
        "ADFPipeline": "GPL_SqlServerTable_NA_AzureBlobStorage_Parquet_IRA",
        "Description": "FullLoad"
    },    
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":4,
        "SourceFormat":"Table",
        "SourceType":"SQL Server",
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "SQLAuth",
        "TargetFormat":"Parquet",
        "TargetType": "ADLS",
        "ADFPipeline": "GPL_SqlServerTable_NA_AzureBlobFS_Parquet_IRA",
        "Description": "FullLoad"
    }
];

local template = import "./partials/functionapptest.libsonnet";
[
if(t.Active) then 
template(
    t.ADFPipeline,
    t.Pattern, 
    t.TestNumber,
    t.SourceFormat,
    t.SourceType,
    t.ExtractionSQL,
    t.DataFilename,
    t.SchemaFileName,
    t.SourceSystemAuthType,
    t.TargetFormat,
    t.TargetType
)
for t in tests if t.Active == true  
  
]
