Write-Host "----If there are errors between these lines, Teamviewer upload failed, update it manually.----" -ForegroundColor Yellow

[string]$tvid = Get-ItemPropertyValue 'HKLM:\SOFTWARE\WOW6432Node\TeamViewer' -Name ClientID

$Body = @{
    hostName = Hostname
    teamViewerID = $tvid
}

$JsonBody = $Body | ConvertTo-Json

 $Params = @{
     Method = "Post"
     Uri = "http://localhost:5000/TeamViewer"
     Body = $JsonBody
     ContentType = "application/json"
 }

 Invoke-RestMethod @Params

Write-Host "----If there are errors between these lines, Teamviewer upload failed, update it manually.----" -ForegroundColor Yellow
pause