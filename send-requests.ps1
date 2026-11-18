param(
    [uri]$BaseUrl = 'http://localhost:5180'
)

if (-not $BaseUrl.IsAbsoluteUri -or $BaseUrl.Scheme -notin @('http', 'https')) {
    throw 'BaseUrl must be an absolute HTTP or HTTPS URL.'
}

$ErrorActionPreference = 'Stop'

foreach ($route in @(@{ Path = '/weather'; Count = 1000 }, @{ Path = '/forecast'; Count = 10 })) {
    $url = [uri]::new($BaseUrl, $route.Path)
    for ($request = 0; $request -lt $route.Count; $request++) {
        Invoke-RestMethod -Uri $url -Method Get | Out-Null
    }
    Write-Host "Sent $($route.Count) requests to $url"
}