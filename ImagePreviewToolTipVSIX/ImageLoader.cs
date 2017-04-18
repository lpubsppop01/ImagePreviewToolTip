using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace lpubsppop01.ImagePreviewToolTipVSIX
{
    class ImageLoader
    {
        public static ImageSource Load(string imageUrl, string basePath)
        {
            try
            {
                if (imageUrl.StartsWith("data:"))
                {
                    // ref. https://gist.github.com/vbfox/484643
                    var base64Str = Regex.Match(imageUrl, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    var imageBytes = Convert.FromBase64String(base64Str);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                }
                bool absolute;
                if (IsFilePath(imageUrl, out absolute))
                {
                    var filePath = absolute ? imageUrl : Path.Combine(basePath, imageUrl);
                    using (var stream = new MemoryStream(File.ReadAllBytes(filePath)))
                    {
                        return new WriteableBitmap(BitmapFrame.Create(stream));
                    }
                }
                return BitmapFrame.Create(new Uri(imageUrl));
            }
            catch { }
            return null;
        }

        static bool IsFilePath(string imageUrl, out bool absolute)
        {
            if (Regex.Match(imageUrl, @"^[a-zA-Z]:|^//|^\\").Success)
            {
                absolute = true;
                return true;
            }
            else if (imageUrl.StartsWith("http:") || imageUrl.StartsWith("https:"))
            {
                absolute = true;
                return false;
            }
            absolute = false;
            return true;
        }
    }
}
