using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Diagnostics;

namespace ImageViewerControl
{
    public partial class ImageViewer : UserControl
    {
       BitmapImage _image = new BitmapImage();
        private double prevScale = 0;
        private double srcHeight;
        private double srcWitdth;
        private bool updInLayout = false;
        private bool isInPinching = false;
        private double scaleKoef = 2;
        bool isResized = false;

        public string SourceUrl
        {
            get { return (string)GetValue(SourceUrlProperty); }
            set 
            { 
                SetValue(SourceUrlProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceUrlProperty =
            DependencyProperty.Register("SourceUrl", typeof(string), typeof(ImageViewer), new PropertyMetadata("http://cs313829.vk.me/v313829441/18e9/s9ah8cFaphA.jpg"));



        public bool IsNeedToUpdate
        {
            get { return (bool)GetValue(IsNeedToUpdateProperty); }
            set 
            {
                SetValue(IsNeedToUpdateProperty, value);
                if (value == true)
                    ConfigureImage();
            }
        }

        // Using a DependencyProperty as the backing store for IsNeedToUpdate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNeedToUpdateProperty =
            DependencyProperty.Register("IsNeedToUpdate", typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

        

        public string Update
        {
            get { return (string)GetValue(UpdateProperty); }
            set 
            { 
                SetValue(UpdateProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for Update.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateProperty =
            DependencyProperty.Register("Update", typeof(string), typeof(ImageViewer), new PropertyMetadata("22"));



        public ImageViewer()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
            Loaded += ZoomingImage_Loaded;
            this.SizeChanged += ZoomingImage_SizeChanged;
            
            //imageDocument.SetBinding(Image.SourceProperty, new System.Windows.Data.Binding
            //{
            //    Source=this,
            //    Path = new PropertyPath(
            
        }

        void ZoomingImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ConfigureImage();
            //throw new NotImplementedException();
        }

        void ZoomingImage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        const double MaxScale = 10;

        double _scale = 1.0;
        double _minScale;
        double _coercedScale;
        double _originalScale;

        Size _viewportSize;
        bool _pinching;
        Point _screenMidpoint;
        Point _relativeMidpoint;

        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _pinching = false;
            _originalScale = _scale;
        }

        void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;
                isInPinching = true;
                Point center = e.PinchManipulation.Original.Center;
                if (!_pinching)
                {
                    _pinching = true;
                    
                    //_relativeMidpoint = new Point(center.X / imageDocument.ActualWidth, center.Y / imageDocument.ActualHeight);
                    _relativeMidpoint = new Point(center.X / srcWitdth, center.Y /srcHeight);
                    var xform = imageDocument.TransformToVisual(viewport);
                    _screenMidpoint = xform.Transform(center);
                }

                _scale = _originalScale * e.PinchManipulation.CumulativeScale;
                //_scale = _coercedScale * e.PinchManipulation.CumulativeScale;
                CoerceScale(false);
                ResizeImage(false,_coercedScale);
                //viewport.SetViewportOrigin(center);
            }
            else if (_pinching)
            {
                _pinching = false;
                _originalScale = _scale = _coercedScale;
            }
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            _pinching = false;
            _scale = _coercedScale;
        }

        void viewport_ViewportChanged(object sender, ViewportChangedEventArgs e)
        {
            //Size newSize = new Size(viewport.Viewport.Width, viewport.Viewport.Height);
            //if (newSize != _viewportSize)
            //{
            //    _viewportSize = newSize;
            //    CoerceScale(true);
            //    ResizeImage(false);
            //}
        }

        void OnDoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            double currentScale=0;
            if (xform.ScaleX > scaleKoef)
                scaleKoef += 1;
            if (xform.ScaleX == scaleKoef)
            {
                _coercedScale = prevScale;
                xform.ScaleX = xform.ScaleY = prevScale;
                Thread.Sleep(100);
               // currentScale=_coercedScale;
                
            }
            else
            {
                xform.ScaleX = xform.ScaleY = scaleKoef;
                _coercedScale = scaleKoef;
                //currentScale=2;
            }
            double newWidth = canvas.Width = Math.Round(imageDocument.ActualWidth * _coercedScale);
            double newHeight = canvas.Height = Math.Round(imageDocument.ActualHeight * _coercedScale);
            viewport.Bounds = new Rect(0, 0, newWidth, newHeight);
            Point center = e.GetPosition(this);
            center.X = center.X * _coercedScale;//+ viewport.ActualWidth/2;
            center.Y =center.Y * _coercedScale ;
            viewport.SetViewportOrigin(center);
           
            //var xform2 = imageDocument.TransformToVisual(viewport);
            //_screenMidpoint = xform2.Transform(point);


            //if (_scale > _minScale)
            //{
            //    _scale = _minScale;
            //}
            //else
            //{
            //    _scale = 1;

            //Point center = e.GetPosition(imageDocument);
            //Point center = e.GetPosition(this);
            //_relativeMidpoint = new Point(center.X / imageDocument.ActualWidth, center.Y / imageDocument.ActualHeight);
            //_relativeMidpoint = new Point(center.X / srcWitdth, center.Y / srcHeight);

            //var xform2 = imageDocument.TransformToVisual(viewport);
            //_screenMidpoint = xform2.Transform(center);
            //ResizeImage(false, _coercedScale);
            //}
            //CoerceScale(false);
            //_scale = _coercedScale;
            //if (_coercedScale == 1.5)
            //    _coercedScale = 0.66666;
               
            //else
            //    _coercedScale = 1.5;
            //ResizeImage(false);
        }

        void ConfigureImage()
        {
            _scale = 0;
            CoerceScale(true);
            _scale = _coercedScale;
            prevScale = _coercedScale;
            ResizeImage(true,_coercedScale);
            
        }

       public  void ResizeImage(bool center, double scale )
        {
            if (scale != 0)
            {
                //double newWidth = canvas.Width = Math.Round(imageDocument.ActualWidth * _coercedScale);
                //double newHeight = canvas.Height = Math.Round(imageDocument.ActualHeight * _coercedScale);
                //CoerceScale(true);
                double newWidth = canvas.Width = Math.Round(imageDocument.ActualWidth * scale);
                double newHeight = canvas.Height = Math.Round(imageDocument.ActualHeight * scale);
                if (newHeight > 768 || newWidth > 480)
                {

                }
                //double newWidth = canvas.Width = Math.Round(srcWitdth * _coercedScale);
                //double newHeight = canvas.Height = Math.Round(srcHeight * _coercedScale);
                xform.ScaleX = xform.ScaleY = scale;
               
               viewport.Bounds = new Rect(0, 0, newWidth, newHeight);

                if (center)
                {
                    viewport.SetViewportOrigin(
                        new Point(
                        //  3,
                        // 5
                          Math.Round((newWidth - viewport.ActualWidth) / 2),
                         Math.Round((newHeight - viewport.ActualHeight) / 2)
                            ));
                }
                else
                {
                    Point newImgMid = new Point(newWidth * _relativeMidpoint.X, newHeight * _relativeMidpoint.Y);
                    Point origin = new Point(Math.Abs(newImgMid.X - _screenMidpoint.X), Math.Abs(newImgMid.Y - _screenMidpoint.Y));
                    viewport.SetViewportOrigin(origin);
                }
            }
        }

        void CoerceScale(bool recompute)
        {
            if (recompute && _image != null && viewport != null)
            {
                // Calculate the minimum scale to fit the viewport 
                double minX = viewport.ActualWidth / imageDocument.ActualWidth;// .PixelWidth;
                double minY = viewport.ActualHeight / imageDocument.ActualHeight;//.PixelHeight;
                //double minX = viewport.ActualWidth / _image.PixelWidth;
                //double minY = viewport.ActualHeight / _image.PixelHeight;
                //double minX = viewport.ActualWidth / srcWitdth;
                //double minY = viewport.ActualHeight / srcHeight;

                _minScale = Math.Min(minX, minY);
            }

            _coercedScale = Math.Min(MaxScale, Math.Max(_scale, _minScale));
        }

        private void imageDocument_ImageOpened_1(object sender, RoutedEventArgs e)
        {
            //ConfigureImage();
            Debug.WriteLine(SourceUrl + "  ImageOpened_1" );
            SetImageSize();
            ConfigureImage();
            //ResizeImage(true);
            temporaryImage.Visibility = Visibility.Collapsed;
           
        }

        private void imageDocument_Loaded(object sender, RoutedEventArgs e)
        {
            
            //_image.PixelHeight = (int)imageDocument.ActualHeight;
            
             Debug.WriteLine(SourceUrl + " Loaded");
             if (SourceUrl.Contains("isostore"))
             {
                 temporaryImage.Visibility = Visibility.Collapsed;
                 SetImageSize();
                 ConfigureImage();
             }
            //ConfigureImage();
        }

        private void SetImageSize()
        {
            _image = new BitmapImage(new Uri(SourceUrl, UriKind.RelativeOrAbsolute));
            _image.CreateOptions = BitmapCreateOptions.None;
            if (_image.PixelWidth > 0)
            {
                srcHeight = _image.PixelHeight;
                srcWitdth = _image.PixelWidth;
            }
            else
            {
                srcHeight = imageDocument.ActualHeight-1;
                srcWitdth = imageDocument.ActualWidth-1;
            }
        }

        private void imageDocument_LayoutUpdated(object sender, EventArgs e)
        {
           // if (updInLayout == false)
            //{
            //ConfigureImage();
            if (xform.ScaleX != scaleKoef && _image.PixelWidth == 0 && isInPinching == false)
            {
                //Debug.WriteLine(SourceUrl + " LayoutUpdated");
                //_scale = 0;
                //CoerceScale(true);
                //ResizeImage(true, _coercedScale);
                ConfigureImage();
                temporaryImage.Visibility = Visibility.Collapsed;
            }
            else
            {

            }

                //updInLayout = true;
           // }
            //Debug.WriteLine(SourceUrl + " LayoutUpdated");
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isResized == false)
            {
                //_scale = 0;
                //CoerceScale(true);
                ////_originalScale = _coercedScale;
                //ResizeImage(true, _coercedScale);
                //isResized = true;
            }

        }

      
    }
}
