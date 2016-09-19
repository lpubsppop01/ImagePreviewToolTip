# ImagePreviewToolTip Visual Studio Extension
This Visual Studio extension provides image preview tooltips for path texts quoted by `"` or `'`.

## Feature Details
* Supported path formats:
    + Relative paths from current file, e.g. "foo/bar.jpg".
    + Data URI scheme, e.g. "data:image/png;base64,baz".
    + Other formats supported by `System.Windows.Media.Imaging.BitmapFrameCreate(Uri)` method.
* Works in several windows categorized to `ContentType("Code")`.
* On/Off by "Toggle ImagePreviewToolTip" in Tools menu.

## Requirements
Visual Studio 2013 or later.

## License
MIT License. Please see the license file `LICENSE.txt`.