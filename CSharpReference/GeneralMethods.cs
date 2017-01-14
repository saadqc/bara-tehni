using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bara_Tehni.Methods;
using System.Windows.Media.Effects;


namespace CSharpReference
{
    public class GeneralMethods
    {
        static Color defaultColor = Colors.Blue;
        const double LargeSize = 1.05;
        public static void onMouseEnter_Scale(object sender)
        {
            var obj = sender as UIElement;

            if (obj != null )
            {
                var tn = new TranslateTransform(0.5, 0.5);
                obj.RenderTransform = tn;
                obj.Effect = generateGlowEffect();
                AnimationsEffects.Scale_Effect(obj, 1, LargeSize, 200);              
            }
        }

        public static void onMouseLeave_Scale(object sender)
        {
            var obj = sender as UIElement;

            if (obj != null)
            {
                var tn = new TranslateTransform(0.5, 0.5);
                obj.RenderTransform = tn;
                obj.Effect = null;
                AnimationsEffects.Scale_Effect(obj, LargeSize, 1, 200);
            }
        }

        public static DropShadowEffect generateGlowEffect()
        {
            DropShadowEffect dropShadowEffect = new DropShadowEffect();
            dropShadowEffect.ShadowDepth = 0;
            dropShadowEffect.Color = defaultColor;
            dropShadowEffect.Opacity = 1;
            dropShadowEffect.BlurRadius = 24;
            return dropShadowEffect;
        }
        public static FormatConvertedBitmap ConvertImageToGrayScaleImage(String imagePath)
        {
            // Create a BitmapImage and sets its DecodePixelWidth and DecodePixelHeight
            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bmpImage.EndInit();

            // Create a new image using FormatConvertedBitmap and set DestinationFormat to GrayScale
            FormatConvertedBitmap grayBitmap = new FormatConvertedBitmap();
            grayBitmap.BeginInit();
            grayBitmap.Source = bmpImage;
            grayBitmap.DestinationFormat = PixelFormats.Gray8;
            grayBitmap.EndInit();

            return grayBitmap;
        }

        public static void toggleFullScreen(WindowState wind,bool fullScreen)
        {
            if(fullScreen)
            {
                wind = System.Windows.WindowState.Normal;
                wind = System.Windows.WindowState.Maximized;
            }
            else
            {
                wind = System.Windows.WindowState.Normal;
            }
        }
        
    }
}
