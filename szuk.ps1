Get-ChildItem -Path (Get-Location).Path -Recurse -File |
    Select-String -Pattern 'Avocado-model.Controllers' -SimpleMatch -List |
    ForEach-Object { "$($_.Path):$($_.LineNumber) `t $($_.Line.Trim())" }