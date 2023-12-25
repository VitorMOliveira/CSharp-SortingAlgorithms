using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using SortingAlgorithms.Algorithms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SortingAlgorithms
{
    internal class SortEngine
    {
        static readonly Brush WhiteBrush = new SolidColorBrush(Colors.White);
        static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);
        static readonly Brush GreenBrush = new SolidColorBrush(Colors.LightGreen);

        private static readonly int rectWidth = SortParameters.RECT_WIDTH;

        public static double[] GetShuffledArray(Canvas SortingCanvas)
        {
            
            int numEntries = (int) (Math.Ceiling(SortingCanvas.ActualWidth) / rectWidth);
            int maxVal = (int)SortingCanvas.ActualHeight + SortParameters.RECT_DRAW_OFFSET_LAST;

            double[] sortingArray = new double[numEntries];

            SortingCanvas.Background = BlackBrush;
            SortingCanvas.Children.Clear();

            // scale 0-maxVal array to 0-numEntries
            for (int i = 0; i < numEntries; i++)
            {
                double scaledValue = ScaleArray(0, numEntries, SortParameters.RECT_DRAW_OFFSET_FIRST, maxVal, i);
                sortingArray[i] = scaledValue;
            }

            sortingArray = ShuffleFisherYates(sortingArray);

            for (int i = 0; i < numEntries; i++)
            {
                //SortingCanvas.Children.Add(CreateRectangle.Create(WhiteBrush, i, maxVal - sortingArray[i], RECT_WIDTH, maxVal));
                SortingCanvas.Children.Add(CreateRectangle.Create(WhiteBrush, i * rectWidth, 0, rectWidth, sortingArray[i]));
            }

            return sortingArray;
        }

        public static double ScaleArray(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
        {
            float OldRange = OldMax - OldMin;
            float NewRange = NewMax - NewMin;
            float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

            return NewValue;
        }

        public static double[] ShuffleFisherYates(double[] array)
        {
            int count = array.Length;
            while (count > 1)
            {
                int i = Random.Shared.Next(count--);
                (array[i], array[count]) = (array[count], array[i]);
            }
            return array;
        }

        public static double[] GetRandomArray(Canvas SortingCanvas, int RECT_WIDTH)
        {
            int numEntries = (int)(SortingCanvas.ActualWidth / RECT_WIDTH);
            int maxVal = (int)SortingCanvas.ActualHeight;

            double[] sortingArray = new double[numEntries];

            SortingCanvas.Background = BlackBrush;
            SortingCanvas.Children.Clear();

            Random rand = new();

            for (int i = 0; i < numEntries; i++)
            {
                sortingArray[i] = rand.Next(0, maxVal);
            }

            for (int i = 0; i < numEntries; i++)
            {
                SortingCanvas.Children.Add(CreateRectangle.Create(WhiteBrush, i * RECT_WIDTH, 0, RECT_WIDTH, sortingArray[i]));
            }

            return sortingArray;
        }

        public static void UpdateCanvas(Canvas _canvas, double[] _sortingArray, int i, int v)
        {
            _canvas.Children.Remove(_canvas.Children[i]);
            //_canvas.Children.Insert(i, CreateRectangle.Create(WhiteBrush, i, _maxVal - _sortingArray[i], 1, _maxVal));
            _canvas.Children.Insert(i, CreateRectangle.Create(WhiteBrush, i * rectWidth, 0, rectWidth, _sortingArray[i]));

            _canvas.Children.Remove(_canvas.Children[v]);
            //_canvas.Children.Insert(v, CreateRectangle.Create(WhiteBrush, v, _maxVal - _sortingArray[v], 1, _maxVal));
            _canvas.Children.Insert(v, CreateRectangle.Create(WhiteBrush, v * rectWidth, 0, rectWidth, _sortingArray[v]));

            _canvas.UpdateLayout();
        }
        public static void UpdateOneElementInCanvas(Canvas _canvas, double[] _sortingArray, int i)
        {
            _canvas.Children.Remove(_canvas.Children[i]);
            _canvas.Children.Insert(i, CreateRectangle.Create(WhiteBrush, i * rectWidth, 0, rectWidth, _sortingArray[i]));

            _canvas.UpdateLayout();
        }

        public static async Task CompletedSorting(MainWindow _mainWindow, double[] _sortingArray, Canvas _canvas, 
                                                  CancellationToken _token)
        {
            for (int i = 0; i < _sortingArray.Length; i++)
            {
                await _mainWindow.Dispatcher.InvokeAsync(() =>
                {
                    var c = _canvas.Children[i];
                    if (c is Shape)
                    {
                        ((Shape)c).Fill = GreenBrush;
                    }

                    SortSoundPlayer.AdjustFrequency(i);

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                });
                Thread.Sleep(SortParameters.COMPLETED_DRAW_DELAY);  
            }

            SortSoundPlayer.StopSoundLoopTask();
        }

        public static List<string> GetAllAlgorithmsNames()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                  .Where(x => typeof(ISortAlgorithms).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                  .Select(x => x.Name).ToList();
        }

    }
}
