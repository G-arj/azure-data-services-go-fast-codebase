function(GFPIR="IRA")
{
    "name": "GLS_SqlServerDatabase_" + GFPIR,
    "type": "Microsoft.DataFactory/factories/linkedservices",
    "properties": {
        "parameters": {
            "KeyVaultBaseUrl": {
                "type": "string"
            },
            "PasswordSecret": {
                "type": "string"
            },
            "Server": {
                "type": "string"
            },
            "Database": {
                "type": "string"
            },
            "UserName": {
                "type": "string"
            }
        },
        "annotations": [],
        "type": "SqlServer",
        "typeProperties": {
            "connectionString": "Integrated Security=False;Data Source=@{linkedService().Server};Initial Catalog=@{linkedService().Database};User ID=@{linkedService().UserName}",
            "password": {
                "type": "AzureKeyVaultSecret",
                "store": {
                    "referenceName": "GenericAzureKeyVault",
                    "type": "LinkedServiceReference",
                    "parameters": {
                        "KeyVaultBaseUrl": {
                            "value": "@linkedService().KeyVaultBaseUrl",
                            "type": "Expression"
                        }
                    }
                },
                "secretName": {
                    "value": "@linkedService().PasswordSecret",
                    "type": "Expression"
                }
            }
        },
        "connectVia": {
            "referenceName": GFPIR,
            "type": "IntegrationRuntimeReference"
        }
    }
}