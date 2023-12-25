using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace SortingAlgorithms
{
    public static class CreateRectangle
    {
        static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);
        public static Rectangle Create(Brush fillColor, double setLeft, double setTop, double setWidth, double setHeight)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = fillColor;
            rect.Stroke = BlackBrush;
            rect.Width = setWidth;
            rect.Height = setHeight;
            Canvas.SetLeft(rect, setLeft);
            Canvas.SetTop(rect, setTop);
            return rect;
        }

    }
}
