$old = $args[0]
$new = $args[1]

if ($args.Count -lt 2)
{
    Write-Host "Usage:"
    Write-Host "    .\UpdateFilterValue.ps1 <old> <new>"
    Write-Host ""
    Write-Host "Example:"
    Write-Host "    .\UpdateFilterValue.ps1 2017-2018 2018-2019"
    Write-Host ""
}
else
{
    Write-Host "Replacing $old with $new..."

    Get-ChildItem "*.xml" | Foreach-Object { 
        (Get-Content $_ | ForEach { $_ -replace "<value>$old</value>", "<value>$new</value>"}) | Set-Content $_
    }

    Write-Host "Done!"
}
