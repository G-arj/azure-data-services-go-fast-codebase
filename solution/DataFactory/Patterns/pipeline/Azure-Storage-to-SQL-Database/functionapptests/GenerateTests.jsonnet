local tests =
[
    {
        "Active": true,        
        "Pattern": "Azure Storage to SQL Database",         
        "SourceFormat":"Parquet",
        "SourceType":"Azure Blob",        
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "MSI",
        "TargetFormat":"Table",
        "TargetType": "Azure SQL", 
        "ADFPipeline": "GPL_AzureBlobStorage_Parquet_AzureSqlTable_NA_IRA", 
        "Description": "FullLoad",            
        "SkipLineCount":"",
        "MaxConcorrentConnections":0,
        "Recursively":"false",
        "DeleteAfterCompletion":""
    },
    {
        "Active": true,        
        "Pattern": "Azure Storage to SQL Database",         
        "SourceFormat":"Parquet",
        "SourceType":"ADLS",        
        "ExtractionSQL":"",
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json",
        "SourceSystemAuthType": "MSI",
        "TargetFormat":"Table",
        "TargetType": "Azure SQL", 
        "ADFPipeline": "GPL_AzureBlobFS_Parquet_AzureSqlTable_NA_IRA", 
        "Description": "FullLoad",            
        "SkipLineCount":"",
        "MaxConcorrentConnections":0,
        "Recursively":"false",
        "DeleteAfterCompletion":""
    }
];

local template = import "./partials/functionapptest.libsonnet";

local process = function(index, t)
template(
    t.ADFPipeline,
    t.Pattern, 
    index,//t.TestNumber,
    t.SourceFormat,
    t.SourceType,
    t.ExtractionSQL,
    t.DataFilename,
    t.SchemaFileName,
    t.SourceSystemAuthType,
    t.TargetFormat,
    t.TargetType,
    t.SkipLineCount,
    t.MaxConcorrentConnections,
    t.Recursively,
    t.DeleteAfterCompletion
);


std.mapWithIndex(process, tests)

