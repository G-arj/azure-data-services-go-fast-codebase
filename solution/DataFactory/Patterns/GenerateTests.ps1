$patterns = ((Get-Content "Patterns.json") | ConvertFrom-Json).Folder | Get-Unique
$CurDir = $PWD.ToString()

$AllTests = @()
$counter = 0
foreach ($pattern in $patterns) {   

    Write-Host "_____________________________"
    Write-Host  $pattern
    Write-Host "_____________________________"
    $folder = "/pipeline/" + $pattern + "/functionapptests"

    Set-Location -path ($CurDir + $folder)
    $testfile = "./tests/tests.json"
    jsonnet "./GenerateTests.jsonnet" | Set-Content($testfile)
    $testfilejson = Get-Content $testfile | ConvertFrom-Json | ForEach-Object {
        $_.TaskMasterId = $counter
        $AllTests += $_
        $counter += 1
    }    

}

Set-Location -path ($CurDir + '../../../')
Write-Host $PWD.ToString()
$AllTests | ConvertTo-Json -Depth 10 | Set-Content -Path  ($PWD.ToString() + '/FunctionApp/consoleapp/UnitTests/tests.json')
Set-Location $CurDir