$ErrorActionPreference = "Stop"
$dir = "F:\DawnBrewDuskSpirits\Assets\Scripts"
$files = Get-ChildItem -Path $dir -Filter "*.cs" -Recurse
$total = $files.Count
$bom = 0
$nobom = 0
$gbk = 0
$changed = 0
$utf8Strict = New-Object System.Text.UTF8Encoding -ArgumentList ($false, $true)
$utf8BOM = New-Object System.Text.UTF8Encoding -ArgumentList ($true)
$gb = [System.Text.Encoding]::GetEncoding("gb2312")

Write-Host "--- 开始处理 ---"
Write-Host ("扫描 .cs 文件: " + $total)

foreach ($f in $files) {
    $bytes = [System.IO.File]::ReadAllBytes($f.FullName)
    $hasBOM = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF

    if ($hasBOM) {
        $bom++
        continue
    }

    try {
        $content = $utf8Strict.GetString($bytes)
        [System.IO.File]::WriteAllText($f.FullName, $content, $utf8BOM)
        $nobom++
        $changed++
    } catch {
        $text = $gb.GetString($bytes)
        [System.IO.File]::WriteAllText($f.FullName, $text, $utf8BOM)
        $gbk++
        $changed++
    }
}

Write-Host ""
Write-Host "--- 处理完成 ---"
Write-Host ("已是 UTF-8 BOM: " + $bom)
Write-Host ("已添加 BOM (原 UTF-8): " + $nobom)
Write-Host ("已转换 GB2312->UTF-8: " + $gbk)
Write-Host ("总计修改文件数: " + $changed)