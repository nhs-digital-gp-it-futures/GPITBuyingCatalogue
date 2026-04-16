param(
    [Parameter(Mandatory = $true)]
    [string]$BaseUrl
)

$BaseUrl = $BaseUrl.TrimEnd('/')

function Invoke-Test {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Url
    )

    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing
        $actualStatus = $response.StatusCode

        Write-Host "URL under test: $Url"
        Write-Host "Response status: $actualStatus"

        if ($actualStatus -eq 200) {
            Write-Host "OK: status check passed"
            return $true
        }

        Write-Host "FAIL: expected 200 but got $status"
        return $false
    }
    catch {
        Write-Host "FAIL: request could not be completed for $Url"
        Write-Host $_.Exception.Message
        return $false
    }
}

$urlsToTest = @(
    $BaseUrl,
    "$BaseUrl/catalogue-solutions"
)

$allPassed = $true

foreach ($url in $urlsToTest) {
    if (-not (Invoke-Test -Url $url)) {
        $allPassed = $false
    }
}

if ($allPassed) {
    Write-Host "OK: all status checks passed"
    exit 0
}
else {
    Write-Host "FAIL: one or more status checks failed"
    exit 1
}
