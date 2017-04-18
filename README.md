# Image Preview ToolTip Visual Studio Extension
This Visual Studio Extension provides image preview tooltips for path texts quoted by `"` or `'`.

## Features
* Supported path formats:
    + Relative paths from a current file, e.g. "foo/bar.jpg".
    + Data URI scheme, e.g. "data:image/png;base64,baz".
    + Other formats supported by `System.Windows.Media.Imaging.BitmapFrame.Create(Uri)` method.
* Works in several windows categorized as `ContentType("Code")`.
* On/Off by "Toggle Image Preview ToolTip" in Tools menu.

## Download
[ImagePreviewToolTipVSIX.vsix](https://github.com/lpubsppop01/ImagePreviewToolTipVSIX/raw/master/ImagePreviewToolTipVSIX.vsix)

## Requirements
Visual Studio 2013 or later

## Author
[lpubsppop01](https://github.com/lpubsppop01)

## License
[MIT License](https://github.com/lpubsppop01/ImagePreviewToolTipVSIX/raw/master/LICENSE.txt)
