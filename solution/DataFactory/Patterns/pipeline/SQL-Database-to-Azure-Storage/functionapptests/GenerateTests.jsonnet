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
        "TargetFormat":"Parquet",
        "TargetType": "Azure Blob", 
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobStorage_Parquet_IRA"
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
        "TargetFormat":"Parquet",
        "TargetType": "ADLS",
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobFS_Parquet_IRA"
    },
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":2,
        "SourceFormat":"Table",
        "SourceType":"SqlServer",
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "TargetFormat":"Parquet",
        "TargetType": "Azure Blob",
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobStorage_Parquet_IRA"
    },
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":3,
        "SourceFormat":"Table",
        "SourceType":"SqlServer",
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "TargetFormat":"Parquet",
        "TargetType": "ADLS",
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobFS_Parquet_IRA"
    },
    {
        "Active": true,
        "Pattern": "SQL Database to Azure Storage", 
        "TestNumber":1,
        "SourceFormat":"SQL",
        "SourceType":"Azure SQL",
        "ExtractionSQL":"Select top 10 * from SalesLT.Customer",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "TargetFormat":"Parquet",
        "TargetType": "ADLS",
        "ADFPipeline": "GPL_AzureSqlTable_NA_AzureBlobFS_Parquet_IRA"
    },
];

local template = import "./partials/functionapptest.libsonnet";
[
if(t.Active) then 
template(
    t.Pattern, 
    t.TestNumber,
    t.SourceFormat,
    t.SourceType,
    t.ExtractionSQL,
    t.DataFilename,
    t.SchemaFileName,
    t.TargetFormat,
    t.TargetType
)
for t in tests if t.Active == true  
  
]
