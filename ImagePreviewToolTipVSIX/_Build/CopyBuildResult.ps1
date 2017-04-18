if ($args.Length -gt 0) {
    $configulationName = $args[0]
    if ($configulationName -ine "Publish") {
        return
    }
}

$scriptDirPath = Split-Path $MyInvocation.MyCommand.Path -Parent
$srcFilePath = Join-Path $scriptDirPath "..\bin\Publish\lpubsppop01.ImagePreviewToolTipVSIX.vsix"
$destFilePath = Join-Path $scriptDirPath "..\..\ImagePreviewToolTipVSIX.vsix"
copy $srcFilePath $destFilePath
