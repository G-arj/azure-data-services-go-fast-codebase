local tests =
[
    

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
    t.FirstRowAsHeader,
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

