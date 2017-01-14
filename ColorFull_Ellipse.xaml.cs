using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bara_Tehni
{
    /// <summary>
    /// Interaction logic for ColorFull_Ellipse.xaml
    /// </summary>
    public partial class ColorFullEllipse : UserControl
    {

        public string PlayerPiece { get; set; }
        public ColorFullEllipse()
        {
            InitializeComponent();
            PlayerPiece = "EMPTY";
        }

        public void SetBrush(Brush imageBrush)
        {
            this.EllipseTool.Fill = imageBrush;
        }

        public void SetStroke(Brush imageBrush,double thickness)
        {
            this.EllipseTool.StrokeThickness = thickness;
            this.EllipseTool.Stroke = imageBrush;
        }

        private void EllipseTool_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
