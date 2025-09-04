task Prepare_CSharpProject Clean_CSharpProject, Restore_CSharpProject, Build_CSharpProject, Publish_CSharpProject, PostPublish

task PostPublish {
    $PublishDestination = (Join-Path -Path $buildRoot -ChildPath 'src' -AdditionalChildPath 'bin','Release','netstandard2.0','publish')

    # Copy PSMailKit.psd1
    Copy-Item -Path (Join-Path -Path $buildRoot -ChildPath 'PSMailKit.psd1') -Destination $PublishDestination

    # Copy Formats
    Copy-Item -Path (Join-Path -Path $buildRoot -ChildPath 'Formats') -Destination $PublishDestination -Recurse -Filter "*.Format.ps1xml" -Force

    # Copy maml
    Copy-Item -Path (Join-Path -Path $buildRoot -ChildPath 'maml' -AdditionalChildPath 'PSMailKit','PSMailKit-Help.xml') -Destination $PublishDestination

    # Removing PSMailKit.pdb as it stores sensitive data
    Get-Item -Path (Join-Path -Path $buildRoot -ChildPath 'src' -AdditionalChildPath 'bin','Release','netstandard2.0','publish','PSMailKit.pdb') | Remove-Item -Force
}

task Publish_CSharpProject {
    Push-Location -Path (Join-Path -Path $buildRoot -ChildPath 'src')
    dotnet publish --configuration Release --no-restore
    Pop-Location
}

task Build_CSharpProject {
    Push-Location -Path (Join-Path -Path $buildRoot -ChildPath 'src')
    dotnet build --configuration Release --no-restore
    Pop-Location
}

task Restore_CSharpProject {
    Push-Location -Path (Join-Path -Path $buildRoot -ChildPath 'src')
    dotnet restore
    Pop-Location
}

task Clean_CSharpProject {
    Push-Location -Path (Join-Path -Path $buildRoot -ChildPath 'src')
    dotnet clean
    Pop-Location
}