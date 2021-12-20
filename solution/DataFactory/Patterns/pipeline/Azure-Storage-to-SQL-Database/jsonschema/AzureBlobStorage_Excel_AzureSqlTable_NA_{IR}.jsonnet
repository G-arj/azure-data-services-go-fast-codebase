{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "object",
    "title": "TaskMasterJson",
    "properties": {
        "Source": {
            "type": "object",
            "properties": {
                "Type": {
                    "type": "string",                    
                    "enum": [
                        "Excel"
                    ], 
                    "options":{
                        "hidden": true
                    }
                },
                "RelativePath": {
                    "type": "string",
                    "options": {
                        "inputAttributes": {
                            "placeholder": "eg. AwSample/dbo/Customer/{yyyy}/{MM}/{dd}/{hh}/"
                        },
                        "infoText": "Path of the file to be imported."
                    }
                },
                "DataFileName": {
                    "type": "string",
                    "options": {
                        "inputAttributes": {
                            "placeholder": "eg. Customer.xlsx"
                        },
                        "infoText": "Name of the file to be imported."
                    }
                },
                "SchemaFileName": {
                    "type": "string",
                    "options": {
                        "inputAttributes": {
                            "placeholder": "eg. Customer.json"
                        },
                        "infoText": "Name of the schema file to create. Generally this is the same name as your DataFileName but with a .json extension."
                    }
                },
                "FirstRowAsHeader": {
                    "type": "string",
                    "enum": [
                        "true",
                        "false"
                    ],
                    "default": "true",
                    "options": {                        
                        "infoText": "Set to true if you want the first row of data to be used as column names."
                    }
                },
                "SheetName": {
                    "type": "string",
                    "options": {
                        "inputAttributes": {
                            "placeholder": "eg. Sheet1"
                        },
                        "infoText": "Name of the Excel Worksheet that you wish to import"
                    }
                }
            },
            "required": [
                "Type",
                "RelativePath",
                "DataFileName",
                "SchemaFileName",
                "FirstRowAsHeader",
                "SheetName"
            ]
        },
        "Target": {
            "type": "object",
            "properties": {
                "Type": {
                    "type": "string",                    
                    "enum": [
                        "Table"
                    ], 
                    "options":{
                        "hidden": true
                    }
                },
                "StagingTableSchema": {
                    "type": "string",
                    "options": {                        
                        "inputAttributes": {
                            "placeholder": "eg. dbo"
                        },
                        "infoText": "Schema for the transient table in which data will first be staged before being merged into final target table."
                    }
                },
                "StagingTableName": {
                    "type": "string",
                    "options": {                        
                        "inputAttributes": {
                            "placeholder": "eg. StgCustomer"
                        },
                        "infoText": "Table name for the transient table in which data will first be staged before being merged into final target table."
                    }
                },
                "AutoCreateTable": {
                    "type": "string",
                    "enum": [
                        "true",
                        "false"
                    ],
                    "default": "true",
                    "options": {
                        "infoText": "Set to true if you want the framework to automatically create the target table if it does not exist. If this is false and the target table does not exist then the task will fail with an error."
                    }
                },
                "TableSchema": {
                    "type": "string",
                    "options": {                        
                        "inputAttributes": {
                            "placeholder": "eg. dbo"
                        },
                        "infoText": "Schema of the final target table."
                    }
                },
                "TableName": {
                    "type": "string",
                    "options": {                        
                        "inputAttributes": {
                            "placeholder": "eg. Customer"
                        },
                        "infoText": "Name of the final target table."
                    }
                },
                "PreCopySQL": {
                    "type": "string",
                    "options": {
                        "inputAttributes": {
                            "placeholder": "eg. Delete from dbo.StgCustomer where Active = 0"
                        },
                        "infoText": "A SQL statement that you wish to be applied prior to merging the staging table and the final table"
                    }
                },
                "PostCopySQL": {
                    "type": "string",
                    "options": {
                        "inputAttributes": {
                            "placeholder": "eg. Delete from dbo.Customer where Active = 0"
                        },
                        "infoText": "A SQL statement that you wish to be applied after merging the staging table and the final table"
                    }
                },
                "AutoGenerateMerge": {
                    "type": "string",
                    "enum": [
                        "true",
                        "false"
                    ],
                    "default": "true",
                    "options": {
                        "infoText": "Set to true if you want the framework to autogenerate the merge based on the primary key of the target table."
                    }
                },
                "MergeSQL": {
                    "type": "string",
                    "format": "sql",                                      
                    "options": {                                                
                        "infoText": "A custom merge statement to exectute. Note that this will be ignored if 'AutoGenerateMerge' is true. Click in the box below to view or edit ",
                        "ace": {                            
                            "tabSize": 2,
                            "useSoftTabs": true,
                            "wrap": true
                          }
                    }
                }
            },
            "required": [
                "Type",
                "StagingTableSchema",
                "StagingTableName",
                "AutoCreateTable",
                "TableSchema",
                "TableName",
                "PreCopySQL",
                "PostCopySQL",
                "AutoGenerateMerge"
            ]
        }
    },
    "required": [
        "Source",
        "Target"
    ]
}