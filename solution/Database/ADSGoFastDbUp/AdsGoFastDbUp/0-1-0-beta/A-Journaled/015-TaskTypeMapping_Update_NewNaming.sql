


--Add Additional Task Type Mapping

/***********************************************************
Update Mapping Names
***********************************************************/
-- AzureBlobStorage_Excel_AzureSqlTable
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Excel_AzureSqlTable'
    where TaskTypeId = 1
    and MappingName = 'AZ_Storage_Excel_AZ_SQL_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Excel'
    and TargetType = 'Table';
-- AzureBlobStorage_CSV_AzureSqlTable
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_CSV_AzureSqlTable'
    where TaskTypeId = 1
    and MappingName = 'AZ_Storage_CSV_AZ_SQL_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Csv'
    and TargetType = 'Table';
-- AzureBlobStorage_Json_AzureSqlTable
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Json_AzureSqlTable'
    where TaskTypeId = 1
    and MappingName = 'AZ_Storage_JSON_AZ_SQL_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Json'
    and TargetType = 'Table';
-- AzureBlobStorage_Excel_AzureBlobStorage_DelimitedText
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Excel_AzureBlobStorage_DelimitedText'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Excel_AZ_Storage_CSV_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Excel'
    and TargetType = 'Csv';
-- AzureBlobFS_Excel_AzureBlobFS_DelimitedText
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobFS_Excel_AzureBlobFS_DelimitedText'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Excel_AZ_Storage_CSV_IRA'
    and SourceSystemType = 'ADLS'
    and TargetSystemType = 'ADLS'
    and SourceType = 'Excel'
    and TargetType = 'Csv';
-- AzureSqlTable_NA_AzureBlobStorage_Parquet
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureSqlTable_NA_AzureBlobStorage_Parquet'
    where TaskTypeId = 3
    and MappingName = 'AZ_SQL_AZ_Storage_Parquet_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- AzureSqlTable_NA_AzureBlobFS_Parquet
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureSqlTable_NA_AzureBlobFS_Parquet'
    where TaskTypeId = 3
    and MappingName = 'AZ_SQL_AZ_Storage_Parquet_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'ADLS'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- AzureBlobStorage_Json_AzureSqlTable
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Json_AzureSqlTable'
    where TaskTypeId = 3
    and MappingName = 'AZ_Storage_JSON_AZ_SQL_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Json'
    and TargetType = 'Table';
-- AzureBlobStorage_Parquet_AzureSqlTable
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Parquet_AzureSqlTable'
    where TaskTypeId = 1
    and MappingName = 'AZ_Storage_Parquet_AZ_SQL_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Parquet'
    and TargetType = 'Table';
-- AzureBlobFS_Parquet_AzureSqlTable
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobFS_Parquet_AzureSqlTable'
    where TaskTypeId = 1
    and MappingName = 'AZ_Storage_Parquet_AZ_SQL_IRA'
    and SourceSystemType = 'ADLS'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Parquet'
    and TargetType = 'Table';
-- GEN_File_Binary_AZ_Storage_Binary_IRA
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'GEN_File_Binary_AZ_Storage_Binary_IRA'
    where TaskTypeId = 2
    and MappingName = 'GEN_File_Binary_AZ_Storage_Binary_IRA'
    and SourceSystemType = 'File'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Binary'
    and TargetType = 'Binary';
-- GEN_File_Binary_AZ_Storage_Binary_IRB
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'GEN_File_Binary_AZ_Storage_Binary_IRB'
    where TaskTypeId = 2
    and MappingName = 'GEN_File_Binary_AZ_Storage_Binary_IRB'
    and SourceSystemType = 'File'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Binary'
    and TargetType = 'Binary';
-- OnP_SQL_AZ_Storage_Parquet_IRB
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'OnP_SQL_AZ_Storage_Parquet_IRB'
    where TaskTypeId = 3
    and MappingName = 'OnP_SQL_AZ_Storage_Parquet_IRB'
    and SourceSystemType = 'SQL Server'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- OnP_SQL_GEN_File_Parquet_IRB
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'OnP_SQL_GEN_File_Parquet_IRB'
    where TaskTypeId = 3
    and MappingName = 'OnP_SQL_GEN_File_Parquet_IRB'
    and SourceSystemType = 'SQL Server'
    and TargetSystemType = 'File'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- Create_Task_Master_AZ_SQL_IRA
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    where TaskTypeId = 8
    and MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- Create_Task_Master_AZ_SQL_IRA
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    where TaskTypeId = 8
    and MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Parquet'
    and TargetType = 'Table';
-- Create_Task_Master_AZ_SQL_IRA
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    where TaskTypeId = 8
    and MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'ADLS'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- Create_Task_Master_AZ_SQL_IRA
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    where TaskTypeId = 8
    and MappingName = 'Create_Task_Master_AZ_SQL_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'ADLS'
    and SourceType = 'Parquet'
    and TargetType = 'Table';
-- Create_Task_Master_AZ_SQL_IRB
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Create_Task_Master_AZ_SQL_IRB'
    where TaskTypeId = 8
    and MappingName = 'Create_Task_Master_AZ_SQL_IRB'
    and SourceSystemType = 'SQL Server'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Table'
    and TargetType = 'Parquet';
-- Create_Task_Master_AZ_SQL_IRB
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Create_Task_Master_AZ_SQL_IRB'
    where TaskTypeId = 8
    and MappingName = 'Create_Task_Master_AZ_SQL_IRB'
    and SourceSystemType = 'SQL Server'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Parquet'
    and TargetType = 'Table';
-- AzureBlobStorage_Binary_AzureBlobStorage_Binary
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Binary_AzureBlobStorage_Binary'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Binary_AZ_Storage_Binary_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Parquet'
    and TargetType = 'Parquet';
-- AzureBlobStorage_Binary_AzureBlobFS_Binary
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Binary_AzureBlobFS_Binary'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Binary_AZ_Storage_Binary_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'ADLS'
    and SourceType = 'Parquet'
    and TargetType = 'Parquet';
-- AzureBlobFS_Binary_AzureBlobStorage_Binary
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobFS_Binary_AzureBlobStorage_Binary'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Binary_AZ_Storage_Binary_IRA'
    and SourceSystemType = 'ADLS'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'Parquet'
    and TargetType = 'Parquet';
-- AzureBlobFS_Binary_AzureBlobFS_Binary
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobFS_Binary_AzureBlobFS_Binary'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Binary_AZ_Storage_Binary_IRA'
    and SourceSystemType = 'ADLS'
    and TargetSystemType = 'ADLS'
    and SourceType = 'Parquet'
    and TargetType = 'Parquet';
-- AzureSqlSource_SQL
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureSqlSource_SQL'
    where TaskTypeId = 7
    and MappingName = 'AZ_SQL_StoredProcedure_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'StoredProcedure'
    and TargetType = 'StoredProcedure';
-- AZ_Storage_SAS_Uri_SMTP_Email
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AZ_Storage_SAS_Uri_SMTP_Email'
    where TaskTypeId = 9
    and MappingName = 'AZ_Storage_SAS_Uri_SMTP_Email'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'SendGrid'
    and SourceType = 'SASUri'
    and TargetType = 'Email';
-- AZ_Storage_Cache_File_List
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AZ_Storage_Cache_File_List'
    where TaskTypeId = 10
    and MappingName = 'AZ_Storage_Cache_File_List'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure SQL'
    and SourceType = 'Filelist'
    and TargetType = 'Table';
-- AzureBlobStorage_Binary_AzureBlobStorage_Binary
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureBlobStorage_Binary_AzureBlobStorage_Binary'
    where TaskTypeId = 2
    and MappingName = 'AZ_Storage_Binary_AZ_Storage_Binary_IRA'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'Azure Blob'
    and SourceType = '*'
    and TargetType = '*';
-- Cache_File_List_To_Email_Alert
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'Cache_File_List_To_Email_Alert'
    where TaskTypeId = 11
    and MappingName = 'Cache_File_List_To_Email_Alert'
    and SourceSystemType = 'Azure Blob'
    and TargetSystemType = 'SendGrid'
    and SourceType = '*'
    and TargetType = '*';
-- AzureSqlTable_NA_AzureBlobStorage_Parquet
    Update [dbo].[TaskTypeMapping]
    Set MappingName = 'AzureSqlTable_NA_AzureBlobStorage_Parquet'
    where TaskTypeId = 3
    and MappingName = 'AZ_SQL_AZ_Storage_Parquet_IRA'
    and SourceSystemType = 'Azure SQL'
    and TargetSystemType = 'Azure Blob'
    and SourceType = 'SQL'
    and TargetType = 'Parquet';


--Update Task Instance Json Shcemas
Update [dbo].[TaskTypeMapping] 
Set TaskInstanceJsonSchema = '
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "type": "object",
  "properties": {
    "TargetRelativePath": {
      "type": "string"
    },
    "IncrementalField": {
      "type": "string"
    },
    "IncrementalColumnType": {
      "type": "string"
    },
    "IncrementalValue": {
      "type": "string"
    }
  },
  "required": [
    "TargetRelativePath"
  ]
}'
where mappingname in ('AzureSqlTable_NA_AzureBlobStorage_Parquet', 'AzureSqlTable_NA_AzureBlobFS_Parquet')