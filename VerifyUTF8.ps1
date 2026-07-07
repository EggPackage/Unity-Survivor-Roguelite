$dir = "F:\DawnBrewDuskSpirits\Assets\Scripts"
$bad = 0
$files = Get-ChildItem -Path $dir -Filter "*.cs" -Recurse
foreach ($f in $files) {
    $b = [System.IO.File]::ReadAllBytes($f.FullName)
    if (-not ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF)) {
        Write-Host ("缺少BOM: " + $f.Name)
        $bad++
    }
}
Write-Host ""
Write-Host ("总文件数: " + $files.Count)
Write-Host ("缺失BOM文件数: " + $bad)