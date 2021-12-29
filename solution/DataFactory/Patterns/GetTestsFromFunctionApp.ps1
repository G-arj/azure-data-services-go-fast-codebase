$items = Get-ChildItem -Path "../../FunctionApp/consoleapp/UnitTestResults/Todo/" 

#$arr = @()

foreach($item in $items)
{
    $testfromjson = $item | Get-Content | ConvertFrom-Json 
    ($item | Get-Content) | Set-Content('FileForUpload.json')
    echo az datafactory pipeline create-run --factory-name $env:AdsOpts_CD_Services_DataFactory_Name --parameters '@FileForUpload.json' --name $testfromjson.TaskObject.DataFactory.ADFPipeline --resource-group $env:AdsOpts_CD_ResourceGroup_Name 
    az datafactory pipeline create-run --factory-name $env:AdsOpts_CD_Services_DataFactory_Name --parameters '@FileForUpload.json' --name $testfromjson.TaskObject.DataFactory.ADFPipeline --resource-group $env:AdsOpts_CD_ResourceGroup_Name 
}


