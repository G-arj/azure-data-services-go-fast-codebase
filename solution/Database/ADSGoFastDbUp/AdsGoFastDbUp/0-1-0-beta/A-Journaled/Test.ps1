$str = `
@"
TaskTypeMappingId,TaskTypeId,MappingType,MappingName,SourceSystemType,SourceType,TargetSystemType,TargetType,TaskDatafactoryIR,NewMappingName
1,1,ADF,AZ_Storage_Excel_AZ_SQL_IRA,Azure Blob,Excel,Azure SQL,Table,IRA,AzureBlobStorage_Excel_AzureSqlTable
2,1,ADF,AZ_Storage_CSV_AZ_SQL_IRA,Azure Blob,Csv,Azure SQL,Table,IRA,AzureBlobStorage_CSV_AzureSqlTable
3,1,ADF,AZ_Storage_JSON_AZ_SQL_IRA,Azure Blob,Json,Azure SQL,Table,IRA,AzureBlobStorage_Json_AzureSqlTable
4,2,ADF,AZ_Storage_Excel_AZ_Storage_CSV_IRA,Azure Blob,Excel,Azure Blob,Csv,IRA,AzureBlobStorage_Excel_AzureBlobStorage_DelimitedText
5,2,ADF,AZ_Storage_Excel_AZ_Storage_CSV_IRA,ADLS,Excel,ADLS,Csv,IRA,AzureBlobFS_Excel_AzureBlobFS_DelimitedText
6,3,ADF,AZ_SQL_AZ_Storage_Parquet_IRA,Azure SQL,Table,Azure Blob,Parquet,IRA,AzureSqlTable_NA_AzureBlobStorage_Parquet
7,3,ADF,AZ_SQL_AZ_Storage_Parquet_IRA,Azure SQL,Table,ADLS,Parquet,IRA,AzureSqlTable_NA_AzureBlobFS_Parquet
8,3,ADF,AZ_Storage_JSON_AZ_SQL_IRA,Azure Blob,Json,Azure SQL,Table,IRA,AzureBlobStorage_Json_AzureSqlTable
9,1,ADF,AZ_Storage_Parquet_AZ_SQL_IRA,Azure Blob,Parquet,Azure SQL,Table,IRA,AzureBlobStorage_Parquet_AzureSqlTable
10,1,ADF,AZ_Storage_Parquet_AZ_SQL_IRA,ADLS,Parquet,Azure SQL,Table,IRA,AzureBlobFS_Parquet_AzureSqlTable
11,2,ADF,GEN_File_Binary_AZ_Storage_Binary_IRA,File,Binary,Azure Blob,Binary,IRA,GEN_File_Binary_AZ_Storage_Binary_IRA
12,2,ADF,GEN_File_Binary_AZ_Storage_Binary_IRB,File,Binary,Azure Blob,Binary,IRB,GEN_File_Binary_AZ_Storage_Binary_IRB
13,3,ADF,OnP_SQL_AZ_Storage_Parquet_IRB,SQL Server,Table,Azure Blob,Parquet,IRB,OnP_SQL_AZ_Storage_Parquet_IRB
14,3,ADF,OnP_SQL_GEN_File_Parquet_IRB,SQL Server,Table,File,Parquet,IRB,OnP_SQL_GEN_File_Parquet_IRB
15,8,ADF,Create_Task_Master_AZ_SQL_IRA,Azure SQL,Table,Azure Blob,Parquet,IRA,Create_Task_Master_AZ_SQL_IRA
16,8,ADF,Create_Task_Master_AZ_SQL_IRA,Azure SQL,Parquet,Azure Blob,Table,IRA,Create_Task_Master_AZ_SQL_IRA
17,8,ADF,Create_Task_Master_AZ_SQL_IRA,Azure SQL,Table,ADLS,Parquet,IRA,Create_Task_Master_AZ_SQL_IRA
18,8,ADF,Create_Task_Master_AZ_SQL_IRA,Azure SQL,Parquet,ADLS,Table,IRA,Create_Task_Master_AZ_SQL_IRA
19,8,ADF,Create_Task_Master_AZ_SQL_IRB,SQL Server,Table,Azure Blob,Parquet,IRB,Create_Task_Master_AZ_SQL_IRB
20,8,ADF,Create_Task_Master_AZ_SQL_IRB,SQL Server,Parquet,Azure Blob,Table,IRB,Create_Task_Master_AZ_SQL_IRB
21,2,ADF,AZ_Storage_Binary_AZ_Storage_Binary_IRA,Azure Blob,Parquet,Azure Blob,Parquet,IRA,AzureBlobStorage_Binary_AzureBlobStorage_Binary
22,2,ADF,AZ_Storage_Binary_AZ_Storage_Binary_IRA,Azure Blob,Parquet,ADLS,Parquet,IRA,AzureBlobStorage_Binary_AzureBlobFS_Binary
23,2,ADF,AZ_Storage_Binary_AZ_Storage_Binary_IRA,ADLS,Parquet,Azure Blob,Parquet,IRA,AzureBlobFS_Binary_AzureBlobStorage_Binary
24,2,ADF,AZ_Storage_Binary_AZ_Storage_Binary_IRA,ADLS,Parquet,ADLS,Parquet,IRA,AzureBlobFS_Binary_AzureBlobFS_Binary
25,7,ADF,AZ_SQL_StoredProcedure_IRA,Azure SQL,StoredProcedure,Azure SQL,StoredProcedure,IRA,AzureSqlSource_SQL
26,9,AF,AZ_Storage_SAS_Uri_SMTP_Email,Azure Blob,SASUri,SendGrid,Email,N/A,AZ_Storage_SAS_Uri_SMTP_Email
27,10,AF,AZ_Storage_Cache_File_List,Azure Blob,Filelist,Azure SQL,Table,N/A,AZ_Storage_Cache_File_List
29,2,ADF,AZ_Storage_Binary_AZ_Storage_Binary_IRA,Azure Blob,*,Azure Blob,*,IRA,AzureBlobStorage_Binary_AzureBlobStorage_Binary
30,11,AF,Cache_File_List_To_Email_Alert,Azure Blob,*,SendGrid,*,*,Cache_File_List_To_Email_Alert
31,3,ADF,AZ_SQL_AZ_Storage_Parquet_IRA,Azure SQL,SQL,Azure Blob,Parquet,IRA,AzureSqlTable_NA_AzureBlobStorage_Parquet
"@ | ConvertFrom-Csv 

foreach ($item in $str) 
{
    $out =  @"
    Update [dbo].[TaskTypeMapping]
    Set MappingName = '{NewMappingName}'
    where TaskTypeId = {TaskTypeId}
    and MappingName = '{MappingName}'
    and SourceSystemType = '{SourceSystemType}'
    and TargetSystemType = '{TargetSystemType}'
    and SourceType = '{SourceType}'
    and TargetType = '{TargetType}';
"@ 
    $out = $out.replace("{NewMappingName}",$item.NewMappingName)
    $out = $out.replace("{SourceSystemType}",$item.SourceSystemType)
    $out = $out.replace("{SourceType}",$item.SourceType)
    $out = $out.replace("{TargetSystemType}",$item.TargetSystemType)
    $out = $out.replace("{TargetType}",$item.TargetType)
    $out = $out.replace("{MappingName}",$item.MappingName)    
    $out = $out.replace("{TaskTypeId}",$item.TaskTypeId)
    
    write-host "--" $item.NewMappingName
    write-host $out
}