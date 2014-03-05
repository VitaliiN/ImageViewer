using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageViewerControl
{
    public class MyImageConverter : IValueConverter 
    {
        private static IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value is ImageSource)
                return value;
            else
            {
                string path = (string)value;
                if ((path.Length > 9) && (path.ToLower().Substring(0, 9).Equals("isostore:")))
                {
                    using (var sourceFile = isoStorage.OpenFile(path.Substring(9), FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage image = new BitmapImage();
                        image.CreateOptions = BitmapCreateOptions.None;
                        try
                        {
                            image.SetSource(sourceFile);
                        }
                        catch
                        {
                           // return new BitmapImage(new Uri("/VKMagazine;component/Icons/spash_logo.png", UriKind.Relative));
                        }

                        if (parameter != null)
                        {
                            WriteableBitmap wBM = new WriteableBitmap(image);
                            WriteableBitmap watermarkedImage = wBM.Watermark((string)parameter, Colors.White, fontSize: 50, opacity: .8, hasDropShadow: false);
                            return watermarkedImage;
                        }
                        else
                        {
                            return image;
                        }

                    }
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    image.CreateOptions = BitmapCreateOptions.None;
                    image.UriSource = new Uri(path,UriKind.RelativeOrAbsolute);
                    if (parameter != null)
                    {
                        WriteableBitmap wBM = new WriteableBitmap(image);
                        WriteableBitmap watermarkedImage = wBM.Watermark((string)parameter, Colors.White, fontSize: 50, opacity: .8, hasDropShadow: false);
                        return watermarkedImage;
                    }
                    else
                    {
                        return image;
                    }

                    //MemoryStream ms = new MemoryStream(watermarkedImage.ToByteArray());
                    //image.SetSource(ms);


                   // return image;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public static class WriteableBitmapEx
    {
        /// <summary>
        /// Creates a watermark on the specified image
        /// </summary>
        /// <param name="input">The image to create the watermark from</param>
        /// <param name="watermark">The text to watermark</param>
        /// <param name="color">The color - default is White</param>
        /// <param name="fontSize">The font size - default is 50</param>
        /// <param name="opacity">The opacity - default is 0.25</param>
        /// <param name="hasDropShadow">Specifies if a drop shadow effect must be added - default is true</param>
        /// <returns>The watermarked image</returns>
        public static WriteableBitmap Watermark(this WriteableBitmap input, string watermark, Color color = default(Color), double fontSize = 50, double opacity = 0.25, bool hasDropShadow = true)
        {
            var watermarked = GetTextBitmap(watermark, fontSize, color == default(Color) ? Colors.White : color, opacity, hasDropShadow);

            var width = watermarked.PixelWidth;
            var height = watermarked.PixelHeight;

            var result = input.Clone();

            var position = new Rect(20, input.PixelHeight - height, width, height);
            result.Blit(position, watermarked, new Rect(0, 0, width, height));

            return result;
        }

        /// <summary>
        /// Creates a WriteableBitmap from a text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        /// <param name="opacity"></param>
        /// <param name="hasDropShadow"></param>
        /// <returns></returns>
        private static WriteableBitmap GetTextBitmap(string text, double fontSize, Color color, double opacity, bool hasDropShadow)
        {
            TextBlock txt = new TextBlock();
            txt.Text = text;
            txt.FontSize = fontSize;
            txt.Foreground = new SolidColorBrush(color);
            txt.Opacity = opacity;
            WriteableBitmap bitmap = new WriteableBitmap((int)txt.ActualWidth, (int)txt.ActualHeight);
            bitmap.Render(txt, null);
            bitmap.Invalidate();
            return bitmap;
        }
    }
}
