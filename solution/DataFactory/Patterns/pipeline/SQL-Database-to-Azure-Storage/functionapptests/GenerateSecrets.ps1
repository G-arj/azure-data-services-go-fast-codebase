
$adsopts = (gci env:* | sort-object name | Where-Object { $_.Name -like "AdsOpts*" })
$obj =  @{}
foreach ($opts in $adsopts) {
    $obj | Add-Member $opts.Name $opts.Value
}

$SubscriptionId = $obj.AdsOpts_CD_ResourceGroup_Id.Split("/")
$obj | Add-Member "AdsOpts_CD_Services_DataFactory_SubscriptionId" $SubscriptionId[2]

$obj | ConvertTo-Json | Set-Content ('./partials/secrets.libsonnet')
