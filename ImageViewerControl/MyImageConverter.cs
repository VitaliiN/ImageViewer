using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        image.CreateOptions = BitmapCreateOptions.DelayCreation;
                        try
                        {
                            image.SetSource(sourceFile);
                        }
                        catch
                        {
                           // return new BitmapImage(new Uri("/VKMagazine;component/Icons/spash_logo.png", UriKind.Relative));
                        }
                        return image;

                    }
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    image.UriSource = new Uri(path,UriKind.RelativeOrAbsolute);
                    return image;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
