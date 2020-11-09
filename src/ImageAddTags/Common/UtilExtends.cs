using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ImageAddTags.Common
{
    static class UtilExtends
    {

        public static BitmapImage MatToBitmapImage(this Mat image)
        {
            var bitmap = image.ToBitmap();
            using var ms = new MemoryStream();

            bitmap.Save(ms, ImageFormat.Png);

            BitmapImage result = new BitmapImage();
            result.BeginInit();
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = ms;
            result.EndInit();

            return result;
        }

    }
}
