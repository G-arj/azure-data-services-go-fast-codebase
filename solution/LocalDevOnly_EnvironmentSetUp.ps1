#az login 
#az account set -s "jorampon internal consumption"
#$DebugPreference = "Continue"
#$DebugPreference = "SilentlyContinue"

Set-Location ./Deployment/workflows

[Environment]::SetEnvironmentVariable("ENVIRONMENT_NAME", "development")
. .\Steps\PushEnvFileIntoVariables.ps1
ParseEnvFile("$env:ENVIRONMENT_NAME")
Invoke-Expression -Command  ".\Steps\CD_SetResourceGroupHash.ps1" 


#Load Secrets into Environment Variables 
ParseSecretsFile ($SecretFile) 

Set-Location ./../../
