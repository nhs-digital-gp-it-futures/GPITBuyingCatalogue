$projectPath=$args[0]
$projectPath="Z:\repos\nhs\GPITBuyingCatalogue\database\NHSD.GPITBuyingCatalogue.Database"
$postDeploymentPath = "$($projectPath)$($slash)\PostDeployment"
$slash = [IO.Path]::DirectorySeparatorChar

$outFile = "$projectPath\PostDeployment\PostDeploymentBuilt.sql"
 
cls
if((Test-Path $outFile) -eq $true) {Remove-Item -Path $outFile -Force}

New-Item -ItemType file -Path $outFile -Force | Out-Null

Add-Content -Path $outFile "----------------------------------------------------------------------------------------------------------------------------------------------------------------"
Add-Content -Path $outFile "--      IMPORTANT - please do not manually edit this file.  It is generated automatically on build.  Any changes made will be lost."
Add-Content -Path $outFile "----------------------------------------------------------------------------------------------------------------------------------------------------------------"

foreach($line in Get-Content -ErrorAction Stop $postDeploymentPath\PostDeployment.sql) {
    $trimmedLine = $line.Trim()

    if ($trimmedLine.StartsWith(":r ")) {
        $script = $trimmedLine.Replace(":r ", "")

        Write-Host "Appending file $script..." -ForegroundColor Gray
        $content = Get-Content -ErrorAction Stop -Path "$($postDeploymentPath)$($slash)$($script)"
        Add-Content -Path $outFile "----------------------------------------------------------------------------------------------------------------------------------------------------------------"
        Add-Content -Path $outFile "--      $script"
        Add-Content -Path $outFile "----------------------------------------------------------------------------------------------------------------------------------------------------------------"
        Add-Content -Path $outFile $content        
        Add-Content -Path $outFile "GO`r`n"
    }
}

if($error.Count -gt 0) {

  exit 1
}

Write-Host "Completed file $outFile" -ForegroundColor DarkGreen
