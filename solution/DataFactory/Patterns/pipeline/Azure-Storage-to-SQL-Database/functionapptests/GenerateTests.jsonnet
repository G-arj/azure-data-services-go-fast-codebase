local tests =
[
    {
        "Active": true,        
        "Pattern": "Azure Storage to SQL Database",         
        "SourceSystemAuthType": "MSI",
        
        "SourceFormat":"Parquet",
        "SourceType":"ADLS",        
        "DataFilename":"SalesLT.Customer.parquet",
        "SchemaFileName":"SalesLT.Customer.json", 
        "SkipLineCount":"",
        "SheetName":"",
        "MaxConcorrentConnections":0,
        "Recursively":"false",
        "DeleteAfterCompletion":"",
        
        "TargetFormat":"Table",
        "TargetType": "Azure SQL", 
        "TableSchema":"dbo",
        "TableName":"Customer",
        "StagingTableSchema":"dbo",
        "StagingTableName":"stg_Customer",
        "AutoCreateTable": "true",
        "PreCopySQL": "",
        "PostCopySQL": "",
        "AutoGenerateMerge": "true",
        "MergeSQL":"", 
        
        "Description": "FullLoad",  
        "ADFPipeline": "GPL_AzureBlobFS_Parquet_AzureSqlTable_NA_IRA", 
       
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
    t.DataFilename,
    t.SchemaFileName,
    t.SourceSystemAuthType,
    t.SkipLineCount,
    t.SheetName,
    t.MaxConcorrentConnections,
    t.Recursively,
    t.DeleteAfterCompletion,
    t.TargetFormat,
    t.TargetType,
    t.TableSchema,
    t.TableName,
    t.StagingTableSchema,
    t.StagingTableName,
    t.AutoCreateTable,
    t.PreCopySQL,
    t.PostCopySQL,
    t.AutoGenerateMerge,
    t.MergeSQL
);


std.mapWithIndex(process, tests)

