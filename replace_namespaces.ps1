$files = Get-ChildItem -Path "d:\MVCProjects\Smart Platform" -Include "*.cs","*.cshtml" -Recurse -File | Where-Object { $_.FullName -notmatch "\\(bin|obj|\.git)\\" }

foreach ($file in $files) {
    # Skip the old ViewModel and old Implementation repository to avoid modifying files we are going to delete
    if ($file.FullName -match "SmartPlatform\.Application\\Common\\ViewModels") { continue }
    if ($file.FullName -match "SmartPlatform\.Infrastructure\\Repositories\\Implementation\\GenericRepository\.cs") { continue }
    
    $content = [System.IO.File]::ReadAllText($file.FullName)
    $updated = $false
    
    if ($content.Contains("SmartPlatform.Application.Common.ViewModels")) {
        $content = $content.Replace("SmartPlatform.Application.Common.ViewModels", "SmartPlatform.Application.DTOs")
        $updated = $true
    }
    
    if ($content.Contains("SmartPlatform.Infrastructure.Repositories.Implementation")) {
        $content = $content.Replace("SmartPlatform.Infrastructure.Repositories.Implementation", "SmartPlatform.Infrastructure.Repositories")
        $updated = $true
    }
    
    if ($updated) {
        [System.IO.File]::WriteAllText($file.FullName, $content)
        Write-Host "Updated: $($file.FullName)"
    }
}
