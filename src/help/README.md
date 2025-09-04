# About help

`./help` directory is used to output results of Microsoft.PowerShell.PlatyPS cmdlets.

## Creating new markdown help file

To create new markdown help file we need to get module with desired cmdlet and then use `New-MarkdownCommandHelp`.

Import-Module "$PSScriptRoot\bin\Debug\netstandard2.0\publish\PSMailKit.psd1"
New-MarkdownCommandHelp -Module (Get-Module PSMailKit) -OutputFolder "$PSScriptRoot\help"

```Powershell
# As we using publish directory which is not in PATH, importing module so we could pass Get-Module PSMailKit to ModuleInfo
Import-Module ".\bin\Debug\netstandard2.0\publish\PSMailKit.psd1" -PassThru
$newMarkdownCommandHelpSplat = @{
    ModuleInfo = Get-Module PSMailKit
    Locale = 'en-US'
    OutputFolder = '.\help'
    HelpVersion = '1.0.0.0'
    WithModulePage = $true
}
# New-MarkdownCommandHelp will append `/PSMailKit` to OutputFolder
New-MarkdownCommandHelp @newMarkdownCommandHelpSplat
```

## Test markdown cmdlet help files

We could test created cmdlet markdown files

```Powershell
Test-MarkdownCommandHelp .\help\PSMailKit\Get-SMTPCapabilities.md -DetailView
```

## Updating maml help file

To update maml help file `.\maml\PSMailKit\PSMailKit-Help.xml` we need to get previously generated *.md CommandHelp files then import them using `Import-MarkdownCommandHelp` and then pipeline resulted objects to `Export-MamlCommandHelp`. `-Force` is used to replace existing maml file.

```Powershell
Get-ChildItem .\help\ -Recurse -Filter *.md | Measure-PlatyPSMarkdown | Where-Object FileType -Match 'CommandHelp' | Import-MarkdownCommandHelp -Path {$_.FilePath} | Export-MamlCommandHelp -OutputFolder .\maml\ -Force -Verbose
```

## Review maml help file

We could review maml help file as it would appear when output by Get-Help.

```Powershell
Show-HelpPreview -Path .\maml\PSMailKit\PSMailKit-Help.xml
```
